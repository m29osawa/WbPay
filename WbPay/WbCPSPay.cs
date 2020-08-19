using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WbPay
{
	public delegate void WbCPSCompleteEventHandler(WbCPSRequestType type,Object response);
	public delegate void WbCPSErrorEventHandler(WbCPSRequestType type,WbCPSResponseError error);

	public partial class WbCPSPay
	{
		public static string WELLBA_HOME	= @"C:\WellbaHome";
		static string APP_HOME {
			get { return WELLBA_HOME + @"\WbPay"; }
		}
		const string APP_NAME		= "WbCPSPay";
		const string APP_VERSION	= "1.0";

		public event WbCPSCompleteEventHandler CompleteEvent;
		public event WbCPSErrorEventHandler ErrorEvent;

		const int INSTANCE_LOCK_TIMEOUT	= 3000;		// msec
		const int SEQUENCE_LOCK_TIMEOUT	= 2000;		// msec
		const int EVENT_LOCK_TIMEOUT	= 240000;	// msec
		const int PAYCONFIRM_TIMEOUT	= 200000;	// msec

		WbPayLog	log;
		WbPayApp	app;
		WbPayConf	conf;
		bool isOpen;
		SemaphoreSlim sequenceLock;
		object resourceLock = new object();
		object eventLock = new object();
		AutoResetEvent waitPayConfirm;

		int		requestConfirmInterval		= 1000;		//msec
		int		requestConfirmNum			= 1;
		
		bool	payConfirmInProgress		= false;
		volatile bool payConfirmAbort		= false;
		bool	payConfirmPollingMode		= false;
		int		payConfirmPollingWait		= 5000;		// msec
		int		payConfirmPollingInterval	= 3000;		// msec
		int		payConfirmPollingTimeout	= 60000;	// msec

		bool	appTestMode					= false;

		public WbCPSPay()
		{
			try{
				log = new WbPayLog(APP_HOME,APP_NAME);
				log?.Write(WbPayLogLevel.Basic,"WbCPSPay","Start.");

				app = new WbPayApp(APP_HOME,APP_NAME);
				if(!app.StartExclulsive()){
					app = null;
					log?.Write(WbPayLogLevel.Error,"WbCPSPay","WbCPSPay already running.");
					throw new WbPayException(	WbCPSError.LIB,
												WbCPSError.Lib.PROGRAM,
												WbCPSError.Lib.Program.PROGRAM_ERROR,
												"WbCPSPayはすでに動作中です。");
				}

				WbPayConf.Log = log;
				conf = new WbPayConf(APP_HOME,APP_NAME);

				string st;
				st = conf.Get("LogLevel");
				if(!String.IsNullOrEmpty(st)){
					WbPayLogLevel loglevel;
					bool flag = Enum.TryParse<WbPayLogLevel>(st,true,out loglevel);
					if(flag){
						log.LogLebel = loglevel;
					}else{
						log?.Write(WbPayLogLevel.Error,"WbCPSPay","Illegal configuration value : LogLevel=" + st);
						throw new WbPayException(	WbCPSError.LIB,
													WbCPSError.Lib.PARAMETER,
													WbCPSError.Lib.Parameter.ILLEGAL_CONFIG_VALUE,
													"コンフィギュレーション LogLevel=" + st + " の値が不正です。");
					}
				}
				int int_value;
				bool bool_value;
				if(GetConfOptionalInt("LogExpire",out int_value)) log?.Expire(int_value);
				if(GetConfOptionalInt("RequestConfirmInterval",out int_value)) requestConfirmInterval = int_value * 1000;
				if(GetConfOptionalInt("RequestConfirmNum",out int_value)) requestConfirmNum = int_value;
				if(GetConfOptionalYesNo("PayConfirmPollingMode",out bool_value)) payConfirmPollingMode = bool_value;
				if(GetConfOptionalInt("PayConfirmPollingWait",out int_value)) payConfirmPollingWait = int_value * 1000;
				if(GetConfOptionalInt("PayConfirmPollingInterval",out int_value)) payConfirmPollingInterval = int_value * 1000;
				if(GetConfOptionalInt("PayConfirmPollingTimeout",out int_value)) payConfirmPollingTimeout = int_value * 1000;
				if(GetConfOptionalYesNo("AppTestMode",out bool_value)) appTestMode = bool_value;

				if(appTestMode) StartTestMode();

				WbCPSClient.Log = log;
				WbCPSClient.Conf = conf;
				WbCPSClient.Open(APP_HOME,APP_NAME,APP_VERSION);

				sequenceLock = new SemaphoreSlim(1,1);
				waitPayConfirm = new AutoResetEvent(true);
				isOpen = true;
			}catch(Exception){
				app?.Stop();
				log?.Close();
				log = null;
				throw;
			}
		}
		public void Close()
		{
			
			bool ins_lock_flag = false;
			try{
				Monitor.TryEnter(this,INSTANCE_LOCK_TIMEOUT,ref ins_lock_flag);
				if(!ins_lock_flag){
					log?.Write(WbPayLogLevel.Error,"Close","Other method running.");
					throw new WbPayException(	WbCPSError.LIB,
												WbCPSError.Lib.PROGRAM,
												WbCPSError.Lib.Program.PROGRAM_ERROR,
												"プログラムエラー：他のメソッドが実行中です。");
				}
				if(!isOpen){
					log?.Write(WbPayLogLevel.Error,"Close","WbCPSPay not open.");
					throw new WbPayException(	WbCPSError.LIB,
												WbCPSError.Lib.PROGRAM,
												WbCPSError.Lib.Program.PROGRAM_ERROR,
												"プログラムエラー：WbCPSPayがオープンされていません。");
				}
				waitPayConfirm.Dispose();
				sequenceLock.Dispose();
				isOpen = false;
				WbCPSClient.Close();
				WbCPSClient.Log = null;

				app?.Stop();
				app = null;

				log?.Write(WbPayLogLevel.Basic,"WbCPSPay","Stop.");
				log?.Close();
				log = null;
			}finally{
				if(ins_lock_flag) Monitor.Exit(this);
			}
		}

		bool GetConfOptionalInt(string key,out int value)
		{
			value = -1;
			string st = conf.Get(key);
			if(String.IsNullOrEmpty(st)) return false;
			if(Int32.TryParse(st,out value)){
				if(0 <= value) return true;
			}
			log?.Write(WbPayLogLevel.Error,"WbCPSPay","Illegal configuration value : " + key + "=" + st);
			throw new WbPayException(	WbCPSError.LIB,
										WbCPSError.Lib.PARAMETER,
										WbCPSError.Lib.Parameter.ILLEGAL_CONFIG_VALUE,
										"コンフィギュレーション " + key + "=" + st + " の値が不正です。");
		}
		bool GetConfOptionalYesNo(string key,out bool value)
		{
			value = false;
			string st = conf.Get(key);
			if(String.IsNullOrEmpty(st)) return false;
			if(st.Equals("yes",StringComparison.OrdinalIgnoreCase)){
				value = true;
				return true;
			}else if(st.Equals("no",StringComparison.OrdinalIgnoreCase)){
				value = false;
				return true;
			}
			log?.Write(WbPayLogLevel.Error,"WbCPSPay","Illegal configuration value : " + key + "=" + st);
			throw new WbPayException(	WbCPSError.LIB,
										WbCPSError.Lib.PARAMETER,
										WbCPSError.Lib.Parameter.ILLEGAL_CONFIG_VALUE,
										"コンフィギュレーション " + key + "=" + st + "の値が不正です。");
		}

		public void Pay(WbCPSPayRequest request)
		{
			bool ins_lock_flag = false;
			try{
				Monitor.TryEnter(this,INSTANCE_LOCK_TIMEOUT,ref ins_lock_flag);
				if(!ins_lock_flag){
					log?.Write(WbPayLogLevel.Error,"Pay","Other method running.");
					throw new WbPayException(	WbCPSError.LIB,
												WbCPSError.Lib.PROGRAM,
												WbCPSError.Lib.Program.PROGRAM_ERROR,
												"プログラムエラー：他のメソッドが実行中です。");
				}
				if(!isOpen){
					log?.Write(WbPayLogLevel.Error,"Pay","WbCPSPay not open.");
					throw new WbPayException(	WbCPSError.LIB,
												WbCPSError.Lib.PROGRAM,
												WbCPSError.Lib.Program.PROGRAM_ERROR,
												"プログラムエラー：WbCPSPayがオープンされていません。");
				}
				bool lock_flag = false;
				try{
					lock_flag = sequenceLock.Wait(SEQUENCE_LOCK_TIMEOUT);
					if(!lock_flag){
						log?.Write(WbPayLogLevel.Error,"Pay","Request sequence not completed.");
						throw new WbPayException(	WbCPSError.LIB,
													WbCPSError.Lib.REQUEST,
													WbCPSError.Lib.Request.REQUEST_DUPLICATE,
													"他のリクエストシーケンスが実行中です。");
					}
					if(request == null){
						log?.Write(WbPayLogLevel.Error,"Pay","Null argument.");
						throw new ArgumentNullException("request");
					}
					CheckParamAmount(request.Amount,"Pay","Amount");
					CheckParamOptionalString(request.ReceiptNo,32,"Pay","ReceiptNo");
					CheckParamRequiredString(request.UserCode,512,"Pay","UserCode");
					CheckParamOptionalString(request.Remark,128,"Pay","Remark");
					//allowPayConfirm = false;
				}catch(Exception){
					if(lock_flag) sequenceLock.Release();
					throw;
				}
				Task t = Task.Run(() =>
				{
					try{
						if(appTestMode){
							PayTestSequence(request);
						}else{
							PaySequence(request);
						}
					} finally{
						sequenceLock.Release();
					}
				});
			}finally{
				if(ins_lock_flag) Monitor.Exit(this);
			}
		}
		public void PayConfirm(bool confirm_flag)
		{
			bool ins_lock_flag = false;
			try {
				Monitor.TryEnter(this, INSTANCE_LOCK_TIMEOUT, ref ins_lock_flag);
				if(!ins_lock_flag) {
					log?.Write(WbPayLogLevel.Error, "PayConfirm", "Other method running.");
					throw new WbPayException(WbCPSError.LIB,
												WbCPSError.Lib.PROGRAM,
												WbCPSError.Lib.Program.PROGRAM_ERROR,
												"プログラムエラー：他のメソッドが実行中です。");
				}
				if(!isOpen) {
					log?.Write(WbPayLogLevel.Error, "PayConfirm", "WbCPSPay not open.");
					throw new WbPayException(WbCPSError.LIB,
												WbCPSError.Lib.PROGRAM,
												WbCPSError.Lib.Program.PROGRAM_ERROR,
												"プログラムエラー：WbCPSPayがオープンされていません。");
				}
				lock(resourceLock){
					if(!payConfirmInProgress) {
						log?.Write(WbPayLogLevel.Error, "PayConfirm", "Illegal PayConfirm Call.");
						throw new WbPayException(WbCPSError.LIB,
													WbCPSError.Lib.REQUEST,
													WbCPSError.Lib.Request.ILLEGAL_PAYCONFIRM,
													"PayConfirmは顧客支払待ちの場合のみ実行可能です。");
					}
					log?.Write(WbPayLogLevel.Sequence,"PayConfirm",confirm_flag.ToString());
					if(confirm_flag){
						waitPayConfirm.Set();
					}else{
						payConfirmAbort = true;
						waitPayConfirm.Set();
					}
				}
			} finally {
				if(ins_lock_flag) Monitor.Exit(this);
			}
		}
		public void Refund(WbCPSRefundRequest request)
		{
			bool ins_lock_flag = false;
			try{
				Monitor.TryEnter(this,INSTANCE_LOCK_TIMEOUT,ref ins_lock_flag);
				if(!ins_lock_flag){
					log?.Write(WbPayLogLevel.Error,"Refund","Other method running.");
					throw new WbPayException(	WbCPSError.LIB,
												WbCPSError.Lib.PROGRAM,
												WbCPSError.Lib.Program.PROGRAM_ERROR,
												"プログラムエラー：他のメソッドが実行中です。");
				}
				if(!isOpen){
					log?.Write(WbPayLogLevel.Error,"Refund","WbCPSPay not open.");
					throw new WbPayException(	WbCPSError.LIB,
												WbCPSError.Lib.PROGRAM,
												WbCPSError.Lib.Program.PROGRAM_ERROR,
												"プログラムエラー：WbCPSPayがオープンされていません。");
				}
				bool lock_flag = false;
				try{
					lock_flag = sequenceLock.Wait(SEQUENCE_LOCK_TIMEOUT);
					if(!lock_flag){
						log?.Write(WbPayLogLevel.Error,"Refund","Request sequence not completed.");
						throw new WbPayException(	WbCPSError.LIB,
													WbCPSError.Lib.REQUEST,
													WbCPSError.Lib.Request.REQUEST_DUPLICATE,
													"他のリクエストシーケンスが実行中です。");
					}
					if(request == null){
						log?.Write(WbPayLogLevel.Error,"Refund","Null argument.");
						throw new ArgumentNullException("request");
					}
					CheckParamAmount(request.RefundAmount,"Refund","RefundAmount");
					CheckParamRequiredString(request.OrderId,32,"Refund","OrderId");
					CheckParamOptionalString(request.RefundReason,256,"Refund","RefundReason");
					CheckParamOptionalString(request.Remark,256,"Refund","Remark");
				}catch(Exception){
					if(lock_flag) sequenceLock.Release();
					throw;
				}
				Task t = Task.Run(() =>
				{
					try{
						if(appTestMode){
							RefundTestSequence(request);
						}else{
							RefundSequence(request);
						}
					}finally{
						sequenceLock.Release();
					}
				});
			}finally{
				if(ins_lock_flag) Monitor.Exit(this);
			}
		}
		public void Reverse(WbCPSReverseRequest request)
		{
			bool ins_lock_flag = false;
			try{
				Monitor.TryEnter(this,INSTANCE_LOCK_TIMEOUT,ref ins_lock_flag);
				if(!ins_lock_flag){
					log?.Write(WbPayLogLevel.Error,"Reverse","Other method running.");
					throw new WbPayException(	WbCPSError.LIB,
												WbCPSError.Lib.PROGRAM,
												WbCPSError.Lib.Program.PROGRAM_ERROR,
												"プログラムエラー：他のメソッドが実行中です。");
				}
				if(!isOpen){
					log?.Write(WbPayLogLevel.Error,"Reverse","WbCPSPay not open.");
					throw new WbPayException(	WbCPSError.LIB,
												WbCPSError.Lib.PROGRAM,
												WbCPSError.Lib.Program.PROGRAM_ERROR,
												"プログラムエラー：WbCPSPayがオープンされていません。");
				}
				bool lock_flag = false;
				try{
					lock_flag = sequenceLock.Wait(SEQUENCE_LOCK_TIMEOUT);
					if(!lock_flag){
						log?.Write(WbPayLogLevel.Error,"Reverse","Request sequence not completed.");
						throw new WbPayException(	WbCPSError.LIB,
													WbCPSError.Lib.REQUEST,
													WbCPSError.Lib.Request.REQUEST_DUPLICATE,
													"他のリクエストシーケンスが実行中です。");
					}
					if(request == null){
						log?.Write(WbPayLogLevel.Error,"Reverse","Null argument.");
						throw new ArgumentNullException("request");
					}
					CheckParamRequiredString(request.OrderId,32,"Reverse","OrderId");
				}catch(Exception){
					if(lock_flag) sequenceLock.Release();
					throw;
				}
				Task t = Task.Run(() =>
				{
					try{
						if(appTestMode){
							ReverseTestSequence(request);
						}else{
							ReverseSequence(request);
						}
					}finally{
						sequenceLock.Release();
					}
				});
			}finally{
				if(ins_lock_flag) Monitor.Exit(this);
			}
		}
		//public void Confirm(WbCPSConfirmRequest request)
		//{
		//	bool ins_lock_flag = false;
		//	try{
		//		Monitor.TryEnter(this,INSTANCE_LOCK_TIMEOUT,ref ins_lock_flag);
		//		if(!ins_lock_flag){
		//			log?.Write(WbPayLogLevel.Error,"Confirm","Other method running.");
		//			throw new WbPayException(	WbCPSError.LIB,
		//										WbCPSError.Lib.PROGRAM,
		//										WbCPSError.Lib.Program.PROGRAM_ERROR,
		//										"プログラムエラー：他のメソッドが実行中です。");
		//		}
		//		if(!isOpen){
		//			log?.Write(WbPayLogLevel.Error,"Confirm","WbCPSPay not open.");
		//			throw new WbPayException(	WbCPSError.LIB,
		//										WbCPSError.Lib.PROGRAM,
		//										WbCPSError.Lib.Program.PROGRAM_ERROR,
		//										"プログラムエラー：WbCPSPayがオープンされていません。");
		//		}
		//		bool lock_flag = false;
		//		try{
		//			lock_flag = sequenceLock.Wait(SEQUENCE_LOCK_TIMEOUT);
		//			if(!lock_flag){
		//				log?.Write(WbPayLogLevel.Error,"Confirm","Request sequence not completed.");
		//				throw new WbPayException(	WbCPSError.LIB,
		//											WbCPSError.Lib.REQUEST,
		//											WbCPSError.Lib.Request.REQUEST_DUPLICATE,
		//											"他のリクエストシーケンスが実行中です。");
		//			}
		//			if(request == null){
		//				log?.Write(WbPayLogLevel.Error,"Confirm","Null argument.");
		//				throw new ArgumentNullException("request");
		//			}
		//			CheckParamRequiredString(request.OrderId,32,"Confirm","OrderId");
		//		}catch(Exception){
		//			if(lock_flag) sequenceLock.Release();
		//			throw;
		//		}
		//		Task t = Task.Run(() =>
		//		{
		//			try{
		//				if(appTestMode){
		//					ConfirmTestSequence(request);
		//				}else{
		//					ConfirmSequence(request);
		//				}
		//			}finally{
		//				sequenceLock.Release();
		//			}
		//		});
		//	}finally{
		//		if(ins_lock_flag) Monitor.Exit(this);
		//	}
		//}
		public void Deposit(WbCPSDepositRequest request)
		{
			bool ins_lock_flag = false;
			try{
				Monitor.TryEnter(this,INSTANCE_LOCK_TIMEOUT,ref ins_lock_flag);
				if(!ins_lock_flag){
					log?.Write(WbPayLogLevel.Error,"Deposit","Other method running.");
					throw new WbPayException(	WbCPSError.LIB,
												WbCPSError.Lib.PROGRAM,
												WbCPSError.Lib.Program.PROGRAM_ERROR,
												"プログラムエラー：他のメソッドが実行中です。");
				}
				if(!isOpen){
					log?.Write(WbPayLogLevel.Error,"Deposit","WbCPSPay not open.");
					throw new WbPayException(	WbCPSError.LIB,
												WbCPSError.Lib.PROGRAM,
												WbCPSError.Lib.Program.PROGRAM_ERROR,
												"プログラムエラー：WbCPSPayがオープンされていません。");
				}
				bool lock_flag = false;
				try{
					lock_flag = sequenceLock.Wait(SEQUENCE_LOCK_TIMEOUT);
					if(!lock_flag){
						log?.Write(WbPayLogLevel.Error,"Deposit","Request sequence not completed.");
						throw new WbPayException(	WbCPSError.LIB,
													WbCPSError.Lib.REQUEST,
													WbCPSError.Lib.Request.REQUEST_DUPLICATE,
													"他のリクエストシーケンスが実行中です。");
					}
					if(request == null){
						log?.Write(WbPayLogLevel.Error,"Deposit","Null argument.");
						throw new ArgumentNullException("request");
					}
					CheckParamAmount(request.Amount,"Deposit","Amount");
					CheckParamOptionalString(request.ReceiptNo,32,"Deposit","ReceiptNo");
					CheckParamRequiredString(request.UserCode,32,"Deposit","UserCode");
				}catch(Exception){
					if(lock_flag) sequenceLock.Release();
					throw;
				}
				Task t = Task.Run(() =>
				{
					try{
						if(appTestMode){
							DepositTestSequence(request);
						}else{
							DepositSequence(request);
						}
					}finally{
						sequenceLock.Release();
					}
				});
			}finally{
				if(ins_lock_flag) Monitor.Exit(this);
			}
		}

		void InvokeErrorEvent(WbCPSRequestType type,WbCPSResponseError error)
		{
			Task t = Task.Run(() =>
			{
				bool lock_flag = false;
				try{
					Monitor.TryEnter(eventLock,EVENT_LOCK_TIMEOUT,ref lock_flag);
					if(!lock_flag){
						log?.Write(WbPayLogLevel.Warning,type.ToString(),"InvokeErrorEvent Synchronize fail.");
					}
					ErrorEvent?.Invoke(type,error);
				}finally{
					if(lock_flag) Monitor.Exit(eventLock);
				}
			});
		}
		void InvokeCompleteEvent(WbCPSRequestType type,Object response)
		{
			Task t = Task.Run(() =>
			{
				bool lock_flag = false;
				try{
					Monitor.TryEnter(eventLock,EVENT_LOCK_TIMEOUT,ref lock_flag);
					if(!lock_flag){
						log?.Write(WbPayLogLevel.Warning,type.ToString(),"InvokeCompleteEvent Synchronize fail.");
					}
					CompleteEvent?.Invoke(type,response);
				}finally{
					if(lock_flag) Monitor.Exit(eventLock);
				}
			});
		}
		
		void PaySequence(WbCPSPayRequest request)
		{
			WbCPSResponseError error = null;

			WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Pay,"Request",LogStPayRequest(request));

			WbCPSPayResult pay_result = TryPay(request);
			if(pay_result.IsSuccess){
				if(pay_result.Response.TransStatus == 0){			// 支払成功
					WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Pay,"Response",LogStPayResponse(pay_result.Response));
					InvokeCompleteEvent(WbCPSRequestType.Pay,pay_result.Response);
					return;
				}else if(pay_result.Response.TransStatus == 14){	// 顧客支払待ち
					lock(resourceLock){
						payConfirmInProgress = true;
						payConfirmAbort = false;
						waitPayConfirm.Reset();
					}
					WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Pay,"Response",LogStPayResponse(pay_result.Response));
					InvokeCompleteEvent(WbCPSRequestType.Pay,pay_result.Response);
					WbCPSConfirmResult confirm_result = PayConfirmSequence(pay_result.Token);
					lock(resourceLock){
						payConfirmInProgress = false;
					}
					if(confirm_result.IsSuccess){
						if(confirm_result.Response.TransStatus == 0){		// 支払成功
							pay_result.Response.OrderId = confirm_result.Response.OrderId;
							pay_result.Response.TransTime = confirm_result.Response.TransTime;
							pay_result.Response.TransStatus = confirm_result.Response.TransStatus;
							pay_result.Response.PayType = confirm_result.Response.PayType;
							WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Pay,"Response",LogStPayResponse(pay_result.Response));
							InvokeCompleteEvent(WbCPSRequestType.Pay,pay_result.Response);
							return;
						}else if(confirm_result.Response.TransStatus == 15){	// 顧客支払中止　取消へ
							error = CreateErrorTransStatus(confirm_result.Response.TransStatus);
							WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Pay,"Response",LogStResponseError(error));
							InvokeErrorEvent(WbCPSRequestType.Pay,error);
						}else{													// なんらかのエラー　取消へ
							error = CreateErrorTransStatus(confirm_result.Response.TransStatus);
							WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Pay,"Response",LogStResponseError(error));
							InvokeErrorEvent(WbCPSRequestType.Pay,error);
						}
					}else{
						error = CreateError(confirm_result.Error);
						WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Pay,"Response", LogStResponseError(error));
						InvokeErrorEvent(WbCPSRequestType.Pay, error);
						if(IsOrderNotExistError(confirm_result.Error)){
							return;
						}else{
							;		// 取消へ
						}
					}
				}else if(pay_result.Response.TransStatus == 15){	// 顧客支払中止　取消へ
					error = CreateErrorTransStatus(pay_result.Response.TransStatus);
					WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Pay,"Response",LogStResponseError(error));
					InvokeErrorEvent(WbCPSRequestType.Pay,error);
				}else{												// なんらかのエラー　取消へ
					error = CreateErrorTransStatus(pay_result.Response.TransStatus);
					WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Pay,"Response",LogStResponseError(error));
					InvokeErrorEvent(WbCPSRequestType.Pay,error);
				}
			}else{
				error = CreateError(pay_result.Error);
				WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Pay,"Response",LogStResponseError(error));
				InvokeErrorEvent(WbCPSRequestType.Pay,error);

				if(IsNetError(pay_result.Error)){
					;				// 取消へ
				}else if(IsInProgressError(pay_result.Error)){
					;				//　取消へ
				}else if(IsOrderNotExistError(pay_result.Error)){
					return;
				}else{
					switch(pay_result.Error.Type){
						case WbCPSError.LIB:	// 内部エラー
							return;
						case 9:	// PAYTREEエラー
						default:
							break;	//　取消へ
					}
				}
			}

			WbCPSReverseRequest reverse_request = new WbCPSReverseRequest();
			string reverse_token = null;
			if(pay_result.IsSuccess){
				reverse_request.OrderId = pay_result.Response.OrderId;
			}else{
				reverse_token = pay_result.Token;
			}
			WbCPSReverseResult reverse_result = TryReverse(reverse_request,reverse_token);
			if(reverse_result.IsSuccess){
				if(reverse_result.Response.TransStatus == 10){	// 取消成功
					return;
				}else{											// なんらかのエラー
					error = CreateErrorTransStatus(reverse_result.Response.TransStatus);
					WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Pay,"Response",LogStResponseError(error));
					InvokeErrorEvent(WbCPSRequestType.Pay,error);
					return;
				}
			}else{
				if(IsOrderNotExistError(reverse_result.Error)){
					return;
				}else{
					error = CreateError(reverse_result.Error);
					WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Pay,"Response",LogStResponseError(error));
					InvokeErrorEvent(WbCPSRequestType.Pay,error);
					return;
				}
			}
		}
		WbCPSConfirmResult PayConfirmSequence(string token)
		{
			WbCPSConfirmResult result = null;
			WbCPSResponseError error = null;
			DateTime limit_time;
			if(payConfirmPollingMode){
				limit_time = DateTime.Now.AddMilliseconds(payConfirmPollingTimeout);
			}else{
				limit_time = DateTime.Now.AddMilliseconds(PAYCONFIRM_TIMEOUT);
			}
			TimeSpan rest =  limit_time - DateTime.Now;
			for(int i = 0;TimeSpan.Zero < rest;i++){
				if(payConfirmPollingMode){
					if(i == 0) {
						if(0 < payConfirmPollingWait) Thread.Sleep(payConfirmPollingWait);
					} else {
						if(0 < payConfirmPollingInterval) Thread.Sleep(payConfirmPollingInterval);
					}
				}else{
					if(!waitPayConfirm.WaitOne(rest)) break;
				}
				lock(resourceLock){
					if(payConfirmAbort) break;
				}
				result = TryConfirm(new WbCPSConfirmRequest(),token);
				if(result.IsSuccess) {
					if(result.Response.TransStatus == 0) {			// 支払成功
						return result;
					}else if(result.Response.TransStatus == 14){	// 顧客支払待ち
						if(payConfirmPollingMode){
							;												// ポーリング継続
						}else{
							error = CreateErrorTransStatus(result.Response.TransStatus);
							WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Pay,"Response",LogStResponseError(error));
							InvokeErrorEvent(WbCPSRequestType.Pay,error);	// エラー通知後、再度待ち
						}
					}else if(result.Response.TransStatus == 15){	// 顧客支払中止
						return result;
					}else{											// なんらかのエラー
						return result;
					}
				} else {
					return result;
				}
				rest =  limit_time - DateTime.Now;
			}
			if(result == null){
				result = new WbCPSConfirmResult();
				result.Response = new WbCPSConfirmResponse();
				result.IsSuccess = true;
			}
			result.Response.TransStatus = 1;	// 強制支払失敗
			return result;
		}
		void RefundSequence(WbCPSRefundRequest request)
		{
			WbCPSResponseError error = null;

			WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Refund,"Request",LogStRefundRequest(request));

			WbCPSRefundResult refund_result = TryRefund(request);
			if(refund_result.IsSuccess){
				if(refund_result.Response.TransStatus == 6){	// 返金済
					WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Refund,"Response",LogStRefundResponse(refund_result.Response));
					InvokeCompleteEvent(WbCPSRequestType.Refund,refund_result.Response);
					return;
				}else{											// なんらかのエラー
					error = CreateErrorTransStatus(refund_result.Response.TransStatus);
					WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Refund,"Response", LogStResponseError(error));
					InvokeErrorEvent(WbCPSRequestType.Refund, error);
				}
			}else{
				// すでに返金済みの場合もエラーになる
				error = CreateError(refund_result.Error);
				WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Refund,"Response",LogStResponseError(error));
				InvokeErrorEvent(WbCPSRequestType.Refund,error);
				return;
			}
		}
		void ReverseSequence(WbCPSReverseRequest request)
		{
			WbCPSResponseError error = null;

			WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Reverse,"Request",LogStReverseRequest(request));

			WbCPSReverseResult reverse_result =　TryReverse(request,null);
			if(reverse_result.IsSuccess){
				if(reverse_result.Response.TransStatus == 10
					|| reverse_result.Response.TransStatus == 25){	// 取消成功
					WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Reverse,"Response",LogStReverseResponse(reverse_result.Response));
					InvokeCompleteEvent(WbCPSRequestType.Reverse,reverse_result.Response);
					return;
				}else{										// なんらかのエラーで取消失敗扱い
					error = CreateErrorTransStatus(reverse_result.Response.TransStatus);
					WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Reverse,"Response",LogStResponseError(error));
					InvokeErrorEvent(WbCPSRequestType.Reverse,error);
				}
			}else{
				// すでに取消済みの場合もエラーになる
				error = CreateError(reverse_result.Error);
				WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Reverse,"Response",LogStResponseError(error));
				InvokeErrorEvent(WbCPSRequestType.Reverse,error);
				return;
			}
		}
		//void ConfirmSequence(WbCPSConfirmRequest request)
		//{
		//	WbCPSResponseError error = null;

		//	WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Confirm ,"Request",LogStConfirmRequest(request));

		//	WbCPSConfirmResult confirm_result = TryConfirm(request,null);
		//	if(confirm_result.IsSuccess){
		//		WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Confirm,"Response",LogStConfirmResponse(confirm_result.Response));
		//		InvokeCompleteEvent(WbCPSRequestType.Confirm,confirm_result.Response);
		//		return;
		//	}else{
		//		error = CreateError(confirm_result.Error);
		//		WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Confirm,"Response",LogStResponseError(error));
		//		InvokeErrorEvent(WbCPSRequestType.Confirm,error);
		//		return;
		//	}
		//}
		void DepositSequence(WbCPSDepositRequest request)
		{
			WbCPSResponseError error = null;

			WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Deposit,"Request",LogStDepositRequest(request));

			WbCPSDepositResult deposit_result = TryDeposit(request);
			if(deposit_result.IsSuccess){
				if(deposit_result.Response.TransStatus == 18){	// 入金成功
					WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Deposit,"Response",LogStDepositResponse(deposit_result.Response));
					InvokeCompleteEvent(WbCPSRequestType.Deposit,deposit_result.Response);
					return;
				}else{	// なんらかのエラー
					error = CreateErrorTransStatus(deposit_result.Response.TransStatus);
					WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Deposit,"Response",LogStResponseError(error));
					InvokeErrorEvent(WbCPSRequestType.Deposit,error);
				}
			}else{
				error = CreateError(deposit_result.Error);
				WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Deposit,"Response",LogStResponseError(error));
				InvokeErrorEvent(WbCPSRequestType.Deposit,error);

				if(IsNetError(deposit_result.Error)){
					;					// 取消へ
				}else if(IsInProgressError(deposit_result.Error)){
					;					// 取消へ
				}else if(IsOrderNotExistError(deposit_result.Error)){
					return;
				}else{
					switch(deposit_result.Error.Type){
						case WbCPSError.LIB:	// 内部エラー
							return;
						case 9:	// PAYTREEエラー
						default:
							break;						// 取消へ
					}
				}
			}

			WbCPSReverseResult reverse_result = TryReverse(new WbCPSReverseRequest(),deposit_result.Token);
			if(reverse_result.IsSuccess){
				if(reverse_result.Response.TransStatus == 10){	// 取消成功
					return;
				}else{											// なんらかのエラー
					error = CreateErrorTransStatus(reverse_result.Response.TransStatus);
					WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Deposit,"Response",LogStResponseError(error));
					InvokeErrorEvent(WbCPSRequestType.Deposit,error);
					return;
				}
			}else{
				error = CreateError(reverse_result.Error);
				WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Deposit,"Response",LogStResponseError(error));
				InvokeErrorEvent(WbCPSRequestType.Deposit,error);
				return;
			}
		}

		WbCPSPayResult TryPay(WbCPSPayRequest request)
		{
			WbCPSPayResult result = WbCPSClient.PayRequest(request);
			if(result.IsSuccess){
				if(result.Response.TransStatus == 0){			// 支払成功
					return result;
				}else if(result.Response.TransStatus == 14){	// 顧客支払待ち
					return result;
				}else if(result.Response.TransStatus == 15){	// 顧客支払中止
					return result;
				}												// なんらかのエラーは確認へ
			}else{
				if(IsNetError(result.Error)){
					WbCPSResponseError error = CreateError(result.Error);
					WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Pay,"Response",LogStResponseError(error));
					;			// 確認へ
				}else if(IsInProgressError(result.Error)){
					;			// 確認へ
				}else{
					return result;
				}
			}

			if(0 < requestConfirmInterval) Thread.Sleep(requestConfirmInterval);

			WbCPSConfirmResult confirm_result = TryConfirm(new WbCPSConfirmRequest(),result.Token);
			WbCPSPayResult new_result = new WbCPSPayResult();
			new_result.IsSuccess = confirm_result.IsSuccess;
			new_result.Error = confirm_result.Error;
			if(confirm_result.IsSuccess){
				WbCPSPayResponse new_response = new WbCPSPayResponse();
				new_response.OrderId = confirm_result.Response.OrderId;
				new_response.TransTime = confirm_result.Response.TransTime;
				new_response.Amount = request.Amount;
				new_response.TransStatus = confirm_result.Response.TransStatus;
				new_response.PayType = confirm_result.Response.PayType;
				new_result.Response = new_response;
			}
			new_result.Token = result.Token;
			return new_result;
		}
		WbCPSRefundResult TryRefund(WbCPSRefundRequest request)
		{
			WbCPSRefundResult result = WbCPSClient.RefundRequest(request);
			if(result.IsSuccess){
				if(result.Response.TransStatus == 6){	// 返金済
					return result;
				}										// なんらかのエラーは確認へ
			}else{
				if(IsNetError(result.Error)){
					WbCPSResponseError error = CreateError(result.Error);
					WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Refund,"Response",LogStResponseError(error));
					;	// 確認へ
				}else if(IsInProgressError(result.Error)){
					;	// 確認へ
				}else{
					return result;
				}
			}

			if(0 < requestConfirmInterval) Thread.Sleep(requestConfirmInterval);

			WbCPSConfirmResult confirm_result = TryConfirm(new WbCPSConfirmRequest(),result.Token);
			WbCPSRefundResult new_result = new WbCPSRefundResult();
			new_result.IsSuccess = confirm_result.IsSuccess;
			new_result.Error = confirm_result.Error;
			if(confirm_result.IsSuccess){	
				WbCPSRefundResponse new_response = new WbCPSRefundResponse();
				new_response.OrderId = confirm_result.Response.OrderId;
				new_response.TransStatus = confirm_result.Response.TransStatus;
				new_response.RefundAmount = request.RefundAmount;
				new_response.TransTime = confirm_result.Response.TransTime;
				new_result.Response = new_response;
			}
			new_result.Token = result.Token;	
			return new_result;
		}
		WbCPSReverseResult TryReverse(WbCPSReverseRequest request,string token)
		{
			WbCPSReverseResult result = WbCPSClient.ReverseRequest(request,token);
			if(result.IsSuccess){
				if(result.Response.TransStatus == 10){　// 取消成功
					return result;
				}										// なんらかのエラーは確認へ
			}else{
				if(IsNetError(result.Error)){
					WbCPSResponseError error = CreateError(result.Error);
					WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Reverse,"Response",LogStResponseError(error));
					;	// 確認へ
				}else if(IsInProgressError(result.Error)){
					;	// 確認へ
				}else{
					return result;
				}
			}

			if(0 < requestConfirmInterval) Thread.Sleep(requestConfirmInterval);

			WbCPSConfirmResult confirm_result = TryConfirm(new WbCPSConfirmRequest(),result.Token);
			WbCPSReverseResult new_result = new WbCPSReverseResult();
			new_result.IsSuccess = confirm_result.IsSuccess;
			new_result.Error = confirm_result.Error;
			if(confirm_result.IsSuccess){	
				WbCPSReverseResponse new_response = new WbCPSReverseResponse();
				new_response.OrderId = confirm_result.Response.OrderId;
				new_response.TransStatus = confirm_result.Response.TransStatus;
				new_response.Amount = 0M;				// 注意　Amountが0の場合がある
				new_response.TransTime = confirm_result.Response.TransTime;
				new_result.Response = new_response;
			}
			new_result.Token = result.Token;
			return new_result;
		}
		WbCPSConfirmResult TryConfirm(WbCPSConfirmRequest request,string token)
		{
			int num = 0;
			WbCPSConfirmResult result = null;
			do{
				if(0 < num){
					if(0 < requestConfirmInterval) Thread.Sleep(requestConfirmInterval);
				}
				result = WbCPSClient.ConfirmRequest(request,token);
				if(result.IsSuccess){
					return result;
				}else{
					if(IsNetError(result.Error)){
						WbCPSResponseError error = CreateError(result.Error);
						WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Confirm,"Response",LogStResponseError(error));
						;			// 再確認へ
					}else if(IsInProgressError(result.Error)){
						;			// 再確認へ
					}else{
						return result;
					}
				}
				num ++;
			}while(num < requestConfirmNum);
			return result;
		}
		WbCPSDepositResult TryDeposit(WbCPSDepositRequest request)
		{
			WbCPSDepositResult result = WbCPSClient.DepositRequest(request);
			if(result.IsSuccess){
				if(result.Response.TransStatus == 18){	// 入金成功
					return result;
				}										// なんらかのエラーは確認へ
			}else{
				if(IsNetError(result.Error)){
					WbCPSResponseError error = CreateError(result.Error);
					WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Deposit,"Response",LogStResponseError(error));
					;			// 確認へ
				}else if(IsInProgressError(result.Error)){
					;			// 確認へ
				}else{
					return result;
				}
			}

			if(0 < requestConfirmInterval) Thread.Sleep(requestConfirmInterval);

			WbCPSConfirmResult confirm_result = TryConfirm(new WbCPSConfirmRequest(),result.Token);
			WbCPSDepositResult new_result = new WbCPSDepositResult();
			new_result.IsSuccess = confirm_result.IsSuccess;
			new_result.Error = confirm_result.Error;
			if(confirm_result.IsSuccess){
				WbCPSDepositResponse new_response = new WbCPSDepositResponse();
				new_response.OrderId = confirm_result.Response.OrderId;
				new_response.TransTime = confirm_result.Response.TransTime;
				new_response.Amount = request.Amount;
				new_response.TransStatus = confirm_result.Response.TransStatus;
				new_result.Response = new_response;
			}
			new_result.Token = result.Token;
			return new_result;
		}
		
		bool IsOrderNotExistError(WbCPSErrorResult error){
			switch(error.Type){
				case WbCPSError.CPS_NETWORK:
				case WbCPSError.LIB:
					break;
				case WbCPSError.CPS:
					switch(error.SubType){
						case WbCPSError.Cps.PARAMETER:
							switch(error.Code){
								case 3:
								case 17:
								case 18:
								case 19:
								case 21:
									return true;
							}
							break;
						case WbCPSError.Cps.SYSTEM:
							break;
						case WbCPSError.Cps.PROCESSOR:
							switch(error.SubCode){
								case WbCPSError.Cps.Processor.TRADE_NOT_EXIST:
								case WbCPSError.Cps.Processor.TRANSACTION_NOT_FOUND:
									return true;
							}
							break;
					}
					break;
			}
			return false;
		}
		bool IsNetError(WbCPSErrorResult error)
		{
			switch(error.Type){
				case WbCPSError.CPS_NETWORK:	// PAYTREEネットワークエラー
					return true;
				case WbCPSError.LIB:	// 内部エラー
					switch(error.SubType){
						case WbCPSError.Lib.RESPONSE:	//POST後のエラー
						case WbCPSError.Lib.HTTP:	// Httpタイムアウトの場合がある
							return true;
						default:	// 内部のみのエラー
							break;
					}
					break;
				case WbCPSError.CPS:	// PAYTREEエラー
					switch(error.SubType){
						case WbCPSError.Cps.PARAMETER:
							break;
						case WbCPSError.Cps.SYSTEM:	// システムエラー
							return true;
						case WbCPSError.Cps.PROCESSOR:	// プロセッサエラー
						case WbCPSError.Cps.SECURITY:
							break;
					}
					break;
			}
			return false;
		}
		bool IsInProgressError(WbCPSErrorResult error)
		{
			switch(error.Type){
				case WbCPSError.CPS_NETWORK:	// PAYTREEネットワークエラー
				case WbCPSError.LIB:	// 内部エラー
					break;
				case WbCPSError.CPS:	// PAYTREEエラー
					switch(error.SubType){
						case WbCPSError.Cps.PARAMETER:
						case WbCPSError.Cps.SYSTEM:	// システムエラー
							break;
						case WbCPSError.Cps.PROCESSOR:	// プロセッサエラー
							switch(error.SubCode){
								case WbCPSError.Cps.Processor.PAYMENT_IN_PROGRESS:
								case WbCPSError.Cps.Processor.REQUEST_DUPLICATED:
									return true;
								default:
									break;
							}
							break;
						case WbCPSError.Cps.SECURITY:
							break;
					}
					break;
			}
			return false;
		}
		
		void CheckParamRequiredString(string target,int maxlen,string proc_st,string name_st)
		{
			if(String.IsNullOrEmpty(target)){
				log?.Write(WbPayLogLevel.Error,"Pay","Required request parameter not found. : " + name_st);
				throw new WbPayException(	WbCPSError.LIB,
											WbCPSError.Lib.PARAMETER,
											WbCPSError.Lib.Parameter.PARAMETER_REQUIRED,
											"リクエストパラメータ " + name_st + " は必須です。");
			}
			if(maxlen < target.Length){
				log?.Write(WbPayLogLevel.Error,proc_st,"Illegal request parameter. : " + name_st);
				throw new WbPayException(	WbCPSError.LIB,
											WbCPSError.Lib.PARAMETER,
											WbCPSError.Lib.Parameter.ILLEGAL_PARAMETER,
											"リクエストパラメータ " + name_st + " が不正です。");
			}
		}
		void CheckParamOptionalString(string target,int maxlen,string proc_st,string name_st)
		{
			if(String.IsNullOrEmpty(target)) return;
			if(maxlen < target.Length){
				log?.Write(WbPayLogLevel.Error,proc_st,"Illegal request parameter. : " + name_st);
				throw new WbPayException(	WbCPSError.LIB,
											WbCPSError.Lib.PARAMETER,
											WbCPSError.Lib.Parameter.ILLEGAL_PARAMETER,
											"リクエストパラメータ " + name_st + " が不正です。");
			}
		}
		void CheckParamAmount(decimal amount,string proc_st,string name_st)
		{
			if(amount < 0M || 1_000_000_000_000_000M <= amount){
				log?.Write(WbPayLogLevel.Error,proc_st,"Illegal request parameter. : " + name_st);
				throw new WbPayException(	WbCPSError.LIB,
											WbCPSError.Lib.PARAMETER,
											WbCPSError.Lib.Parameter.ILLEGAL_PARAMETER,
											"リクエストパラメータ " + name_st + " が不正です。");
			}
		}

		WbCPSResponseError CreateError(WbCPSErrorResult result)
		{
			WbCPSResponseError error = new WbCPSResponseError();

			error.Type = result.Type;
			error.Code = String.Format("E{0:D2}{1:D1}{2:D2}",result.Type,result.SubType,result.Code);
			error.SubCode = result.SubCode;
			error.Message = result.Message;
			if(result.Type == WbCPSError.LIB){
				error.RequestAction = WbCPSAction.VendorCall;
			}else if(result.Type == WbCPSError.CPS
							&& result.SubType == WbCPSError.Cps.PROCESSOR){
				error.RequestAction= WbCPSError.Cps.Processor.GetRequsetAction(result.SubCode);
			}else{
				error.RequestAction = WbCPSAction.CPSCall;
			}
			return error;
		}
		WbCPSResponseError CreateErrorTransStatus(int status)
		{
			WbCPSResponseError error = new WbCPSResponseError();

			error.Type = WbCPSError.POST_PROCESS;
			error.Code = String.Format("E{0:D2}{1:D1}{2:D2}",error.Type,WbCPSError.PostProcess.STYPE,status);
			error.SubCode = "";
			error.Message = WbCPSError.PostProcess.GetErrorMessage(status);
			error.RequestAction = WbCPSAction.CPSCall;

			return error;
		}
		
		void WriteLog(WbPayLogLevel level,WbCPSRequestType type,string proc,string message)
		{
			log?.Write(level,type.ToString() + proc,message);
		}

		StringBuilder workLogString = new StringBuilder();
		string LogStResponseError(WbCPSResponseError error)
		{
			workLogString.Clear();
			workLogString.Append("{Type=");
			workLogString.Append(error.Type.ToString());
			workLogString.Append(",Code=");
			workLogString.Append(error.Code.ToString());
			workLogString.Append(",SubCode=");
			workLogString.Append(error.SubCode);
			workLogString.Append(",Message=");
			workLogString.Append(error.Message);
			workLogString.Append(",RequestAction=");
			workLogString.Append(error.RequestAction.ToString());
			workLogString.Append("}");
			return workLogString.ToString();
		}
		string LogStPayRequest(WbCPSPayRequest request)
		{
			workLogString.Clear();
			workLogString.Append("{PayType=");
			workLogString.Append(request.PayType.ToString());
			workLogString.Append(",Amount=");
			workLogString.Append(request.Amount.ToString());
			workLogString.Append(",ReceiptNo=");
			workLogString.Append(request.ReceiptNo);
			workLogString.Append(",UserCode=");
			workLogString.Append(request.UserCode);
			workLogString.Append(",Remark=");
			workLogString.Append(request.Remark);
			workLogString.Append("}");
			return workLogString.ToString();
		}
		string LogStPayResponse(WbCPSPayResponse response)
		{
			workLogString.Clear();
			workLogString.Append("{OrderId=");
			workLogString.Append(response.OrderId);
			workLogString.Append(",TransTime=");
			workLogString.Append(response.TransTime);
			workLogString.Append(",Amount=");
			workLogString.Append(response.Amount.ToString());
			workLogString.Append(",TransStatus=");
			workLogString.Append(response.TransStatus.ToString());
			workLogString.Append(",PayType=");
			workLogString.Append(response.PayType.ToString());
			workLogString.Append("}");
			return workLogString.ToString();
		}
		string LogStRefundRequest(WbCPSRefundRequest request)
		{
			workLogString.Clear();
			workLogString.Append("{RefundAmount=");
			workLogString.Append(request.RefundAmount.ToString());
			workLogString.Append(",OrderId=");
			workLogString.Append(request.OrderId);
			workLogString.Append(",RefundReason=");
			workLogString.Append(request.RefundReason);
			workLogString.Append(",Remark=");
			workLogString.Append(request.Remark);
			workLogString.Append("}");
			return workLogString.ToString();
		}
		string LogStRefundResponse(WbCPSRefundResponse response)
		{
			workLogString.Clear();
			workLogString.Append("{OrderId=");
			workLogString.Append(response.OrderId);
			workLogString.Append(",TransStatus=");
			workLogString.Append(response.TransStatus.ToString());
			workLogString.Append(",RefundAmount=");
			workLogString.Append(response.RefundAmount.ToString());
			workLogString.Append(",TransTime=");
			workLogString.Append(response.TransTime);
			workLogString.Append("}");
			return workLogString.ToString();
		}
		string LogStReverseRequest(WbCPSReverseRequest request)
		{
			workLogString.Clear();
			workLogString.Append("{OrderId=");
			workLogString.Append(request.OrderId);
			workLogString.Append("}");
			return workLogString.ToString();
		}
		string LogStReverseResponse(WbCPSReverseResponse response)
		{
			workLogString.Clear();
			workLogString.Append("{OrderId=");
			workLogString.Append(response.OrderId);
			workLogString.Append(",TransStatus=");
			workLogString.Append(response.TransStatus.ToString());
			workLogString.Append(",Amount=");
			workLogString.Append(response.Amount.ToString());
			workLogString.Append(",TransTime=");
			workLogString.Append(response.TransTime);
			workLogString.Append("}");
			return workLogString.ToString();
		}
		string LogStConfirmRequest(WbCPSConfirmRequest request)
		{
			workLogString.Clear();
			workLogString.Append("{OrderId=");
			workLogString.Append(request.OrderId);
			workLogString.Append("}");
			return workLogString.ToString();
		}
		string LogStConfirmResponse(WbCPSConfirmResponse response)
		{
			workLogString.Clear();
			workLogString.Append("{TransStatus=");
			workLogString.Append(response.TransStatus.ToString());
			workLogString.Append(",PayCheckDate=");
			workLogString.Append(response.PayCheckDate);
			workLogString.Append(",TransTime=");
			workLogString.Append(response.TransTime);
			workLogString.Append(",OrderId=");
			workLogString.Append(response.OrderId);
			workLogString.Append(",PayType=");
			workLogString.Append(response.PayType.ToString());
			workLogString.Append("}");
			return workLogString.ToString();
		}
		string LogStDepositRequest(WbCPSDepositRequest request)
		{
			workLogString.Clear();
			workLogString.Append("{PayType=");
			workLogString.Append(request.PayType.ToString());
			workLogString.Append(",ValueType=");
			workLogString.Append(request.ValueType.ToString());
			workLogString.Append(",Amount=");
			workLogString.Append(request.Amount.ToString());
			workLogString.Append(",ReceiptNo=");
			workLogString.Append(request.ReceiptNo);
			workLogString.Append(",UserCode=");
			workLogString.Append(request.UserCode);
			workLogString.Append("}");
			return workLogString.ToString();
		}
		string LogStDepositResponse(WbCPSDepositResponse response)
		{
			workLogString.Clear();
			workLogString.Append("{OrderId=");
			workLogString.Append(response.OrderId);
			workLogString.Append(",TransTime=");
			workLogString.Append(response.TransTime);
			workLogString.Append(",Amount=");
			workLogString.Append(response.Amount.ToString());
			workLogString.Append(",TransStatus=");
			workLogString.Append(response.TransStatus.ToString());
			workLogString.Append("}");
			return workLogString.ToString();
		}

		//void DebugWait(int sec)
		//{
		//	for(int i = 0;i < sec;i++){
		//		Console.WriteLine("■");
		//		Thread.Sleep(1000);
		//	}
		//}
	}
}
