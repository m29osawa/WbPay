using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WbPay
{
	public partial class WbCPSPay
	{
		int testClassPay		= 0;
		int testClassRefund		= 0;
		int testClassReverse	= 0;
		//int testClassConfirm	= 0;
		int testClassDeposit	= 0;

		int testSequenceDelay	= 1000;	//msec

		Dictionary<string,WbCPSPayResponse> testOrderHistory;

		void StartTestMode()
		{
			int	value;
			if(GetConfOptionalInt("TestSequenceDelay",out value)) testSequenceDelay = value * 1000;
			if(GetConfOptionalInt("TestClassPay",out value)) testClassPay = value;
			if(GetConfOptionalInt("TestClassRefund",out value)) testClassRefund = value;
			if(GetConfOptionalInt("TestClassReverse",out value)) testClassReverse = value;
			//if(GetConfOptionalInt("TestClassConfirm",out value)) testClassConfirm = value;
			if(GetConfOptionalInt("TestClassDeposit",out value)) testClassDeposit = value;

			testOrderHistory = new Dictionary<string, WbCPSPayResponse>();
			log?.Write(WbPayLogLevel.Basic,"WbCPSPay","Enter test mode.");
		}
		
		void PayTestSequence(WbCPSPayRequest request)
		{
			WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Pay,"Request*",LogStPayRequest(request));
			if(0 < testSequenceDelay) Thread.Sleep(testSequenceDelay);
			switch(testClassPay){
				case 1:
					TestPay1(request);
					break;
				case 2:
					TestPay2(request);
					break;
				case 3:
					TestPay3(request);
					break;
				case 4:
					TestPay4(request);
					break;
				case 5:
					TestPay5(request);
					break;
				case 6:
					TestPay6(request);
					break;
				case 7:
					TestPay7(request);
					break;
				default:
					TestPay0(request);
					break;
			}
		}
		void RefundTestSequence(WbCPSRefundRequest request)
		{
			WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Refund,"Request*",LogStRefundRequest(request));
			if(0 < testSequenceDelay) Thread.Sleep(testSequenceDelay);
			switch(testClassRefund){
				case 1:
					TestRefund1(request);
					break;
				default:
					TestRefund0(request);
					break;
			}
		}
		void ReverseTestSequence(WbCPSReverseRequest request)
		{
			WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Reverse,"Request*",LogStReverseRequest(request));
			if(0 < testSequenceDelay) Thread.Sleep(testSequenceDelay);
			switch(testClassReverse){
				default:
					TestReverse0(request);
					break;
			}
		}
		//void ConfirmTestSequence(WbCPSConfirmRequest request)
		//{
		//	WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Confirm,"Request*",LogStConfirmRequest(request));
		//	if(0 < testSequenceDelay) Thread.Sleep(testSequenceDelay);
		//	switch(testClassConfirm){
		//		default:
		//			TestConfirm0(request);
		//			break;
		//	}
		//}
		void DepositTestSequence(WbCPSDepositRequest request)
		{
			WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Deposit,"Request*",LogStDepositRequest(request));
			if(0 < testSequenceDelay) Thread.Sleep(testSequenceDelay);
			switch(testClassDeposit){
				default:
					TestDeposit0(request);
					break;
			}
		}

		void TestPay0(WbCPSPayRequest request)
		{
			WbCPSPayResponse response = new WbCPSPayResponse();
			response.OrderId = "TESTORDER-" + DateTime.Now.ToString("MMddHHmmss");
			response.TransTime = DateTime.Now.ToString("yyyyMMddHHmmss");
			response.Amount = request.Amount;
			response.TransStatus = 0;
			if(request.PayType == WbCPSPayType.AutoDetect){
				response.PayType = WbCPSPayType.LinePay;
			}else{
				response.PayType = request.PayType;
			}
			testOrderHistory[response.OrderId] = response;
			WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Pay,"Response*",LogStPayResponse(response));
			InvokeCompleteEvent(WbCPSRequestType.Pay,response);
		}
		void TestPay1(WbCPSPayRequest request)
		{
			WbCPSPayResponse response = new WbCPSPayResponse();
			response.OrderId = "TESTORDER-" + DateTime.Now.ToString("MMddHHmmss");
			response.TransTime = DateTime.Now.ToString("yyyyMMddHHmmss");
			response.Amount = request.Amount;
			response.TransStatus = 14;
			if(request.PayType == WbCPSPayType.AutoDetect){
				response.PayType = WbCPSPayType.LinePay;
			}else{
				response.PayType = request.PayType;
			}
			lock(resourceLock){
				payConfirmInProgress = true;
				payConfirmAbort = false;
				waitPayConfirm.Reset();
			}
			WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Pay,"Response*",LogStPayResponse(response));
			InvokeCompleteEvent(WbCPSRequestType.Pay,response);
			
			try{
				while(true){
					if(payConfirmPollingMode){
						if(0 < payConfirmPollingWait) Thread.Sleep(payConfirmPollingWait);
					}else{
						if(!waitPayConfirm.WaitOne(DateTime.Now.AddMilliseconds(PAYCONFIRM_TIMEOUT) - DateTime.Now)) break;
					}
					lock(resourceLock){
						if(payConfirmAbort) break;
					}
					response.TransTime = DateTime.Now.ToString("yyyyMMddHHmmss");
					response.TransStatus = 0;
					testOrderHistory[response.OrderId] = response;
					WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Pay,"Response*",LogStPayResponse(response));
					InvokeCompleteEvent(WbCPSRequestType.Pay,response);
					return;
				}
			
				WbCPSResponseError error =  new WbCPSResponseError();
				error.Type = WbCPSError.POST_PROCESS;
				error.Code = "E06001";
				error.SubCode = "";
				error.Message = "支払失敗";
				error.RequestAction = WbCPSAction.VendorCall;
				WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Pay,"Response*",LogStResponseError(error));
				InvokeErrorEvent(WbCPSRequestType.Pay,error);
			}finally{
				lock(resourceLock){
					payConfirmInProgress = false;
				}
			}
		}
		void TestPay2(WbCPSPayRequest request)
		{
			WbCPSResponseError error =  new WbCPSResponseError();
			error.Type = WbCPSError.LIB;
			error.Code = "E05201";
			error.SubCode = "";
			error.Message = "リモート サーバーに接続できません。";
			error.RequestAction = WbCPSAction.VendorCall;
			WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Pay,"Response*",LogStResponseError(error));
			InvokeErrorEvent(WbCPSRequestType.Pay,error);
		}
		void TestPay3(WbCPSPayRequest request)
		{
			WbCPSResponseError error =  new WbCPSResponseError();
			error.Type = WbCPSError.CPS;
			error.Code = "E09113";
			error.SubCode = "";
			error.Message = "リクエストパラメータ異常、ユーザコードは有効ではありません。";
			error.RequestAction = WbCPSAction.CPSCall;
			WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Pay,"Response*",LogStResponseError(error));
			InvokeErrorEvent(WbCPSRequestType.Pay,error);
		}
		void TestPay4(WbCPSPayRequest request)
		{
			WbCPSResponseError error =  new WbCPSResponseError();
			error.Type = WbCPSError.CPS;
			error.Code = "E09301";
			error.SubCode = "NOT_ENOUGH";
			error.Message = "外部システム異常が発生しました。";
			error.RequestAction = WbCPSAction.Customer;
			WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Pay,"Response*",LogStResponseError(error));
			InvokeErrorEvent(WbCPSRequestType.Pay,error);
		}
		void TestPay5(WbCPSPayRequest request)
		{
			WbCPSResponseError error =  new WbCPSResponseError();
			error.Type = WbCPSError.CPS;
			error.Code = "E09203";
			error.SubCode = "";
			error.Message = "サービス停止中";
			error.RequestAction = WbCPSAction.CPSCall;
			WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Pay,"Response*",LogStResponseError(error));
			InvokeErrorEvent(WbCPSRequestType.Pay,error);
		}
		void TestPay6(WbCPSPayRequest request)
		{
			WbCPSPayResponse response = new WbCPSPayResponse();
			response.OrderId = "TESTORDER-" + DateTime.Now.ToString("MMddHHmmss");
			response.TransTime = DateTime.Now.ToString("yyyyMMddHHmmss");
			response.Amount = request.Amount;
			response.TransStatus = 14;
			if(request.PayType == WbCPSPayType.AutoDetect){
				response.PayType = WbCPSPayType.LinePay;
			}else{
				response.PayType = request.PayType;
			}
			lock(resourceLock){
				payConfirmInProgress = true;
				payConfirmAbort = false;
				waitPayConfirm.Reset();
			}
			WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Pay,"Response*",LogStPayResponse(response));
			InvokeCompleteEvent(WbCPSRequestType.Pay,response);

			try{
				WbCPSResponseError error = new WbCPSResponseError();
				while(true){
					if(payConfirmPollingMode){
						if(0 < payConfirmPollingWait) Thread.Sleep(payConfirmPollingWait);
					}else{
						if(!waitPayConfirm.WaitOne(DateTime.Now.AddMilliseconds(PAYCONFIRM_TIMEOUT) - DateTime.Now)) break;
					}
					lock(resourceLock){
						if(payConfirmAbort) break;
					}
					error.Type = WbCPSError.POST_PROCESS;
					error.Code = "E06015";
					error.SubCode = "";
					error.Message = "顧客支払中止";
					error.RequestAction = WbCPSAction.CPSCall;
					WriteLog(WbPayLogLevel.Sequence, WbCPSRequestType.Pay, "Response*", LogStResponseError(error));
					InvokeErrorEvent(WbCPSRequestType.Pay,error);
					return;
				}
				error.Type = WbCPSError.POST_PROCESS;
				error.Code = "E06001";
				error.SubCode = "";
				error.Message = "支払失敗";
				error.RequestAction = WbCPSAction.VendorCall;
				WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Pay,"Response*",LogStResponseError(error));
				InvokeErrorEvent(WbCPSRequestType.Pay,error);
			}finally{
				lock(resourceLock){
					payConfirmInProgress = false;
				}
			}
		}
		void TestPay7(WbCPSPayRequest request)
		{
			WbCPSPayResponse response = new WbCPSPayResponse();
			response.OrderId = "TESTORDER-" + DateTime.Now.ToString("MMddHHmmss");
			response.TransTime = DateTime.Now.ToString("yyyyMMddHHmmss");
			response.Amount = request.Amount;
			response.TransStatus = 14;
			if(request.PayType == WbCPSPayType.AutoDetect){
				response.PayType = WbCPSPayType.LinePay;
			}else{
				response.PayType = request.PayType;
			}
			lock(resourceLock){
				payConfirmInProgress = true;
				payConfirmAbort = false;
				waitPayConfirm.Reset();
			}
			WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Pay,"Response*",LogStPayResponse(response));
			InvokeCompleteEvent(WbCPSRequestType.Pay,response);

			try{
				WbCPSResponseError error = new WbCPSResponseError();
				while(true){
					if(payConfirmPollingMode){
						if(0 < payConfirmPollingWait) Thread.Sleep(payConfirmPollingWait);
					}else{
						if(!waitPayConfirm.WaitOne(DateTime.Now.AddMilliseconds(PAYCONFIRM_TIMEOUT) - DateTime.Now)) break;
					}
					lock(resourceLock){
						if(payConfirmAbort) break;
					}
					error.Type = WbCPSError.POST_PROCESS;
					error.Code = "E06004";
					error.SubCode = "";
					error.Message = "支払タイムアウト";
					error.RequestAction = WbCPSAction.CPSCall;
					WriteLog(WbPayLogLevel.Sequence, WbCPSRequestType.Pay, "Response*", LogStResponseError(error));
					InvokeErrorEvent(WbCPSRequestType.Pay,error);
					return;
				}
				error.Type = WbCPSError.POST_PROCESS;
				error.Code = "E06001";
				error.SubCode = "";
				error.Message = "支払失敗";
				error.RequestAction = WbCPSAction.VendorCall;
				WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Pay,"Response*",LogStResponseError(error));
				InvokeErrorEvent(WbCPSRequestType.Pay,error);
			}finally{
				lock(resourceLock){
					payConfirmInProgress = false;
				}
			}
		}
		void TestRefund0(WbCPSRefundRequest request)
		{
			WbCPSPayResponse pay_response;
			WbCPSRefundResponse response = new WbCPSRefundResponse();
			WbCPSResponseError error = new WbCPSResponseError();
			if(testOrderHistory.TryGetValue(request.OrderId,out pay_response)){
				if(pay_response.TransStatus == 0 || pay_response.TransStatus == 6){
					if(request.RefundAmount <= pay_response.Amount){
						response.OrderId = pay_response.OrderId;
						response.TransStatus = 6;
						response.RefundAmount = request.RefundAmount;
						response.TransTime = DateTime.Now.ToString("yyyyMMddHHmmss");
						pay_response.TransStatus = 6;
						pay_response.Amount = pay_response.Amount - request.RefundAmount;
						pay_response.TransTime = response.TransTime;
						WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Refund,"Response*",LogStRefundResponse(response));
						InvokeCompleteEvent(WbCPSRequestType.Refund,response);
						return;
					}else{
						error.Type = WbCPSError.CPS;
						error.Code = "E09128";
						error.SubCode = "";
						error.Message = "返金金額が不正です。";
						error.RequestAction = WbCPSAction.CPSCall;
					}
				}else{
					error.Type = WbCPSError.CPS;
					error.Code = "E09117";
					error.SubCode = "";
					error.Message = "支払が未成功のため、当該オーダが返金できません。";
					error.RequestAction = WbCPSAction.CPSCall;
				}
			}else{
				error.Type = WbCPSError.CPS;
				error.Code = "E09103";
				error.SubCode = "";
				error.Message = "オーダが存在しません。";
				error.RequestAction = WbCPSAction.CPSCall;
			}
			WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Refund,"Response*",LogStResponseError(error));
			InvokeErrorEvent(WbCPSRequestType.Refund,error);
		}
		void TestRefund1(WbCPSRefundRequest request)
		{
			WbCPSResponseError error =  new WbCPSResponseError();
			error.Type = WbCPSError.CPS_NETWORK;
			error.Code = "E02202";
			error.SubCode = "";
			error.Message = "外部システムとの接続にオーバータイムが発生しました。";
			error.RequestAction = WbCPSAction.CPSCall;
			WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Refund,"Response*",LogStResponseError(error));
			InvokeErrorEvent(WbCPSRequestType.Refund,error);
		}
		void TestReverse0(WbCPSReverseRequest request)
		{
			WbCPSPayResponse pay_response;
			WbCPSReverseResponse response = new WbCPSReverseResponse();
			WbCPSResponseError error = new WbCPSResponseError();
			if(testOrderHistory.TryGetValue(request.OrderId,out pay_response)){
				if(pay_response.TransStatus == 0
					|| pay_response.TransStatus == 18){
					response.OrderId = pay_response.OrderId;
					response.TransStatus = 10;
					response.Amount = pay_response.Amount;
					response.TransTime = DateTime.Now.ToString("yyyyMMddHHmmss");
					pay_response.TransStatus = 10;
					pay_response.TransTime = response.TransTime;
					WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Reverse,"Response*",LogStReverseResponse(response));
					InvokeCompleteEvent(WbCPSRequestType.Reverse,response);
					return;
				}else{
					error.Type = WbCPSError.CPS;
					error.Code = "E09135";
					error.SubCode = "";
					error.Message = "取消トランザクションが返金できません。";
					error.RequestAction = WbCPSAction.CPSCall;
				}
			}else{
				error.Type = WbCPSError.CPS;
				error.Code = "E09103";
				error.SubCode = "";
				error.Message = "オーダが存在しません。";
				error.RequestAction = WbCPSAction.CPSCall;
			}
			WriteLog(WbPayLogLevel.Error,WbCPSRequestType.Reverse,"Response*",LogStResponseError(error));
			InvokeErrorEvent(WbCPSRequestType.Reverse,error);
		}
		//void TestConfirm0(WbCPSConfirmRequest request)
		//{
		//	WbCPSPayResponse pay_response;
		//	WbCPSConfirmResponse response = new WbCPSConfirmResponse();
		//	WbCPSResponseError error;
		//	if(testOrderHistory.TryGetValue(request.OrderId,out pay_response)){
		//		response.TransStatus = pay_response.TransStatus;
		//		response.PayCheckDate = DateTime.Now.ToString("yyyyMMdd");
		//		response.TransTime = pay_response.TransTime;
		//		response.OrderId = pay_response.OrderId;
		//		response.PayType = pay_response.PayType;
		//		WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Confirm,"Response*",LogStConfirmResponse(response));
		//		InvokeCompleteEvent(WbCPSRequestType.Confirm,response);
		//		return;
		//	}
		//	error =  new WbCPSResponseError();
		//	error.Type = WbCPSError.CPS;
		//	error.Code = "E09103";
		//	error.SubCode = "";
		//	error.Message = "オーダが存在しません。";
		//	error.RequestAction = WbCPSAction.CPSCall;
		//	WriteLog(WbPayLogLevel.Error,WbCPSRequestType.PayConfirm,"Response*",LogStResponseError(error));
		//	InvokeErrorEvent(WbCPSRequestType.PayConfirm,error);
		//}
		void TestDeposit0(WbCPSDepositRequest request)
		{
			WbCPSDepositResponse response = new WbCPSDepositResponse();
			response.OrderId = "TESTORDER-" + DateTime.Now.ToString("MMddHHmmss");
			response.TransTime = DateTime.Now.ToString("yyyyMMddHHmmss");
			response.Amount = request.Amount;
			response.TransStatus = 18;
			WbCPSPayResponse pay_response = new WbCPSPayResponse();
			pay_response.OrderId = response.OrderId;
			pay_response.TransTime = response.TransTime;
			pay_response.Amount = response.Amount;
			pay_response.TransStatus = response.TransStatus;
			pay_response.PayType = request.PayType;
			testOrderHistory[response.OrderId] = pay_response;
			WriteLog(WbPayLogLevel.Sequence,WbCPSRequestType.Deposit,"Response*",LogStDepositResponse(response));
			InvokeCompleteEvent(WbCPSRequestType.Deposit,response);
		}
	}
}
