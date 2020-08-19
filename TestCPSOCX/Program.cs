using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.PointOfService;
//using OposElectronicValueRW_CCO;


namespace TestCPSOCX
{
    class Program
    {
        static void Main(string[] args)
        {

			Console.WriteLine("Open");

			////EVRWDevice.TerminalUNQID = "123450123456A";
			EVRWDevice.TerminalUNQID = "TERMINALUNIQX";
			////EVRWDevice.TerminalUNQID = "AAAA";
			////EVRWDevice.TerminalUNQID = "ZYXWVUTSRQPONMLKJIHGFEDCBA";
			EVRWDevice dev = new EVRWDevice();

			//string terminal_id = "TERMINALUNIQX";
			//WbCPSPay2 dev = new WbCPSPay2(terminal_id);

			//return;

			//Console.WriteLine("Pay");
			//WbCPSPayRequest pay_request = new WbCPSPayRequest();
			//pay_request.payType = WbCPSPayType.AutoDetect;
			//pay_request.amount = 1000;
			//pay_request.receiptNo = "10002220001";
			//pay_request.userCode = "2016092613361229";
			//pay_request.remark = "remark test";
			////pay_request.extendInfo = "extendInfo test";
			//dev.PayRequest(pay_request);
			////con.EVRW.FreezeEvents = true;
			////for(int i = 0;i < 200;i++) {
			////	//for(int i = 0;i < 10;i++) {
			////	//for(int i = 0;!dev.complete_flag;i++){
			////	Console.WriteLine("{0}",i);

			////	WbCPSConfirmRequest confirm_request = new WbCPSConfirmRequest();
			////	confirm_request.orderId = "111";
			////	confirm_request.orderDetailId = "222";
			////	dev.ConfirmRequest(confirm_request);


			//	//dev.PrintInfo();
			//	//Thread.Sleep(1000);
			//	//if(i == 10)con.EVRW.FreezeEvents = false;
			//}

			////dev.ExecPayRequest2();
			//for(int i = 0;i < 5;i++) {
			//	Console.WriteLine("{0}", i);
			//	//dev.PrintInfo();
			//	Thread.Sleep(1000);
			//	//if(i == 10)con.EVRW.FreezeEvents = false;
			//}

			//Console.WriteLine("Reverse");
			//WbCPSReverseRequest reverse_request = new WbCPSReverseRequest();
			//reverse_request.orderId = "OWC20161104181148726rpVQ";
			//dev.ReverseRequest(reverse_request);
			//for(int i = 0;i < 5;i++) {
			//	Console.WriteLine("{0}", i);
			//	//dev.PrintInfo();
			//	Thread.Sleep(1000);
			//}

			Console.WriteLine("Confirm");
			WbCPSConfirmRequest confirm_request = new WbCPSConfirmRequest();
			confirm_request.orderId = "111";
			//confirm_request.orderDetailId = "222";
			dev.ConfirmRequest(confirm_request);
			for(int i = 0;i < 5;i++) {
				Console.WriteLine("{0}", i);
				//dev.PrintInfo();
				Thread.Sleep(1000);
			}


			//Console.WriteLine("Refund");
			//WbCPSRefundRequest refund_request = new WbCPSRefundRequest();
			//refund_request.amount = 2000;
			//refund_request.orderId = "111";
			//refund_request.orderDetailId = "222";
			//refund_request.refundReason = "Refund Reason";
			//refund_request.remark = "Refund Remark";
			//dev.RefundRequest(refund_request);
			//for(int i = 0;i < 5;i++) {
			//	Console.WriteLine("{0}", i);
			//	//dev.PrintInfo();
			//	Thread.Sleep(1000);
			//}


			//Console.WriteLine("Deposit");
			//WbCPSDepositRequest deposit_request = new WbCPSDepositRequest();
			//deposit_request.payType = WbCPSPayType.LinePay;
			//deposit_request.valueType = WbCPSValueType.Basic;
			//deposit_request.amount = 100;
			//deposit_request.receiptNo = "10002220001";
			//deposit_request.userCode = "2016092613361229";
			//dev.DepositRequest(deposit_request);
			//for(int i = 0;i < 5;i++) {
			//	Console.WriteLine("{0}", i);
			//	//dev.PrintInfo();
			//	Thread.Sleep(1000);
			//}


			Console.WriteLine("Close");
			dev.Close();
			for(int i = 0;i < 5;i++){
				Console.WriteLine("{0}",i);
				Thread.Sleep(1000);
				//if(i == 10)con.EVRW.FreezeEvents = false;
			}
			
		}
	}
}
