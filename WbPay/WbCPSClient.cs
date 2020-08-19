using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net.Http;

using System.Collections;

namespace WbPay
{
	class WbCPSClient
	{
		static string serverURL;
		static string apiPathPayV1;
		static string apiPathRefund;
		static string apiPathReverse;
		static string apiPathConfirmV1;
		static string apiPathDeposit;
		
		const string serviceMode		= "B01";	// 固定
		const string signType			= "0";		// 固定
		const string isBackTran			= "0";		// 固定
		static readonly string urlPart	= serviceMode + "/" + signType + "/" + isBackTran;
		static string tokenPart			= "";
	
		static string merchantId;
		static string terminalUniquId;
		
		static string locale;
		static string timeZone;
		static string branchCode;
		static string terminalCode;
		static string currencyCode;
		static string appVersion;
		
		static string privateKeyFile;
		static string publicKeyFile;
		static int httpTimeout				= 0;	// sec
		static bool ignoreCertificateError	= false;
	
		static WbCPSCurrencyType CurrencyType {
			get { return WbCPSRawCode.GetCurrencyType(currencyCode); }
		}
		static WbPayLog log;
		internal static WbPayLog Log {
			set {
				log = value;
				WbCPSHttpClient.Log = value;
			}
		}
		static WbPayConf conf;
		internal static WbPayConf Conf {
			set { conf = value; }
		}

		static bool openFlag = false;

		private WbCPSClient() { }

		static internal void Open(string home_dir,string app_name,string app_version)
		{
			if(openFlag){
				log?.Write(WbPayLogLevel.Error,"WbCPSClient","WbCPSClient is already opened.");
				throw new WbPayException(	WbCPSError.LIB,
											WbCPSError.Lib.PROGRAM,
											WbCPSError.Lib.Program.PROGRAM_ERROR,
											"WbCPSClient.Open:WbCPSClientがすでにオープンされています。");
			}
			SetParameter();
			WbCPSSignature.LoadKey(home_dir + @"\" + privateKeyFile,home_dir + @"\" + publicKeyFile);
			WbCPSSerialNo.Start(home_dir,app_name);
			if(0 < httpTimeout) WbCPSHttpClient.Timeout = httpTimeout;
			if(ignoreCertificateError) WbCPSHttpClient.IgnoreCertificateError = ignoreCertificateError;
			WbCPSHttpClient.SetUserAget(app_name,app_version);
			tokenPart = merchantId + terminalUniquId;
			openFlag = true;
		}
		static internal void Close()
		{
			if(!openFlag){
				log?.Write(WbPayLogLevel.Error,"WbCPSClient","WbCPSClient is not opened.");
				throw new WbPayException(	WbCPSError.LIB,
											WbCPSError.Lib.PROGRAM,
											WbCPSError.Lib.Program.PROGRAM_ERROR,
											"WbCPSClient.Open:WbCPSClientがオープンされていません。");
			}
			WbCPSSerialNo.Close();				
			openFlag = false;
		}

		static void SetParameter()
		{
			if(conf == null){
				log?.Write(WbPayLogLevel.Error,"WbCPSClient","Configuration is not sotored.");
				throw new WbPayException(	WbCPSError.LIB,
											WbCPSError.Lib.PROGRAM,
											WbCPSError.Lib.Program.PROGRAM_ERROR,
											"コンフィギュレーションが読み込まれていません。");
			}

			serverURL = GetConfValue("ServerURL",0,0);
			apiPathPayV1 = GetConfValue("ApiPathPayV1",0,0);
			apiPathRefund = GetConfValue("ApiPathRefund",0,0);
			apiPathReverse = GetConfValue("ApiPathReverse",0,0);
			apiPathConfirmV1 = GetConfValue("ApiPathConfirmV1",0,0);
			apiPathDeposit = GetConfValue("ApiPathDeposit",0,0);

			merchantId = GetConfValue("MerchantId",12,12);
			terminalUniquId = GetConfValue("TerminalUniquId",13,13);

			locale = GetConfValue("Locale",0,30);
			timeZone = GetConfValue("TimeZone",0,4);
			branchCode = GetConfValue("BranchCode",0,32);
			terminalCode = GetConfValue("TerminalCode",0,32);
			currencyCode = GetConfValue("CurrencyCode",0,10);
			appVersion = GetConfValue("AppVersion",0,32);

			privateKeyFile = GetConfValue("PrivateKeyFile",0,0);
			publicKeyFile = GetConfValue("PublicKeyFile",0,0);

			string st = null;
			st = conf.Get("HttpTimeout");
			if(!string.IsNullOrEmpty(st)){	// HttpTimeoutは設定は任意
				int	value;
				if(!Int32.TryParse(st,out value)){
					log?.Write(WbPayLogLevel.Error,"WbCPSClient","Illegal configuration value : HttpTimeout=" + value);
					throw new WbPayException(	WbCPSError.LIB,
												WbCPSError.Lib.PARAMETER,
												WbCPSError.Lib.Parameter.ILLEGAL_CONFIG_VALUE,
												"コンフィギュレーション HttpTimeout=" + st + " の値が不正です。");
				}
				httpTimeout = value;
			}
			st = conf.Get("IgnoreCertificateError");
			if(!String.IsNullOrEmpty(st)){
				if(st.Equals("yes",StringComparison.OrdinalIgnoreCase)){
					ignoreCertificateError = true;
				}else if(st.Equals("no",StringComparison.OrdinalIgnoreCase)){
					;
				}else{
					log?.Write(WbPayLogLevel.Error,"WbCPSClient","Illegal configuration value : IgnoreCertificateError=" + st);
					throw new WbPayException(	WbCPSError.LIB,
												WbCPSError.Lib.PARAMETER,
												WbCPSError.Lib.Parameter.ILLEGAL_CONFIG_VALUE,
												"コンフィギュレーション IgnoreCertificateError=" + st + " の値が不正です。");
				}
			}
		}
		static string GetConfValue(string key,int minlen,int maxlen)
		{
			string value = conf.Get(key);
			if(String.IsNullOrEmpty(value)){
				log?.Write(WbPayLogLevel.Error,"WbCPSClient","Configuration key not found. : " + key);
				throw new WbPayException(	WbCPSError.LIB,
											WbCPSError.Lib.PARAMETER,
											WbCPSError.Lib.Parameter.CONFIG_NOT_FOUND,
											"コンフィギュレーション" + key + "がありません。");
			}
			if(minlen > 0 && value.Length < minlen){
				log?.Write(WbPayLogLevel.Error,"WbCPSClient","Illegal configuration value : " + key + "=" + value);
				throw new WbPayException(	WbCPSError.LIB,
											WbCPSError.Lib.PARAMETER,
											WbCPSError.Lib.Parameter.ILLEGAL_CONFIG_VALUE,
											"コンフィギュレーション " + key + "=" + value + " の値が不正です。");
			}
			if(maxlen > 0 && value.Length > maxlen){
				log?.Write(WbPayLogLevel.Error,"WbCPSClient","Illegal configuration value : " + key + "=" + value);
				throw new WbPayException(	WbCPSError.LIB,
											WbCPSError.Lib.PARAMETER,
											WbCPSError.Lib.Parameter.ILLEGAL_CONFIG_VALUE,
											"コンフィギュレーション " + key + "=" + value + " の値が不正です。");
			}
			return value;
		}
		
		static StringBuilder workCreateToken = new StringBuilder();
		static string CreateToken(){
			workCreateToken.Clear();
			workCreateToken.Append(tokenPart);
			workCreateToken.Append(DateTime.Now.ToString("yyyyMMddHHmmss"));
			workCreateToken.Append(String.Format("{0:D15}",WbCPSSerialNo.Next()));
			return workCreateToken.ToString();
		}
		static StringBuilder workCreateUrl = new StringBuilder();
		static string CreateUrl(string api,string token){
			workCreateUrl.Clear();
			workCreateUrl.Append(serverURL);
			workCreateUrl.Append("/");
			workCreateUrl.Append(api);
			workCreateUrl.Append("/");
			workCreateUrl.Append(urlPart);
			workCreateUrl.Append("/");
			workCreateUrl.Append(token);
			return workCreateUrl.ToString();
		}

		internal static WbCPSPayResult PayRequest(WbCPSPayRequest request)
		{
			WbCPSPayResult result = new WbCPSPayResult();
			WbCPSErrorResult error = new WbCPSErrorResult();
			error.Type = WbCPSError.LIB;
			result.IsSuccess = false;
			result.Error = error;

			if(!openFlag){
				error.SubType = WbCPSError.Lib.PROGRAM;
				error.Code = WbCPSError.Lib.Program.PROGRAM_ERROR;
				error.Message = "プログラムエラー:WbCPSClientがオープンされていません。";
				return result;
			}
						
			WbCPSRawPayRequest raw_request = new WbCPSRawPayRequest();
			raw_request.locale = locale;
			raw_request.timeZone = timeZone;
			raw_request.payType = WbCPSRawCode.GetPayCode(request.PayType);
			raw_request.branchCode = branchCode;
			raw_request.terminalCode = terminalCode;
			raw_request.currencyCode = currencyCode;
			if(CurrencyType == WbCPSCurrencyType.JPY){
				raw_request.amount = request.Amount.ToString("F0");
			}else{
				raw_request.amount = request.Amount.ToString("F2");
			}
			raw_request.receiptNo = request.ReceiptNo;
			raw_request.userCode = request.UserCode;
			raw_request.appVersion = appVersion;
			raw_request.sign = null;
			if(request.Remark == null){
				//raw_request.remark = "";		// OCX仕様対応
			}else{
				raw_request.remark = request.Remark;
			}
			//raw_request.extendInfo = "";		// OCX仕様対応

			try{
				raw_request.sign = WbCPSSignature.CreateSignature(raw_request);
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_SIGNATURE;
				error.Message = "署名作成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			string content;
			try{
				content = CreateJsonContent(typeof(WbCPSRawPayRequest),raw_request);
			}catch(Exception ex){			
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_CONTENT;
				error.Message = "リクエストメッセージ構成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}
			
			string url;
			try{
				string token = CreateToken();
				result.Token = token;
				url = CreateUrl(apiPathPayV1,token);
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_URL;
				error.Message = "リクエスURL生成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			Task<string> t = Task.Run(() => WbCPSHttpClient.Post(url,content));
			try{
				t.Wait();
			}catch(AggregateException ae){
				error.SubType = WbCPSError.Lib.PROGRAM;
				error.Code = WbCPSError.Lib.Program.PROGRAM_ERROR;
				error.Message = ae.Message;

				foreach(var ex in ae.InnerExceptions){
					if(ex is WbCPSHttpException){
						error.SubType = WbCPSError.Lib.HTTP;
						error.Code = WbCPSError.Lib.Http.POST;
						error.Message = ex.Message;
						return result;
					}else{
						error.Message = ex.Message;
					}
				}
				return result;
			}

			WbCPSRawPayResponseV1 raw_response;
			try{
				raw_response = ReadJsonContent(typeof(WbCPSRawPayResponseV1),t.Result) as WbCPSRawPayResponseV1;
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.PARSE_CONTENT;
				error.Message = "レスポンスメッセージ解析エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			string sign = raw_response.data.sign;
			raw_response.data.sign = null;

			bool sign_verify_ret;
			try{
				sign_verify_ret =WbCPSSignature.VerifySignature(raw_response,sign);
			}catch(Exception ex){	
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.PARSE_SIGNATURE;
				error.Message = "署名検証エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}
			if(!sign_verify_ret){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.VERIFY_SIGNATURE;
				error.Message = "レスポンスの署名が正しくありません。";
				return result;
			}

			WbCPSPayResponse response = new WbCPSPayResponse();
			string error_mess = "";
			try{
				error_mess = "meta";
				if(raw_response.meta.code == null || Int32.Parse(raw_response.meta.code) != 0){
					SetErrorInfo(error,
							raw_response.data.errorCode,raw_response.data.subErrorCode,raw_response.data.errorInfo);
					return result;
				}
				error_mess = "orderId";
				response.OrderId = raw_response.data.result.orderId;
				error_mess = "transTime";
				response.TransTime = raw_response.data.result.transTime;
				if(raw_response.data.result.amount == null && raw_response.data.result.amountRmb == null){
					error_mess = "amount と amountRmb";
					throw new Exception("どちらかの値が必要です。");
				}
				if(raw_response.data.result.amount != null){
					error_mess = "amount";
					response.Amount = Decimal.Parse(raw_response.data.result.amount,
													System.Globalization.NumberStyles.Number);
				}
				error_mess = "transStatus";
				response.TransStatus = Int32.Parse(raw_response.data.result.transStatus);
				error_mess = "payType";
				response.PayType = WbCPSRawCode.GetPayType(raw_response.data.result.payType);
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.ILLEGAL_PARAMETER;
				error.Message = "レスポンスパラメータ異常:"+ error_mess + "が有効ではありません。:"
										+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			result.IsSuccess = true;
			result.Error = null;
			result.Response = response;

			return result;
		}
		internal static WbCPSRefundResult RefundRequest(WbCPSRefundRequest request)
		{
			WbCPSRefundResult result = new WbCPSRefundResult();
			WbCPSErrorResult error = new WbCPSErrorResult();
			error.Type = WbCPSError.LIB;
			result.IsSuccess = false;
			result.Error = error;

			if(!openFlag){
				error.SubType = WbCPSError.Lib.PROGRAM;
				error.Code = WbCPSError.Lib.Program.PROGRAM_ERROR;
				error.Message = "プログラムエラー:WbCPSClientがオープンされていません。";
				return result;
			}
						
			WbCPSRawRefundRequest raw_request = new WbCPSRawRefundRequest();
			raw_request.locale = locale;
			raw_request.timeZone = timeZone;
			raw_request.branchCode = branchCode;
			raw_request.terminalCode = terminalCode;
			if(CurrencyType == WbCPSCurrencyType.JPY){
				raw_request.refundAmount = request.RefundAmount.ToString("F0");
			}else{
				raw_request.refundAmount = request.RefundAmount.ToString("F2");
			}
			raw_request.orderId = request.OrderId;
			//raw_request.orderDetailId = "";		// OCX仕様対応
			raw_request.currencyCode = currencyCode;
			if(request.RefundReason == null){
				//raw_request.refundReason = "";	// OCX仕様対応
			}else{
				raw_request.refundReason = request.RefundReason;
			}
			if(request.Remark == null){
				//raw_request.remark = "";		// OCX仕様対応
			}else{
				raw_request.remark = request.Remark;
			}
			raw_request.appVersion = appVersion;
			raw_request.sign = null;
			//raw_request.qryToken = "";		// OCX仕様対応

			try{
				raw_request.sign = WbCPSSignature.CreateSignature(raw_request);
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_SIGNATURE;
				error.Message = "署名作成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			string content;
			try{
				content = CreateJsonContent(typeof(WbCPSRawRefundRequest),raw_request);
			}catch(Exception ex){			
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_CONTENT;
				error.Message = "リクエストメッセージ構成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}
			
			string url;
			try{
				string token = CreateToken();
				result.Token = token;
				url = CreateUrl(apiPathRefund,token);
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_URL;
				error.Message = "リクエスURL生成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			Task<string> t = Task.Run(() => WbCPSHttpClient.Post(url,content));
			try{
				t.Wait();
			}catch(AggregateException ae){
				error.SubType = WbCPSError.Lib.PROGRAM;
				error.Code = WbCPSError.Lib.Program.PROGRAM_ERROR;
				error.Message = ae.Message;

				foreach(var ex in ae.InnerExceptions){
					if(ex is WbCPSHttpException){
						error.SubType = WbCPSError.Lib.HTTP;
						error.Code = WbCPSError.Lib.Http.POST;
						error.Message = ex.Message;
						return result;
					}else{
						error.Message = ex.Message;
					}
				}
				return result;
			}

			WbCPSRawRefundResponse raw_response;
			try{
				raw_response = ReadJsonContent(typeof(WbCPSRawRefundResponse),t.Result) as WbCPSRawRefundResponse;
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.PARSE_CONTENT;
				error.Message = "レスポンスメッセージ解析エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			string sign = raw_response.data.sign;
			raw_response.data.sign = null;

			bool sign_verify_ret;
			try{
				sign_verify_ret =WbCPSSignature.VerifySignature(raw_response,sign);
			}catch(Exception ex){	
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.PARSE_SIGNATURE;
				error.Message = "署名検証エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}
			if(!sign_verify_ret){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.VERIFY_SIGNATURE;
				error.Message = "レスポンスの署名が正しくありません。";
				return result;
			}

			WbCPSRefundResponse response = new WbCPSRefundResponse();
			string error_mess = "";
			try{
				error_mess = "meta";
				if(raw_response.meta.code == null || Int32.Parse(raw_response.meta.code) != 0){
					SetErrorInfo(error,
							raw_response.data.errorCode,raw_response.data.subErrorCode,raw_response.data.errorInfo);
					return result;
				}
				error_mess = "orderId";
				response.OrderId = raw_response.data.result.orderId;
				error_mess = "transStatus";
				response.TransStatus = Int32.Parse(raw_response.data.result.transStatus);
				error_mess = "refundAmount";
				response.RefundAmount = Decimal.Parse(raw_response.data.result.refundAmount,
																System.Globalization.NumberStyles.Number);
				error_mess = "transTime";
				response.TransTime = raw_response.data.result.transTime;
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.ILLEGAL_PARAMETER;
				error.Message = "レスポンスパラメータ異常:"+ error_mess + "が有効ではありません。:"
										+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			result.IsSuccess = true;
			result.Error = null;
			result.Response = response;

			return result;
		}
		internal static WbCPSReverseResult ReverseRequest(WbCPSReverseRequest request,string pre_token)
		{
			WbCPSReverseResult result = new WbCPSReverseResult();
			WbCPSErrorResult error = new WbCPSErrorResult();
			error.Type = WbCPSError.LIB;
			result.IsSuccess = false;
			result.Error = error;

			if(!openFlag){
				error.SubType = WbCPSError.Lib.PROGRAM;
				error.Code = WbCPSError.Lib.Program.PROGRAM_ERROR;
				error.Message = "プログラムエラー:WbCPSClientがオープンされていません。";
				return result;
			}
						
			WbCPSRawReverseRequest raw_request = new WbCPSRawReverseRequest();
			raw_request.locale = locale;
			raw_request.timeZone = timeZone;
			raw_request.branchCode = branchCode;
			raw_request.terminalCode = terminalCode;
			raw_request.orderId = request.OrderId;
			raw_request.sign = null;
			raw_request.appVersion = appVersion;
			if(pre_token == null){
				//raw_request.qryToken = "";	// OCX仕様対応
			}else{
				raw_request.qryToken = pre_token;
			}

			try{
				raw_request.sign = WbCPSSignature.CreateSignature(raw_request);
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_SIGNATURE;
				error.Message = "署名作成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			string content;
			try{
				content = CreateJsonContent(typeof(WbCPSRawReverseRequest),raw_request);
			}catch(Exception ex){			
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_CONTENT;
				error.Message = "リクエストメッセージ構成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}
			
			string url;
			try{
				string token = CreateToken();
				result.Token = token;
				url = CreateUrl(apiPathReverse,token);
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_URL;
				error.Message = "リクエスURL生成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			Task<string> t = Task.Run(() => WbCPSHttpClient.Post(url,content));
			try{
				t.Wait();
			}catch(AggregateException ae){
				error.SubType = WbCPSError.Lib.PROGRAM;
				error.Code = WbCPSError.Lib.Program.PROGRAM_ERROR;
				error.Message = ae.Message;

				foreach(var ex in ae.InnerExceptions){
					if(ex is WbCPSHttpException){
						error.SubType = WbCPSError.Lib.HTTP;
						error.Code = WbCPSError.Lib.Http.POST;
						error.Message = ex.Message;
						return result;
					}else{
						error.Message = ex.Message;
					}
				}
				return result;
			}

			WbCPSRawReverseResponse raw_response;
			try{
				raw_response = ReadJsonContent(typeof(WbCPSRawReverseResponse),t.Result) as WbCPSRawReverseResponse;
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.PARSE_CONTENT;
				error.Message = "レスポンスメッセージ解析エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			string sign = raw_response.data.sign;
			raw_response.data.sign = null;

			bool sign_verify_ret;
			try{
				sign_verify_ret =WbCPSSignature.VerifySignature(raw_response,sign);
			}catch(Exception ex){	
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.PARSE_SIGNATURE;
				error.Message = "署名検証エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}
			if(!sign_verify_ret){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.VERIFY_SIGNATURE;
				error.Message = "レスポンスの署名が正しくありません。";
				return result;
			}

			WbCPSReverseResponse response = new WbCPSReverseResponse();
			string error_mess = "";
			try{
				error_mess = "meta";
				if(raw_response.meta.code == null || Int32.Parse(raw_response.meta.code) != 0){
					SetErrorInfo(error,
							raw_response.data.errorCode,raw_response.data.subErrorCode,raw_response.data.errorInfo);
					return result;
				}
				error_mess = "orderId";
				response.OrderId = raw_response.data.result.orderId;
				error_mess = "transStatus";
				response.TransStatus = Int32.Parse(raw_response.data.result.transStatus);
				error_mess = "amount";
				if(String.IsNullOrEmpty(raw_response.data.result.amount)){
					response.Amount = 0M;
				}else{
					response.Amount = Decimal.Parse(raw_response.data.result.amount,
																System.Globalization.NumberStyles.Number);
				}
				error_mess = "transTime";
				response.TransTime = raw_response.data.result.transTime;
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.ILLEGAL_PARAMETER;
				error.Message = "レスポンスパラメータ異常:"+ error_mess + "が有効ではありません。:"
										+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			result.IsSuccess = true;
			result.Error = null;
			result.Response = response;

			return result;
		}
		internal static WbCPSConfirmResult ConfirmRequest(WbCPSConfirmRequest request,string pre_token)
		{
			WbCPSConfirmResult result = new WbCPSConfirmResult();
			WbCPSErrorResult error = new WbCPSErrorResult();
			error.Type = WbCPSError.LIB;
			result.IsSuccess = false;
			result.Error = error;

			if(!openFlag){
				error.SubType = WbCPSError.Lib.PROGRAM;
				error.Code = WbCPSError.Lib.Program.PROGRAM_ERROR;
				error.Message = "プログラムエラー:WbCPSClientがオープンされていません。";
				return result;
			}
						
			WbCPSRawConfirmRequest raw_request = new WbCPSRawConfirmRequest();
			raw_request.locale = locale;
			raw_request.timeZone = timeZone;
			raw_request.branchCode = branchCode;
			raw_request.terminalCode = terminalCode;
			raw_request.orderId = request.OrderId;
			//raw_request.orderDetailId = "";     // OCX仕様対応
			raw_request.sign = null;
			if(pre_token == null){
				//raw_request.qryToken = "";      // OCX仕様対応
			} else{
				raw_request.qryToken = pre_token;
			}
			raw_request.queryFlg = "1";
			
			try{
				raw_request.sign = WbCPSSignature.CreateSignature(raw_request);
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_SIGNATURE;
				error.Message = "署名作成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			string content;
			try{
				content = CreateJsonContent(typeof(WbCPSRawConfirmRequest),raw_request);
			}catch(Exception ex){			
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_CONTENT;
				error.Message = "リクエストメッセージ構成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}
			
			string url;
			try{
				string token = CreateToken();
				result.Token = token;
				url = CreateUrl(apiPathConfirmV1,token);
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_URL;
				error.Message = "リクエスURL生成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			Task<string> t = Task.Run(() => WbCPSHttpClient.Post(url,content));
			try{
				t.Wait();
			}catch(AggregateException ae){
				error.SubType = WbCPSError.Lib.PROGRAM;
				error.Code = WbCPSError.Lib.Program.PROGRAM_ERROR;
				error.Message = ae.Message;

				foreach(var ex in ae.InnerExceptions){
					if(ex is WbCPSHttpException){
						error.SubType = WbCPSError.Lib.HTTP;
						error.Code = WbCPSError.Lib.Http.POST;
						error.Message = ex.Message;
						return result;
					}else{
						error.Message = ex.Message;
					}
				}
				return result;
			}

			WbCPSRawConfirmResponseV1 raw_response;
			try{
				raw_response = ReadJsonContent(typeof(WbCPSRawConfirmResponseV1),t.Result) as WbCPSRawConfirmResponseV1;
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.PARSE_CONTENT;
				error.Message = "レスポンスメッセージ解析エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			string sign = raw_response.data.sign;
			raw_response.data.sign = null;

			bool sign_verify_ret;
			try{
				sign_verify_ret =WbCPSSignature.VerifySignature(raw_response,sign);
			}catch(Exception ex){	
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.PARSE_SIGNATURE;
				error.Message = "署名検証エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}
			if(!sign_verify_ret){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.VERIFY_SIGNATURE;
				error.Message = "レスポンスの署名が正しくありません。";
				return result;
			}

			WbCPSConfirmResponse response = new WbCPSConfirmResponse();
			string error_mess = "";
			try{
				error_mess = "meta";
				if(raw_response.meta.code == null || Int32.Parse(raw_response.meta.code) != 0){
					SetErrorInfo(error,
							raw_response.data.errorCode,raw_response.data.subErrorCode,raw_response.data.errorInfo);
					return result;
				}
				error_mess = "transStatus";
				response.TransStatus = Int32.Parse(raw_response.data.result.transStatus);
				error_mess = "payChekDate";
				response.PayCheckDate = raw_response.data.result.payCheckDate;
				error_mess = "transTime";
				response.TransTime = raw_response.data.result.transTime;
				error_mess = "orderId";
				response.OrderId = raw_response.data.result.orderId;
				error_mess = "payType";
				response.PayType = WbCPSRawCode.GetPayType(raw_response.data.result.payType);
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.ILLEGAL_PARAMETER;
				error.Message = "レスポンスパラメータ異常:"+ error_mess + "が有効ではありません。:"
										+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			result.IsSuccess = true;
			result.Error = null;
			result.Response = response;

			return result;
		}
		internal static WbCPSDepositResult DepositRequest(WbCPSDepositRequest request)
		{
			WbCPSDepositResult result = new WbCPSDepositResult();
			WbCPSErrorResult error = new WbCPSErrorResult();
			error.Type = WbCPSError.LIB;
			result.IsSuccess = false;
			result.Error = error;

			if(!openFlag){
				error.SubType = WbCPSError.Lib.PROGRAM;
				error.Code = WbCPSError.Lib.Program.PROGRAM_ERROR;
				error.Message = "プログラムエラー:WbCPSClientがオープンされていません。";
				return result;
			}
						
			WbCPSRawDepositRequest raw_request = new WbCPSRawDepositRequest();
			raw_request.locale = locale;
			raw_request.timeZone = timeZone;
			raw_request.payType = WbCPSRawCode.GetPayCode(request.PayType);
			raw_request.branchCode = branchCode;
			raw_request.terminalCode = terminalCode;
			raw_request.currencyCode = currencyCode;
			raw_request.valueType = WbCPSRawCode.GetValueCode(request.ValueType);
			if(CurrencyType == WbCPSCurrencyType.JPY){
				raw_request.amount = request.Amount.ToString("F0");
			}else{
				raw_request.amount = request.Amount.ToString("F2");
			}
			raw_request.receiptNo = request.ReceiptNo;
			raw_request.userCode = request.UserCode;
			raw_request.appVersion = appVersion;
			raw_request.sign = null;
			
			try{
				raw_request.sign = WbCPSSignature.CreateSignature(raw_request);
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_SIGNATURE;
				error.Message = "署名作成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			string content;
			try{
				content = CreateJsonContent(typeof(WbCPSRawDepositRequest),raw_request);
			}catch(Exception ex){			
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_CONTENT;
				error.Message = "リクエストメッセージ構成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}
			
			string url;
			try{
				string token = CreateToken();
				result.Token = token;
				url = CreateUrl(apiPathDeposit,token);
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.REQUEST;
				error.Code = WbCPSError.Lib.Request.CREATE_URL;
				error.Message = "リクエスURL生成エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			Task<string> t = Task.Run(() => WbCPSHttpClient.Post(url,content));
			try{
				t.Wait();
			}catch(AggregateException ae){
				error.SubType = WbCPSError.Lib.PROGRAM;
				error.Code = WbCPSError.Lib.Program.PROGRAM_ERROR;
				error.Message = ae.Message;

				foreach(var ex in ae.InnerExceptions){
					if(ex is WbCPSHttpException){
						error.SubType = WbCPSError.Lib.HTTP;
						error.Code = WbCPSError.Lib.Http.POST;
						error.Message = ex.Message;
						return result;
					}else{
						error.Message = ex.Message;
					}
				}
				return result;
			}

			WbCPSRawDepositResponse raw_response;
			try{
				raw_response = ReadJsonContent(typeof(WbCPSRawDepositResponse),t.Result) as WbCPSRawDepositResponse;
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.PARSE_CONTENT;
				error.Message = "レスポンスメッセージ解析エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			string sign = raw_response.data.sign;
			raw_response.data.sign = null;

			bool sign_verify_ret;
			try{
				sign_verify_ret =WbCPSSignature.VerifySignature(raw_response,sign);
			}catch(Exception ex){	
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.PARSE_SIGNATURE;
				error.Message = "署名検証エラー:"
								+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}
			if(!sign_verify_ret){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.VERIFY_SIGNATURE;
				error.Message = "レスポンスの署名が正しくありません。";
				return result;
			}

			WbCPSDepositResponse response = new WbCPSDepositResponse();
			string error_mess = "";
			try{
				error_mess = "meta";
				if(raw_response.meta.code == null || Int32.Parse(raw_response.meta.code) != 0){
					SetErrorInfo(error,
							raw_response.data.errorCode,raw_response.data.subErrorCode,raw_response.data.errorInfo);
					return result;
				}
				error_mess = "orderId";
				response.OrderId = raw_response.data.result.orderId;
				error_mess = "transTime";
				response.TransTime = raw_response.data.result.transTime;
				if(raw_response.data.result.amount == null && raw_response.data.result.amountRmb == null){
					error_mess = "amount と amountRmb";
					throw new Exception("どちらかの値が必要です。");
				}
				if(raw_response.data.result.amount != null){
					error_mess = "amount";
					response.Amount = Decimal.Parse(raw_response.data.result.amount,
													System.Globalization.NumberStyles.Number);
				}
				error_mess = "transStatus";
				response.TransStatus = Int32.Parse(raw_response.data.result.transStatus);
			}catch(Exception ex){
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.ILLEGAL_PARAMETER;
				error.Message = "レスポンスパラメータ異常:"+ error_mess + "が有効ではありません。:"
										+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				return result;
			}

			result.IsSuccess = true;
			result.Error = null;
			result.Response = response;

			return result;
		}

		static void SetErrorInfo(WbCPSErrorResult error,string errorCode,string subErrorCode,string errorInfo)
		{
			try{
				error.Type = Int32.Parse(errorCode.Substring(1,2));
				error.SubType = Int32.Parse(errorCode.Substring(3,1));
				error.Code = Int32.Parse(errorCode.Substring(4,2));
				error.SubCode = subErrorCode??"";
				error.Message = errorInfo;
			}catch(Exception ex){
				error.Type = WbCPSError.LIB;
				error.SubType = WbCPSError.Lib.RESPONSE;
				error.Code = WbCPSError.Lib.Response.ILLEGAL_PARAMETER;
				error.SubCode = "";
				error.Message = "レスポンスパラメータ異常:errorCodeが有効ではありません。:"
										+ (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
			}
		}

		static string CreateJsonContent(Type type,Object obj)
		{
			using (var ms = new MemoryStream())
			using (var sr = new StreamReader(ms))
			{
				var serializer = new DataContractJsonSerializer(type);
				serializer.WriteObject(ms,obj);
				ms.Position = 0;

				return sr.ReadToEnd();
			}
		}
		static Object ReadJsonContent(Type type,string data)
		{
			using(var ms = new MemoryStream())
			using(var sw = new StreamWriter(ms))
			{
				var serializer = new DataContractJsonSerializer(type);

				sw.Write(data);
				sw.Flush();

				ms.Position = 0;
				return serializer.ReadObject(ms);
			}
		}
	}
}
