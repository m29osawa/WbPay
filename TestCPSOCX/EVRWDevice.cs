using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OposElectronicValueRW_CCO;
using System.Runtime.Serialization.Json;

namespace TestCPSOCX
{
	public enum WbCPSMethod{
		None			= 0,
		Pay,
		CancelValue,
		Reverse,
		Confirm,
		Deposit,
	}
	public class EVRWDevice
    {
		private static long serialNo = 999_999_999_999_950;	// 15桁	端末でユニークにする必要有
		
		public static string TerminalUNQID {
			set{	if(value.Length > 13){
						value = value.Substring(0,13);
					}
					terminalUNQID = value.PadLeft(13,'0');
			}
		}

		static string terminalUNQID = null;

        public OPOSElectronicValueRW EVRW;
		public WbCPSMethod CurrentMethod = WbCPSMethod.None;

		private void initSerialNo()
		{
			// 排他制御はここに入れる

			//serialNo = 999_999_999_999_950;
			serialNo = 0;
		}

		static string getSrialNoString()
		{
			serialNo += 1;
			if(serialNo >= 1_000_000_000_000_000) serialNo = 0;

			return String.Format("{0:D15}",serialNo);
		}


		public EVRWDevice()
		{
			initSerialNo();

			//for(int i = 0;i < 100;i++){
			//	Console.WriteLine("{0}",getSrialNoString());
			//}

			//return;

			EVRW = new OPOSElectronicValueRW();

            EVRW.Open("CPS_PaymentModule");
			//PrintInfo();

			//OPOSElectronicValueRW EVRW2;
			//EVRW2 = new OPOSElectronicValueRW();
			//Console.WriteLine("Open2");
			//EVRW.Open("CPS_PaymentModule");
			//PrintInfo();
			//Console.WriteLine("resultCode = {0},resultCodeExtended = {1}",EVRW2.ResultCode,EVRW2.ResultCodeExtended);


			EVRW.ErrorEvent += OnErrorEvent;
			EVRW.OutputCompleteEvent += OnOutputCompleteEvent;
			//EVRW.DataEvent += OnDataEvent;

			Console.WriteLine("ClaimDevice");
			EVRW.ClaimDevice(-1);

			//EVRW.CurrentService = "CPM";
			Console.WriteLine("DeviceEnable");
			EVRW.DeviceEnabled = true;
			//EVRW.DataEventEnabled = true;
			EVRW.AsyncMode = true;

			PrintInfo();
		}
		public void PrintInfo()
		{
			Console.WriteLine("********************PrintInfo*********************");
			Console.WriteLine("resultCode = {0},resultCodeExtended = {1}",EVRW.ResultCode,EVRW.ResultCodeExtended);
			Console.WriteLine("OutputID:{0}",EVRW.OutputID);


			Console.WriteLine("AutoDisable:{0}",EVRW.AutoDisable);
			Console.WriteLine("BinaryConversion:{0}",EVRW.BinaryConversion);
			Console.WriteLine("Claimed:{0}",EVRW.Claimed);
			Console.WriteLine("DataCount:{0}",EVRW.DataCount);
			Console.WriteLine("DataEventEnabled:{0}",EVRW.DataEventEnabled);
			Console.WriteLine("DeviceEnabled:{0}",EVRW.DeviceEnabled);
			Console.WriteLine("FreezeEvents:{0}",EVRW.FreezeEvents);
			Console.WriteLine("OpenResult:{0}",EVRW.OpenResult);
			Console.WriteLine("PowerNotify:{0}",EVRW.PowerNotify);
			Console.WriteLine("PowerState:{0}",EVRW.PowerState);
			Console.WriteLine("State:{0}",EVRW.State);

			Console.WriteLine("ControlObjectDescription:{0}", EVRW.ControlObjectDescription);
            Console.WriteLine("ControlObjectVersion:{0}", EVRW.ControlObjectVersion);
            Console.WriteLine("ServiceObjectDescription:{0}", EVRW.ServiceObjectDescription);
            Console.WriteLine("ServiceObjectVersin:{0}", EVRW.ServiceObjectVersion);
            Console.WriteLine("DeviceDescription:{0}", EVRW.DeviceDescription);
            Console.WriteLine("DeviceName:{0}", EVRW.DeviceName);
			Console.WriteLine("CurrentService:{0}",EVRW.CurrentService);
			
			
			Console.WriteLine("AccountNumber:{0}",EVRW.AccountNumber);
			Console.WriteLine("Amount:{0}",EVRW.Amount);
			Console.WriteLine("ApprovalCode:{0}",EVRW.ApprovalCode);
			Console.WriteLine("Balance:{0}",EVRW.Balance);
			Console.WriteLine("BalanceOfPoint:{0}",EVRW.BalanceOfPoint);
			Console.WriteLine("CardServiceList:{0}",EVRW.CardServiceList);
			
			Console.WriteLine("DetectionControl:{0}",EVRW.DetectionControl);
			Console.WriteLine("DetectionStatus:{0}",EVRW.DetectionStatus);
			Console.WriteLine("ExpirationDate:{0}",EVRW.ExpirationDate);
			Console.WriteLine("LastUsedDate:{0}",EVRW.LastUsedDate);
			Console.WriteLine("LogStatus:{0}",EVRW.LogStatus);


			Console.WriteLine("MediumID:{0}",EVRW.MediumID);
			Console.WriteLine("PINEntry:{0}",EVRW.PINEntry);
			Console.WriteLine("Point:{0}",EVRW.Point);
			Console.WriteLine("ReaderWriterServiceList:{0}",EVRW.ReaderWriterServiceList);
			Console.WriteLine("SequenceNumber:{0}",EVRW.SequenceNumber);
			Console.WriteLine("SettledAmount:{0}",EVRW.SettledAmount);
			Console.WriteLine("SettledPoint:{0}",EVRW.SettledPoint);
			Console.WriteLine("TrainingModeState:{0}",EVRW.TrainingModeState);
			Console.WriteLine("TransactionLog:{0}",EVRW.TransactionLog);

			//string token =  null;
			//EVRW.RetrieveResultInformation("qryToken",ref token);
			//Console.WriteLine("qryToken:{0}",token);

			PrintASI();

		}
		public void Close() {
			//EVRW.FreezeEvents = true;
			EVRW.AsyncMode = false;
			Console.WriteLine("DeviceEnable = false");
			EVRW.DeviceEnabled = false;

			Console.WriteLine("RelaseDevice");
			EVRW.ReleaseDevice();

			//EVRW.ErrorEvent -= OnErrorEvent;
			//EVRW.OutputCompleteEvent -= OnOutputCompleteEvent;
			Console.WriteLine("ResultCode,ResultCodeExtended:{0},{1}", EVRW.ResultCode, EVRW.ResultCodeExtended);
			Console.WriteLine("Close");
			//EVRW.Close();
		}

		public void PrintASI()
		{
			Console.WriteLine("*********AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);
		}
		public void PayRequest(WbCPSPayRequest arg)
		{
			int	ret = 0;

			var serializer = new DataContractJsonSerializer(typeof(EVRWPayRequestAux));
			EVRWPayRequestAux aux = new EVRWPayRequestAux();

			EVRW.ClearParameterInformation();

			EVRW.SetParameterInformation("terminalUniqueCode",terminalUNQID);
			EVRW.SetParameterInformation("serialNo", getSrialNoString());

			EVRW.SetParameterInformation("payType",WbCPSPay.codeDicPayType[arg.payType]);
			EVRW.SetParameterInformation("amount",arg.amount.ToString());
			EVRW.SetParameterInformation("receiptNo",arg.receiptNo);
			EVRW.SetParameterInformation("oneTimeCode",arg.userCode);

			aux.remark = arg.remark;
			aux.extendInfo = arg.extendInfo;

			using(var ms = new MemoryStream())
			using(var sr = new StreamReader(ms))
			{
				serializer.WriteObject(ms,aux);

				ms.Position = 0;
				string aux_st = sr.ReadToEnd();

				//string ss = "{\"remark\":\"test\",\"extendInfo\":\"test\"}";
				Console.WriteLine("aux = {0}", aux_st);
				EVRW.AdditionalSecurityInformation = aux_st;
			}
			//EVRW.SetParameterInformation("stubUrl", "stubPay/version=1.0/B01/0/0/");
			Console.WriteLine("【SubtractValue】");
			CurrentMethod = WbCPSMethod.Pay;
			//EVRW.FreezeEvents = true;
			ret = EVRW.SubtractValue(0,-1);
			Console.WriteLine("Ret = {0}",ret);
			PrintInfo();
			//,resultCode = {1},resultCodeExtended = {2}",ret,EVRW.ResultCode,EVRW.ResultCodeExtended);
			//PrintASI();
			//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);
			// エラーの場合、error を返すか、error eventを返すようにする必要あり。
			//Console.WriteLine("SubtractValue:OutputID={0}",EVRW.OutputID);
			//Console.WriteLine("SequenceNumber:{0}",EVRW.SequenceNumber);

			//EVRW.FreezeEvents = false;
		}
		WbCPSResult getResult(string code,string errorCode,string subErrorCode,string errorInfo)
		{
			WbCPSResult	ret = new WbCPSResult();

			ret.errorType = Int32.Parse(code);
			if(ret.errorType == 0) return ret;

			ret.errorSubType = Int32.Parse(errorCode.Substring(3,1));
			ret.errorCode = Int32.Parse(errorCode.Substring(4,2));
			//ret.errorCode = errorCode;
			ret.subErrorCode = subErrorCode;
			ret.errorInfo = errorInfo;

			return ret;
		}
		public WbCPSPayResponse PayResponse()
		{
			var serializer = new DataContractJsonSerializer(typeof(CPSPayResponse));

			CPSPayResponse res_CPS;
			string result = null;
			string message = null;
			string transId = null;
			string transTime = null;
			string amount = null;
			string payType = null;

			EVRW.RetrieveResultInformation("result",ref result);
			EVRW.RetrieveResultInformation("message",ref message);
			EVRW.RetrieveResultInformation("transId",ref transId);
			EVRW.RetrieveResultInformation("transTime",ref transTime);
			EVRW.RetrieveResultInformation("amount",ref amount);
			EVRW.RetrieveResultInformation("payType",ref payType);

			//Console.WriteLine("result:{0}",result);
			//Console.WriteLine("message:{0}",message);
			//Console.WriteLine("transID:{0}",transId);
			//Console.WriteLine("transTime:{0}",transTime);
			//Console.WriteLine("amount:{0}",amount);
			//Console.WriteLine("payType:{0}",payType);
			//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);

			using(var ms = new MemoryStream())
			using(var sw = new StreamWriter(ms))
			{
				sw.Write(EVRW.AdditionalSecurityInformation);
				sw.Flush();

				ms.Position = 0;
				res_CPS = serializer.ReadObject(ms) as CPSPayResponse;

				//Console.WriteLine("RESPONSE:response.meta.code = {0}",res_CPS.meta.code);
				//Console.WriteLine("RESPONSE:response.meta.message = {0}",res_CPS.meta.message);
				//Console.WriteLine("RESPONSE:response.data.errorCode = {0}",res_CPS.data.errorCode);
				//Console.WriteLine("RESPONSE:response.data.errorInfo = {0}",res_CPS.data.errorInfo);
				//Console.WriteLine("RESPONSE:response.data.subErrorCode = {0}",res_CPS.data.subErrorCode);
				//Console.WriteLine("RESPONSE:response.data.sign = {0}",res_CPS.data.sign);
				//Console.WriteLine("RESPONSE:response.data.result.orderDetailId = {0}",res_CPS.data.result.orderDetailId);
				//Console.WriteLine("RESPONSE:response.data.result.orderId = {0}",res_CPS.data.result.orderId);
				//Console.WriteLine("RESPONSE:response.data.result.transTime = {0}",res_CPS.data.result.transTime);
				//Console.WriteLine("RESPONSE:response.data.result.currencyCode = {0}",res_CPS.data.result.currencyCode);
				//Console.WriteLine("RESPONSE:response.data.result.amount = {0}",res_CPS.data.result.amount);
				//Console.WriteLine("RESPONSE:response.data.result.amountRmb = {0}",res_CPS.data.result.amountRmb);
				//Console.WriteLine("RESPONSE:response.data.result.transStatus = {0}",res_CPS.data.result.transStatus);
				//Console.WriteLine("RESPONSE:response.data.result.payType = {0}",res_CPS.data.result.payType);
			}

			WbCPSPayResponse response = new WbCPSPayResponse();
			response.result = getResult(result,res_CPS.data.errorCode,res_CPS.data.subErrorCode,res_CPS.data.errorInfo);
			response.orderDetailId =res_CPS.data.result.orderDetailId;
			response.orderId = transId;
			response.transTime = transTime;
			response.currencyCode = WbCPSPay.getCurrencyType(res_CPS.data.result.currencyCode);
			response.amount = Int64.Parse(amount??"0");	// エラーチェックできていない！とりあえず??入れただけ
			response.transStatus = Int32.Parse(res_CPS.data.result.transStatus??"0");// エラーチェックできていない！とりあえず??入れただけ
			response.payType = WbCPSPay.getPayType(payType);// エラーチェックできていない！

			return response;
		}


		public void RefundRequest(WbCPSRefundRequest arg)
		{
			var serializer = new DataContractJsonSerializer(typeof(EVRWRefundRequestAux));
			EVRWRefundRequestAux request = new EVRWRefundRequestAux();

			EVRW.ClearParameterInformation();

			EVRW.SetParameterInformation("terminalUniqueCode",terminalUNQID);
			EVRW.SetParameterInformation("serialNo", getSrialNoString());

			EVRW.SetParameterInformation("amount",arg.amount.ToString());
			EVRW.SetParameterInformation("originRequestId",arg.orderId);

			
			//EVRW.SetParameterInformation("orderDetailId","bbb");
			//EVRW.SetParameterInformation("transId","bbb");
			//EVRW.SetParameterInformation("orderId","bbb");
			request.orderDetailId = arg.orderDetailId;//なぜか、効かない！！
			request.refundReason = arg.refundReason;
			request.remark = arg.remark;
			request.qryToken = null;

			using(var ms = new MemoryStream())
			using(var sr = new StreamReader(ms))
			{
				serializer.WriteObject(ms,request);

				ms.Position = 0;
				string aux_st = sr.ReadToEnd();

				//aux_st = "{\"orderId\":\"aaaaa\",\"transId\":\"XXX\",\"orderDetailId\":\"XXXXXXXX\",\"refundReason\":\"refundReason test\",\"remark\":\"remark test\"}";
				//aux_st = "{\"transId\":\"XXXXXXXX\",\"refundReason\":\"refundReason test\",\"remark\":\"remark test\"}";
				Console.WriteLine("aux = {0}",aux_st);
				EVRW.AdditionalSecurityInformation = aux_st;
			}
			//EVRW.SetParameterInformation("stubUrl", "stubRefund/B01/0/0/");
			Console.WriteLine("【Refund】");
			CurrentMethod = WbCPSMethod.CancelValue;
			int	ret = 0;
			ret = EVRW.CancelValue(0,-1);
			Console.WriteLine("Ret = {0}",ret);
			PrintInfo();
			//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);
			//// エラーの場合、error を返すか、error eventを返すようにする必要あり。
			//Console.WriteLine("CancelValue:OutputID={0}",EVRW.OutputID);
			//Console.WriteLine("SequenceNumber:{0}",EVRW.SequenceNumber);
		}
		public WbCPSRefundResponse RefundResponse()
		{
			var serializer = new DataContractJsonSerializer(typeof(CPSRefundResponse));

			CPSRefundResponse res_CPS;
			string result = null;
			string message = null;
			string transId = null;
			string amount = null;
			string transTime = null;
			
			EVRW.RetrieveResultInformation("result",ref result);
			EVRW.RetrieveResultInformation("message",ref message);
			EVRW.RetrieveResultInformation("transId",ref transId);
			EVRW.RetrieveResultInformation("amount",ref amount);
			EVRW.RetrieveResultInformation("transTime",ref transTime);

			//Console.WriteLine("result:{0}",result);
			//Console.WriteLine("message:{0}",message);
			//Console.WriteLine("transId:{0}",transId);
			//Console.WriteLine("amount:{0}",amount);
			//Console.WriteLine("transTime:{0}",transTime);
			//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);

			using(var ms = new MemoryStream())
			using(var sw = new StreamWriter(ms))
			{
				sw.Write(EVRW.AdditionalSecurityInformation);
				sw.Flush();
				
				ms.Position = 0;
				res_CPS = serializer.ReadObject(ms) as CPSRefundResponse;

				//Console.WriteLine("RESPONSE:response.meta.code = {0}",res_CPS.meta.code);
				//Console.WriteLine("RESPONSE:response.meta.message = {0}",res_CPS.meta.message);
				//Console.WriteLine("RESPONSE:response.data.errorCode = {0}",res_CPS.data.errorCode);
				//Console.WriteLine("RESPONSE:response.data.errorInfo = {0}",res_CPS.data.errorInfo);
				//Console.WriteLine("RESPONSE:response.data.subErrorCode = {0}",res_CPS.data.subErrorCode);
				//Console.WriteLine("RESPONSE:response.data.sign = {0}",res_CPS.data.sign);
				//Console.WriteLine("RESPONSE:response.data.result.orderId = {0}",res_CPS.data.result.orderId);
				//Console.WriteLine("RESPONSE:response.data.result.orderDetailId = {0}",res_CPS.data.result.orderDetailId);
				//Console.WriteLine("RESPONSE:response.data.result.transStatus = {0}",res_CPS.data.result.transStatus);
				//Console.WriteLine("RESPONSE:response.data.result.currencyCode = {0}",res_CPS.data.result.currencyCode);
				//Console.WriteLine("RESPONSE:response.data.result.refundAamount = {0}",res_CPS.data.result.refundAmount);
				//Console.WriteLine("RESPONSE:response.data.result.transTime = {0}",res_CPS.data.result.transTime);
			}

			WbCPSRefundResponse response = new WbCPSRefundResponse();
			response.result = getResult(result,res_CPS.data.errorCode,res_CPS.data.subErrorCode,res_CPS.data.errorInfo);
			response.orderId = res_CPS.data.result.orderId;
			response.orderDetailId = transId;
			response.transStatus = Int32.Parse(res_CPS.data.result.transStatus);
			response.currencyCode = WbCPSPay.getCurrencyType(res_CPS.data.result.currencyCode);
			response.transTime = transTime;

			return response;
		}

		public int ReverseRequest(WbCPSReverseRequest arg)
		{
			var serializer = new DataContractJsonSerializer(typeof(EVRWReverseRequestAux));
			EVRWReverseRequestAux aux = new EVRWReverseRequestAux();

			EVRW.ClearParameterInformation();

			EVRW.SetParameterInformation("terminalUniqueCode",terminalUNQID);
			EVRW.SetParameterInformation("serialNo",getSrialNoString());

			EVRW.SetParameterInformation("originRequestId",arg.orderId); // 文字数チェック必要！！
			
			aux.qryToken = null;

			using(var ms = new MemoryStream())
			using(var sr = new StreamReader(ms))
			{
				serializer.WriteObject(ms,aux);

				ms.Position = 0;
				string aux_st = sr.ReadToEnd();

				EVRW.AdditionalSecurityInformation = aux_st;
			}
			//EVRW.SetParameterInformation("stubUrl", "stubReverse/B01/0/0/");
			Console.WriteLine("【Reverse】");
			CurrentMethod = WbCPSMethod.Reverse;
			string ret_string = null;
			int		ret = 0;
			ret = EVRW.DirectIO(0,0,ref ret_string);
			Console.WriteLine("Ret = {0},ret_string = {1}",ret,ret_string);
			//PrintInfo();
			//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);
			// エラーの場合、error を返すか、error eventを返すようにする必要あり。
			//Console.WriteLine("Reverse:OutputID={0}",EVRW.OutputID);
			//Console.WriteLine("SequenceNumber:{0}",EVRW.SequenceNumber);

			return ret;
		}

		public WbCPSReverseResponse ReverseResponse()
		{
			Console.WriteLine("ResultReverseValue");
			var serializer = new DataContractJsonSerializer(typeof(CPSReverseResponse));

			CPSReverseResponse res_CPS;
			string result = null;
			string message = null;
			string transId = null;
			string amount = null;
			string transTime = null;
			
			EVRW.RetrieveResultInformation("result",ref result);
			EVRW.RetrieveResultInformation("message",ref message);
			EVRW.RetrieveResultInformation("transId",ref transId);
			EVRW.RetrieveResultInformation("amount",ref amount);
			EVRW.RetrieveResultInformation("transTime",ref transTime);
			
			//Console.WriteLine("result:{0}",result);
			//Console.WriteLine("message:{0}",message);
			//Console.WriteLine("transID:{0}",transId);
			//Console.WriteLine("amount:{0}",amount);
			//Console.WriteLine("transTime:{0}",transTime);
			//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);

			using(var ms = new MemoryStream())
			using(var sw = new StreamWriter(ms))
			{
				sw.Write(EVRW.AdditionalSecurityInformation);
				sw.Flush();

				ms.Position = 0;
				res_CPS = serializer.ReadObject(ms) as CPSReverseResponse;

				//Console.WriteLine("RESPONSE:response.meta.code = {0}",res_CPS.meta.code);
				//Console.WriteLine("RESPONSE:response.meta.message = {0}",res_CPS.meta.message);
				//Console.WriteLine("RESPONSE:response.data.errorCode = {0}",res_CPS.data.errorCode);
				//Console.WriteLine("RESPONSE:response.data.errorInfo = {0}",res_CPS.data.errorInfo);
				//Console.WriteLine("RESPONSE:response.data.subErrorCode = {0}",res_CPS.data.subErrorCode);
				//Console.WriteLine("RESPONSE:response.data.sign = {0}",res_CPS.data.sign);
				//Console.WriteLine("RESPONSE:response.data.result.orderId = {0}",res_CPS.data.result.orderId);
				//Console.WriteLine("RESPONSE:response.data.result.orderDetailId = {0}",res_CPS.data.result.orderDetailId);
				//Console.WriteLine("RESPONSE:response.data.result.transStatus = {0}",res_CPS.data.result.transStatus);
				//Console.WriteLine("RESPONSE:response.data.result.currencyCode = {0}",res_CPS.data.result.currencyCode);
				//Console.WriteLine("RESPONSE:response.data.result.amount = {0}",res_CPS.data.result.amount);
				//Console.WriteLine("RESPONSE:response.data.result.transTime = {0}",res_CPS.data.result.transTime);
			}

			WbCPSReverseResponse response = new WbCPSReverseResponse();
			response.result = getResult(result,res_CPS.data.errorCode,res_CPS.data.subErrorCode,res_CPS.data.errorInfo);
			response.orderId = res_CPS.data.result.orderId;
			response.orderDetailId = transId;
			response.transStatus = Int32.Parse(res_CPS.data.result.transStatus);
			response.amount = Int64.Parse(amount);
			response.currencyCode = WbCPSPay.getCurrencyType(res_CPS.data.result.currencyCode);
			response.transTime = transTime;

			return response;
		}

		public void ConfirmRequest(WbCPSConfirmRequest arg)
		{
			var serializer = new DataContractJsonSerializer(typeof(EVRWConfirmRequestAux));
			EVRWConfirmRequestAux aux = new EVRWConfirmRequestAux();

			EVRW.ClearParameterInformation();

			EVRW.SetParameterInformation("terminalUniqueCode",terminalUNQID);
			EVRW.SetParameterInformation("serialNo",getSrialNoString());

			EVRW.SetParameterInformation("originRequestId",arg.orderId); // 文字数チェック要

			aux.orderDetailId = arg.orderDetailId;	//文字数チェック表
			aux.qryToken = null;
			aux.queryFlg = "1";

			using(var ms = new MemoryStream())
			using(var sr = new StreamReader(ms))
			{
				serializer.WriteObject(ms,aux);

				ms.Position = 0;
				string aux_st = sr.ReadToEnd();

				Console.WriteLine("aux = {0}", aux_st);
				EVRW.AdditionalSecurityInformation = aux_st;
			}
			//EVRW.SetParameterInformation("stubUrl", "stubConfirm/version=1.0/B01/0/0/");
			Console.WriteLine("【Confirm】");
			CurrentMethod = WbCPSMethod.Confirm;
			string ret_string = null;
			int		ret = 0;
			ret = EVRW.DirectIO(1,0,ref ret_string);
			Console.WriteLine("Ret = {0},ret_string = {1}",ret,ret_string);
			//PrintInfo();
			//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);
			//Console.WriteLine("Confirm:OutputID={0}",EVRW.OutputID);
			//Console.WriteLine("SequenceNumber:{0}",EVRW.SequenceNumber);
		}

		public WbCPSConfirmResponse ConfirmResponse()
		{
			Console.WriteLine("ResultConfirm");
			var serializer = new DataContractJsonSerializer(typeof(CPSConfirmResponse));

			CPSConfirmResponse res_CPS;
			string result = null;
			string message = null;
			string transTime = null;
			string transId = null;
			string payType = null;

			EVRW.RetrieveResultInformation("result",ref result);
			EVRW.RetrieveResultInformation("message",ref message);
			EVRW.RetrieveResultInformation("transTime",ref transTime);
			EVRW.RetrieveResultInformation("transId",ref transId);
			EVRW.RetrieveResultInformation("payType",ref payType);

			//Console.WriteLine("result:{0}",st);
			//Console.WriteLine("message:{0}",st);
			//Console.WriteLine("transTime:{0}",st);
			//Console.WriteLine("transID:{0}",st);
			//Console.WriteLine("payType:{0}",st);
			//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);

			using(var ms = new MemoryStream())
			using(var sw = new StreamWriter(ms))
			{
				sw.Write(EVRW.AdditionalSecurityInformation);
				sw.Flush();
				//using(var ms = new MemoryStream(enc.GetBytes(EVRW.AdditionalSecurityInformation))) {

				ms.Position = 0;
				res_CPS = serializer.ReadObject(ms) as CPSConfirmResponse;

				//Console.WriteLine("RESPONSE:response.meta.code = {0}",res_CPS.meta.code);
				//Console.WriteLine("RESPONSE:response.meta.message = {0}",res_CPS.meta.message);
				//Console.WriteLine("RESPONSE:response.data.errorCode = {0}",res_CPS.data.errorCode);
				//Console.WriteLine("RESPONSE:response.data.errorInfo = {0}",res_CPS.data.errorInfo);
				//Console.WriteLine("RESPONSE:response.data.subErrorCode = {0}",res_CPS.data.subErrorCode);
				//Console.WriteLine("RESPONSE:response.data.sign = {0}",res_CPS.data.sign);
				//Console.WriteLine("RESPONSE:response.data.result.transStatus = {0}",res_CPS.data.result.transStatus);
				//Console.WriteLine("RESPONSE:response.data.result.payCheckDate = {0}",res_CPS.data.result.payCheckDate);
				//Console.WriteLine("RESPONSE:response.data.result.transTime = {0}",res_CPS.data.result.transTime);
				//Console.WriteLine("RESPONSE:response.data.result.orderId = {0}",res_CPS.data.result.orderId);
				//Console.WriteLine("RESPONSE:response.data.result.orderDetailId = {0}",res_CPS.data.result.orderDetailId);
				//Console.WriteLine("RESPONSE:response.data.result.payType= {0}",res_CPS.data.result.payType);
			}

			WbCPSConfirmResponse response = new WbCPSConfirmResponse();
			response.result = getResult(result,res_CPS.data.errorCode,res_CPS.data.subErrorCode,res_CPS.data.errorInfo);
			response.transStatus = Int32.Parse(res_CPS.data.result.transStatus);
			response.payCheckDate = res_CPS.data.result.payCheckDate;
			response.transTime = transTime;
			response.orderId = transId;
			response.orderDetailId = res_CPS.data.result.orderDetailId;
			response.payType = WbCPSPay.getPayType(payType);

			return response;
		}
		public void DepositRequest(WbCPSDepositRequest arg)
		{
			var serializer = new DataContractJsonSerializer(typeof(EVRWDepositRequestAux));
			EVRWDepositRequestAux request = new EVRWDepositRequestAux();

			EVRW.SetParameterInformation("terminalUniqueCode",terminalUNQID);
			EVRW.SetParameterInformation("serialNo", getSrialNoString());

			EVRW.SetParameterInformation("payType",WbCPSPay.codeDicPayType[arg.payType]);
			EVRW.SetParameterInformation("amount",arg.amount.ToString());
			EVRW.SetParameterInformation("receiptNo",arg.receiptNo);
			EVRW.SetParameterInformation("oneTimeCode",arg.userCode);

			request.valueType = WbCPSPay.codeDicValueType[arg.valueType]; // valueTypeは指定不要の場合があるその時の処理していない

			using(var ms = new MemoryStream())
			using(var sr = new StreamReader(ms))
			{
				serializer.WriteObject(ms,request);

				ms.Position = 0;
				string aux_st = sr.ReadToEnd();

				//string ss = "{\"remark\":\"test\",\"extendInfo\":\"test\"}";
				//Console.WriteLine("aux = {0}",aux_st);
				EVRW.AdditionalSecurityInformation = aux_st;
			}
			//EVRW.SetParameterInformation("stubUrl", "stubDeposit/B01/0/0/");
			Console.WriteLine("【Deposit】");
			CurrentMethod = WbCPSMethod.Deposit;
			int	ret = 0;
			ret = EVRW.AddValue(0,-1);
			Console.WriteLine("Ret = {0}",ret);
			PrintInfo();
			//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);
			//Console.WriteLine("Deposit:OutputID={0}",EVRW.OutputID);
			//Console.WriteLine("SequenceNumber:{0}",EVRW.SequenceNumber);

		}
		public WbCPSDepositResponse DepositResponse()
		{
			var serializer = new DataContractJsonSerializer(typeof(CPSDepositResponse));

			CPSDepositResponse res_CPS;
			string result = null;
			string message = null;
			string transId = null;
			string transTime = null;
			string amount = null;

			EVRW.RetrieveResultInformation("result",ref result);
			EVRW.RetrieveResultInformation("message",ref message);
			EVRW.RetrieveResultInformation("transId",ref transId);
			EVRW.RetrieveResultInformation("transTime",ref transTime);
			EVRW.RetrieveResultInformation("amount",ref amount);

			//Console.WriteLine("ResultDeposit");
			//Console.WriteLine("result:{0}",result);
			//Console.WriteLine("message:{0}",message);
			//Console.WriteLine("transId:{0}",transId);
			//Console.WriteLine("transTime:{0}",transTime);
			//Console.WriteLine("amount:{0}",amount);
			//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);

			using(var ms = new MemoryStream())
			using(var sw = new StreamWriter(ms))
			{
				sw.Write(EVRW.AdditionalSecurityInformation);
				sw.Flush();
				
				ms.Position = 0;
				res_CPS = serializer.ReadObject(ms) as CPSDepositResponse;

				//Console.WriteLine("RESPONSE:response.meta.code = {0}",res_CPS.meta.code);
				//Console.WriteLine("RESPONSE:response.meta.message = {0}",res_CPS.meta.message);
				//Console.WriteLine("RESPONSE:response.data.errorCode = {0}",res_CPS.data.errorCode);
				//Console.WriteLine("RESPONSE:response.data.errorInfo = {0}",res_CPS.data.errorInfo);
				//Console.WriteLine("RESPONSE:response.data.subErrorCode = {0}",res_CPS.data.subErrorCode);
				//Console.WriteLine("RESPONSE:response.data.sign = {0}",res_CPS.data.sign);
				//Console.WriteLine("RESPONSE:response.data.result.orderDetailId = {0}",res_CPS.data.result.orderDetailId);
				//Console.WriteLine("RESPONSE:response.data.result.orderId = {0}",res_CPS.data.result.orderId);
				//Console.WriteLine("RESPONSE:response.data.result.transTime = {0}",res_CPS.data.result.transTime);
				//Console.WriteLine("RESPONSE:response.data.result.currencyCode = {0}",res_CPS.data.result.currencyCode);
				//Console.WriteLine("RESPONSE:response.data.result.amount = {0}",res_CPS.data.result.amount);
				//Console.WriteLine("RESPONSE:response.data.result.amountRmb = {0}",res_CPS.data.result.amountRmb);
				//Console.WriteLine("RESPONSE:response.data.result.transStatus = {0}",res_CPS.data.result.transStatus);
			}

			WbCPSDepositResponse response = new WbCPSDepositResponse();
			response.result = getResult(result,res_CPS.data.errorCode,res_CPS.data.subErrorCode,res_CPS.data.errorInfo);
			response.orderId = res_CPS.data.result.orderDetailId;
			response.orderDetailId = res_CPS.data.result.orderDetailId;
			response.transTime = transTime;
			response.currencyCode = WbCPSPay.getCurrencyType(res_CPS.data.result.currencyCode);
			response.transStatus = Int32.Parse(res_CPS.data.result.transStatus);

			return response;
		}
	


		

		void OnOutputCompleteEvent(int outputID)
		{
			//Encoding enc = Encoding.UTF8;

			//var serializer = new DataContractJsonSerializer(typeof(CPSPayResponse));

			//string st = "";
			Console.WriteLine("■■OnOutputCompleteEvent:{0}",outputID);


			switch(CurrentMethod){
				case WbCPSMethod.Pay:
					WbCPSPayResponse pay_res = PayResponse();
					//Console.WriteLine("pay_res.result.resultCode = {0}",pay_res.result.resultCode);
					Console.WriteLine("pay_res.result.errorType = {0}",pay_res.result.errorType);
					Console.WriteLine("pay_res.result.errorSubType = {0}",pay_res.result.errorSubType);
					Console.WriteLine("pay_res.result.errorCode = {0}",pay_res.result.errorCode);
					Console.WriteLine("pay_res.result.subErrorCode = {0}",pay_res.result.subErrorCode);
					Console.WriteLine("pay_res.result.errorInfo = {0}",pay_res.result.errorInfo);
					Console.WriteLine("pay_res.orderId = {0}",pay_res.orderId);
					Console.WriteLine("pay_res.orderDetailId = {0}",pay_res.orderDetailId);
					Console.WriteLine("pay_res.transTime = {0}",pay_res.transTime);
					Console.WriteLine("pay_res.currencyCode = {0}",pay_res.currencyCode);
					Console.WriteLine("pay_res.amount = {0}",pay_res.amount);
					Console.WriteLine("pay_res.transStatus = {0}",pay_res.transStatus);
					Console.WriteLine("pay_res.payType = {0}",pay_res.payType);
					break;
				case WbCPSMethod.CancelValue:
					WbCPSRefundResponse refund_res = RefundResponse();
					//Console.WriteLine("refund_res.result.resultCode = {0}",refund_res.result.resultCode);
					Console.WriteLine("refund_res.result.errorType = {0}",refund_res.result.errorType);
					Console.WriteLine("refund_res.result.errorSubType = {0}",refund_res.result.errorSubType);
					Console.WriteLine("refund_res.result.errorCode = {0}",refund_res.result.errorCode);
					Console.WriteLine("refund_res.result.subErrorCode = {0}",refund_res.result.subErrorCode);
					Console.WriteLine("pay_res.result.errorInfo = {0}",refund_res.result.errorInfo);
					Console.WriteLine("refund_res.orderId = {0}",refund_res.orderId);
					Console.WriteLine("refund_res.orderDetailId = {0}",refund_res.orderDetailId);
					Console.WriteLine("refund_res.transStatus = {0}",refund_res.transStatus);
					Console.WriteLine("refund_res.currencyCode = {0}",refund_res.currencyCode);
					Console.WriteLine("refund_res.amount = {0}",refund_res.amount);
					Console.WriteLine("refund_res.transTime = {0}",refund_res.transTime);
					break;
				case WbCPSMethod.Reverse:
					WbCPSReverseResponse reverse_res = ReverseResponse();
					//Console.WriteLine("reverse_res.result.resultCode = {0}",reverse_res.result.resultCode);
					Console.WriteLine("reverse_res.result.errorType = {0}",reverse_res.result.errorType);
					Console.WriteLine("reverse_res.result.errorSubType = {0}",reverse_res.result.errorSubType);
					Console.WriteLine("reverse_res.result.errorCode = {0}",reverse_res.result.errorCode);
					Console.WriteLine("reverse_res.result.subErrorCode = {0}",reverse_res.result.subErrorCode);
					Console.WriteLine("pay_res.result.errorInfo = {0}",reverse_res.result.errorInfo);
					Console.WriteLine("reverse_res.orderId = {0}",reverse_res.orderId);
					Console.WriteLine("reverse_res.orderDetailI = {0}",reverse_res.orderDetailId);
					Console.WriteLine("reverse_res.transStatus = {0}",reverse_res.transStatus);
					Console.WriteLine("reverse_res.amount = {0}",reverse_res.amount);
					Console.WriteLine("reverse_res.currencyCode = {0}",reverse_res.currencyCode);
					Console.WriteLine("reverse_res.transTime = {0}",reverse_res.transTime);
					break;
				case WbCPSMethod.Confirm:
					WbCPSConfirmResponse confirm_res = ConfirmResponse();
					//Console.WriteLine("confirm_res.result.resultCode = {0}",confirm_res.result.resultCode);
					Console.WriteLine("confirm_res.result.errorType = {0}",confirm_res.result.errorType);
					Console.WriteLine("confirm_res.result.errorSubType = {0}",confirm_res.result.errorSubType);
					Console.WriteLine("confirm_res.result.errorCode = {0}",confirm_res.result.errorCode);
					Console.WriteLine("confirm_res.result.subErrorCode = {0}",confirm_res.result.subErrorCode);
					Console.WriteLine("pay_res.result.errorInfo = {0}",confirm_res.result.errorInfo);
					Console.WriteLine("confirm_res.transStatus = {0}",confirm_res.transStatus);
					Console.WriteLine("confirm_res.payCheckDate = {0}",confirm_res.payCheckDate);
					Console.WriteLine("confirm_res.transTime = {0}",confirm_res.transTime);
					Console.WriteLine("confirm_res.orderId = {0}",confirm_res.orderId);
					Console.WriteLine("confirm_res.orderDetailId = {0}",confirm_res.orderDetailId);
					Console.WriteLine("confirm_res.payType = {0}",confirm_res.payType);
					break;
				case WbCPSMethod.Deposit:
					WbCPSDepositResponse deposit_res = DepositResponse();
					//Console.WriteLine("deposit_res.result.resultCode = {0}",deposit_res.result.resultCode);
					Console.WriteLine("deposit_res.result.errorType = {0}",deposit_res.result.errorType);
					Console.WriteLine("deposit_res.result.errorSubType = {0}",deposit_res.result.errorSubType);
					Console.WriteLine("deposit_res.result.errorCode = {0}",deposit_res.result.errorCode);
					Console.WriteLine("deposit_res.result.subErrorCode = {0}",deposit_res.result.subErrorCode);
					Console.WriteLine("pay_res.result.errorInfo = {0}",deposit_res.result.errorInfo);
					Console.WriteLine("deposit_res.orderId = {0}",deposit_res.orderId);
					Console.WriteLine("deposit_res.orderDetailId = {0}",deposit_res.orderDetailId);
					Console.WriteLine("deposit_res.transTime = {0}",deposit_res.transTime);
					Console.WriteLine("deposit_res.currencyCode = {0}",deposit_res.currencyCode);
					Console.WriteLine("deposit_res.transStatus = {0}",deposit_res.transStatus);
					break;
				default:
					break;
			}

			CurrentMethod = WbCPSMethod.None;
		
			//EVRW.RetrieveResultInformation("result",ref st);
			//Console.WriteLine("result:{0}",st);
			//EVRW.RetrieveResultInformation("message",ref st);
			//Console.WriteLine("message:{0}",st);
			//EVRW.RetrieveResultInformation("transId",ref st);
			//Console.WriteLine("transID:{0}",st);
			//EVRW.RetrieveResultInformation("transTime",ref st);
			//Console.WriteLine("transTime:{0}",st);
			//EVRW.RetrieveResultInformation("amount",ref st);
			//Console.WriteLine("amount:{0}",st);
			//EVRW.RetrieveResultInformation("payType",ref st);
			//Console.WriteLine("payType:{0}",st);
			//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);

			//using(var ms = new MemoryStream())
			//using(var sw = new StreamWriter(ms))
			//{
			//	sw.Write(EVRW.AdditionalSecurityInformation);
			//	sw.Flush();
			//	//using(var ms = new MemoryStream(enc.GetBytes(EVRW.AdditionalSecurityInformation))) {

			//	ms.Position = 0;
			//	var pay_response = serializer.ReadObject(ms) as CPSPayResponse;

			//	Console.WriteLine("RESPONSE:pay_response.meta.code = {0}",pay_response.meta.code);
			//	Console.WriteLine("RESPONSE:pay_response.meta.message = {0}",pay_response.meta.message);
			//	Console.WriteLine("RESPONSE:pay_response.data.errorCode = {0}",pay_response.data.errorCode);
			//	Console.WriteLine("RESPONSE:pay_response.data.errorInfo = {0}",pay_response.data.errorInfo);
			//	Console.WriteLine("RESPONSE:pay_response.data.subErrorCode = {0}",pay_response.data.subErrorCode);
			//	Console.WriteLine("RESPONSE:pay_response.data.sign = {0}",pay_response.data.sign);
			//	Console.WriteLine("RESPONSE:pay_response.data.result.orderDetailId = {0}",pay_response.data.result.orderDetailId);
			//	Console.WriteLine("RESPONSE:pay_response.data.result.orderId = {0}",pay_response.data.result.orderId);
			//	Console.WriteLine("RESPONSE:pay_response.data.result.transTime = {0}",pay_response.data.result.transTime);
			//	Console.WriteLine("RESPONSE:pay_response.data.result.currencyCode = {0}",pay_response.data.result.currencyCode);
			//	Console.WriteLine("RESPONSE:pay_response.data.result.amount = {0}",pay_response.data.result.amount);
			//	Console.WriteLine("RESPONSE:pay_response.data.result.amountRmb = {0}",pay_response.data.result.amountRmb);
			//	Console.WriteLine("RESPONSE:pay_response.data.result.transStatus = {0}",pay_response.data.result.transStatus);
			//	Console.WriteLine("RESPONSE:pay_response.data.result.payType = {0}",pay_response.data.result.payType);
			//}

		}
		void OnErrorEvent(int resultCode,int resultCodeExtended,int errorLocus,ref int pErrorResponse){
			Console.WriteLine("■■ErorEvent: resultCode={0},resultCodeExteneded={1},errorLocus={2},pErrorResponse={3}",resultCode,resultCodeExtended,errorLocus,pErrorResponse);
			//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);
			PrintInfo();
			switch(CurrentMethod) {
				case WbCPSMethod.Pay:
					//WbCPSPayResponse pay_res = PayResponse();// やっぱりエラーの時はこれは動かない。なぜならjSONが入っていないから
					//Console.WriteLine("pay_res.result.resultCode = {0}",pay_res.result.resultCode);
					//Console.WriteLine("pay_res.result.errorType = {0}",pay_res.result.errorType);
					//Console.WriteLine("pay_res.result.errorSubType = {0}",pay_res.result.errorSubType);
					//Console.WriteLine("pay_res.result.errorCode = {0}",pay_res.result.errorCode);
					//Console.WriteLine("pay_res.result.subErrorCode = {0}",pay_res.result.subErrorCode);
					//Console.WriteLine("pay_res.orderId = {0}",pay_res.orderId);
					//Console.WriteLine("pay_res.orderDetailId = {0}",pay_res.orderDetailId);
					//Console.WriteLine("pay_res.transTime = {0}",pay_res.transTime);
					//Console.WriteLine("pay_res.currencyCode = {0}",pay_res.currencyCode);
					//Console.WriteLine("pay_res.amount = {0}",pay_res.amount);
					//Console.WriteLine("pay_res.transStatus = {0}",pay_res.transStatus);
					//Console.WriteLine("pay_res.payType = {0}",pay_res.payType);
					break;
				//	case WbCPSMethod.Pay:
				//		WbCPSPayResponse pay_res = PayResponse();// やっぱりエラーの時はこれは動かない。なぜならjSONが入っていないから
				//		Console.WriteLine("pay_res.result.resultCode = {0}",pay_res.result.resultCode);
				//		Console.WriteLine("pay_res.result.errorType = {0}",pay_res.result.errorType);
				//		Console.WriteLine("pay_res.result.errorSubType = {0}",pay_res.result.errorSubType);
				//		Console.WriteLine("pay_res.result.errorCode = {0}",pay_res.result.errorCode);
				//		Console.WriteLine("pay_res.result.subErrorCode = {0}",pay_res.result.subErrorCode);
				//		Console.WriteLine("pay_res.orderId = {0}",pay_res.orderId);
				//		Console.WriteLine("pay_res.orderDetailId = {0}",pay_res.orderDetailId);
				//		Console.WriteLine("pay_res.transTime = {0}",pay_res.transTime);
				//		Console.WriteLine("pay_res.currencyCode = {0}",pay_res.currencyCode);
				//		Console.WriteLine("pay_res.amount = {0}",pay_res.amount);
				//		Console.WriteLine("pay_res.transStatus = {0}",pay_res.transStatus);
				//		Console.WriteLine("pay_res.payType = {0}",pay_res.payType);
				//		break;
				//	case WbCPSMethod.CancelValue:
				//		ResultCancelValue();
				//		break;
				//	case WbCPSMethod.Reverse:
				//		ReverseResponse();
				//		break;
				case WbCPSMethod.Confirm:
					WbCPSConfirmResponse confirm_res = ConfirmResponse();
					//Console.WriteLine("confirm_res.result.resultCode = {0}",confirm_res.result.resultCode);
					Console.WriteLine("confirm_res.result.errorType = {0}",confirm_res.result.errorType);
					Console.WriteLine("confirm_res.result.errorSubType = {0}",confirm_res.result.errorSubType);
					Console.WriteLine("confirm_res.result.errorCode = {0}",confirm_res.result.errorCode);
					Console.WriteLine("confirm_res.result.subErrorCode = {0}",confirm_res.result.subErrorCode);
					Console.WriteLine("pay_res.result.errorInfo = {0}",confirm_res.result.errorInfo);
					Console.WriteLine("confirm_res.transStatus = {0}",confirm_res.transStatus);
					Console.WriteLine("confirm_res.payCheckDate = {0}",confirm_res.payCheckDate);
					Console.WriteLine("confirm_res.transTime = {0}",confirm_res.transTime);
					Console.WriteLine("confirm_res.orderId = {0}",confirm_res.orderId);
					Console.WriteLine("confirm_res.orderDetailId = {0}",confirm_res.orderDetailId);
					Console.WriteLine("confirm_res.payType = {0}",confirm_res.payType);
					break;
				//	case WbCPSMethod.Deposit:
				//		ResultDeposit();
				//		break;
				default:
					break;
			}
		}
		void OnDataEvent(int status){
			Console.WriteLine("■■DataEvent:{0}",status);
			Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);
			PrintInfo();
		}
		
    }
}
