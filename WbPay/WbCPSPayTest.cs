using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WbPay
{
	public class WbCPSPayTest
	{
		public static void Test1()
		{
			//WbCPSHttpClient client = new WbCPSHttpClient();
			//WbCPSClient.Open("dir");
			//string uri_st = "http://localhost:8080/Test/Sample.html";
			//string uri_st = "https://localhost:8443/Test/Sample.html";
			//string uri_st = "https://www.google.co.jp";
			//string uri_st = "http://localhost:8080/Test/Sample1?abc=123&def=ABCDEFG";
			//string uri_st = "http://localhost:8080/gateway/pay/C01/00/0/1003685910426110040bQQW23GEWEAJUgEmW4S3pY4z?abc=123&def=ABCDEFG";
			string url = "https://localhost:8443/gateway/pay/C01/00/0/1003685910426110040bQQW23GEWEAJUgEmW4S3pY4z?abc=123&def=ABCDEFG";
			//string url = "http://abcdefg.co.jp/gateway/pay";
			//string uri_st = "https://localhost:8443/Test/Sample1";

			string request = "ABCDEFG12345678";
			Task<string> t = Task.Run(() => WbCPSHttpClient.Post(url,request));

			//t.Wait();

			//Console.WriteLine(t.Result);
			Console.WriteLine("End Test1");
		}

		public static void Test2()
		{
			Console.WriteLine("Start Test2");

			//WbCPSClient.Open("dir");

			Console.WriteLine("****Request Pay");

			WbCPSPayRequest pay_request = new WbCPSPayRequest();
			pay_request.PayType = WbCPSPayType.AutoDetect;
			pay_request.Amount = 1000;
			pay_request.ReceiptNo = "10002220001";
			pay_request.UserCode = "2016092613361229";
			pay_request.Remark = "remark test";

			WbCPSPayResult pay_result = WbCPSClient.PayRequest(pay_request);

			Console.WriteLine("result.IsSuccess:{0}",pay_result.IsSuccess);
			if(pay_result.IsSuccess){
				WbCPSPayResponse pay_response = pay_result.Response;
				Console.WriteLine("pay_response.OrderId:{0}",pay_response.OrderId);
				Console.WriteLine("pay_response.TransTime:{0}",pay_response.TransTime);
				Console.WriteLine("pay_response.Amount:{0}",pay_response.Amount);
				Console.WriteLine("pay_response.TransStatus:{0}",pay_response.TransStatus);
				Console.WriteLine("pay_response.PayType:{0}",pay_response.PayType);
			}else{
				WriteError(pay_result.Error);
			}
			Console.WriteLine("pay_result.Token:{0}",pay_result.Token);

			Console.WriteLine("****Request Refund");

			WbCPSRefundRequest refund_request = new WbCPSRefundRequest();
			refund_request.RefundAmount = 2000;
			refund_request.OrderId = "111";
			refund_request.RefundReason = "Refund Reason";
			refund_request.Remark = "Refund Remark";

			WbCPSRefundResult refund_result = WbCPSClient.RefundRequest(refund_request);
			if(refund_result.IsSuccess) {
				WbCPSRefundResponse refund_response = refund_result.Response;
				Console.WriteLine("refund_response.OrderId:{0}", refund_response.OrderId);
				Console.WriteLine("refund_response.TransStatus:{0}", refund_response.TransStatus);
				Console.WriteLine("refund_response.RefundAmount:{0}", refund_response.RefundAmount);
				Console.WriteLine("refund_response.TransTime:{0}", refund_response.TransTime);
			} else {
				WriteError(refund_result.Error);
			}
			Console.WriteLine("refund_result.Token:{0}", refund_result.Token);

			Console.WriteLine("****Request Reverse");

			WbCPSReverseRequest reverse_request = new WbCPSReverseRequest();
			reverse_request.OrderId = "OWC20161104181148726rpVQ";

			WbCPSReverseResult reverse_result = WbCPSClient.ReverseRequest(reverse_request,null);
			if(reverse_result.IsSuccess) {
				WbCPSReverseResponse reverse_response = reverse_result.Response;
				Console.WriteLine("reverse_response.OrderId:{0}", reverse_response.OrderId);
				Console.WriteLine("reverse_response.TransStatus:{0}", reverse_response.TransStatus);
				Console.WriteLine("reverse_response.Amount:{0}", reverse_response.Amount);
				Console.WriteLine("reverse_response.TransTime:{0}", reverse_response.TransTime);
			} else {
				WriteError(reverse_result.Error);
			}
			Console.WriteLine("reverse_result.Token:{0}", reverse_result.Token);

			Console.WriteLine("****Request Confirm");

			WbCPSConfirmRequest confirm_request = new WbCPSConfirmRequest();

			confirm_request.OrderId = "111";

			WbCPSConfirmResult confirm_result = WbCPSClient.ConfirmRequest(confirm_request,null);
			if(confirm_result.IsSuccess) {
				WbCPSConfirmResponse confirm_response = confirm_result.Response;
				Console.WriteLine("confirm_response.TransStatus:{0}", confirm_response.TransStatus);
				Console.WriteLine("confirm_response.PayCheckDate:{0}", confirm_response.PayCheckDate);
				Console.WriteLine("confirm_response.TransTime:{0}", confirm_response.TransTime);
				Console.WriteLine("confirm_response.OrderId:{0}", confirm_response.OrderId);
				Console.WriteLine("confirm_response.PayType:{0}", confirm_response.PayType);
			} else {
				WriteError(confirm_result.Error);
			}
			Console.WriteLine("confirm_result.Token:{0}", confirm_result.Token);

			Console.WriteLine("****Request Deposit");

			WbCPSDepositRequest deposit_request = new WbCPSDepositRequest();
			deposit_request.PayType = WbCPSPayType.LinePay;
			deposit_request.ValueType = WbCPSValueType.Basic;
			deposit_request.Amount = 100;
			deposit_request.ReceiptNo = "10002220001";
			deposit_request.UserCode = "2016092613361229";

			WbCPSDepositResult deposit_result = WbCPSClient.DepositRequest(deposit_request);

			Console.WriteLine("deposit_result.IsSuccess:{0}", deposit_result.IsSuccess);
			if(deposit_result.IsSuccess) {
				WbCPSDepositResponse deposit_response = deposit_result.Response;
				Console.WriteLine("deposit_response.OrderId:{0}", deposit_response.OrderId);
				Console.WriteLine("deposit_response.TransTime:{0}", deposit_response.TransTime);
				Console.WriteLine("deposit_response.Amount:{0}", deposit_response.Amount);
				Console.WriteLine("deposit_response.TransStatus:{0}", deposit_response.TransStatus);
			} else {
				WriteError(deposit_result.Error);
			}
			Console.WriteLine("deposit_result.Token:{0}", deposit_result.Token);

			Console.WriteLine("End Test2");
		}
		static void WriteError(WbCPSErrorResult error)
		{
			Console.WriteLine("error.Type:{0}",error.Type);
			Console.WriteLine("error.SubType:{0}",error.SubType);
			Console.WriteLine("error.Code:{0}",error.Code);
			Console.WriteLine("error.SubCode:{0}",error.SubCode);
			Console.WriteLine("error.Message:{0}",error.Message);
		}
		

	}
}
