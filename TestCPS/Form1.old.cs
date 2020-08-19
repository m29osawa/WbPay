using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WbPay;

namespace TestCPS
{
	public partial class Form1 : Form
	{
		WbCPSPay cps_pay;

		public Form1() {
			InitializeComponent();
		}
		
		//private async void button1_Click(object sender, EventArgs e)
		private void button1_Click(object sender, EventArgs e)
		{
			
            Console.WriteLine("BUTTON:START");

			//Task t = TestClient.Test1();
			//Task t = Process1.TestProcess();
			//await Process1.TestProcess();
			//Process1.TestProcess();
			//Process1.TestProcess2();
			//Process1.TestProcess3();
			//Process1.TestProcess4();
			//Process1.TestProcess5();
			//Task t = Task.Run(() => Process1.TestProcess());
			//WbCPSErrorTest.Test1();

			Console.WriteLine("BUTTON:After Process");

			//for(int i = 0;i < 10;i++) {
			//	Console.WriteLine("●");
			//	System.Threading.Thread.Sleep(1000);
			//	//System.Threading.Tasks.Task.Delay(1000);
			//	//t.Wait();
			//	//while(!t.IsCompleted){
			//	//    t.Wait(100);
			//	//Console.WriteLine("●");
			//}
			//Console.WriteLine("");

			Console.WriteLine("BUTTON:END");
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Console.WriteLine("BUTTON2(WbPayTest1):START");
			//WbCPSPayTest.Test1();
			Console.WriteLine("BUTTON2(WbPayTest1):END");
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Console.WriteLine("BUTTON3(WbPayTest2):START");
			//WbCPSPayTest.Test2();
			Console.WriteLine("BUTTON3(WbPayTest2):END");

		}
		
		
		private void Form1_Load(object sender, EventArgs e)
		{

			cps_pay = new WbCPSPay();

			//cps_pay = new WbCPSPay();

			cps_pay.CompleteEvent += OnCompleteEvent;
			cps_pay.ErrorEvent += OnErrorEvent;

		}
		private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
			cps_pay.Close();
			//cps_pay.Close();
		}

		private void button4_Click(object sender, EventArgs e) {

			try{
				WbCPSPayRequest request = new WbCPSPayRequest();
				request.PayType = WbCPSPayType.PayPay;
				request.Amount = 1000;
				request.ReceiptNo = "10002220001";
				//request.ReceiptNo = "012345678901234567890123456789012";
				request.UserCode = "2016092613361229";
				//request.Remark = "remark test";

				cps_pay.Pay(request);
				//cps_pay.Pay(null);

			}catch(Exception ex){
				Console.WriteLine("Exceptin in button4_Click : {0}",ex);
			}

		}
		private void button5_Click(object sender, EventArgs e) {
			WbCPSRefundRequest request = new WbCPSRefundRequest();
			
			request.RefundAmount = 1000;
			request.OrderId = "ORDERIDXXXX";
			//request.OrderId = "01234567890123456789012345678901";
			//request.OrderId = preOrderId;
			//request.RefundReason = "refund reason test";
			//request.Remark = "remark test";
			
			cps_pay.Refund(request);



		}
		private void button6_Click(object sender, EventArgs e) {
			WbCPSReverseRequest request = new WbCPSReverseRequest();

			//request.OrderId = "ORDERIDYYYY";
			request.OrderId = preOrderId;

			cps_pay.Reverse(request);

		}
		private void button7_Click(object sender, EventArgs e) {

			try{
				WbCPSConfirmRequest request = new WbCPSConfirmRequest();

				request.OrderId = "ORDERIDZZZZ";
				//request.OrderId = preOrderId;

				cps_pay.Confirm(request);

				//for(int i = 0;i < 5;i++){
				//	Task t = Task.Run(() => {
				//		try{
				//			cps_pay.Confirm(request);
				//		}catch(Exception ex){
				//			Console.WriteLine("Confirm running : {0}",ex);
				//		}
				//	});
				//}
			} catch(Exception ex){
				Console.WriteLine("Exceptin in button4_Click : {0}",ex);
			}
		}

		private void button8_Click(object sender, EventArgs e) {

			WbCPSDepositRequest request = new WbCPSDepositRequest();
			request.PayType = WbCPSPayType.LinePay;
			request.ValueType = WbCPSValueType.Basic;
			request.Amount = 1000;
			request.ReceiptNo = "10002220001";
			request.UserCode = "2016092613361229";
			
			cps_pay.Deposit(request);

		}
		string preOrderId = "";
		void OnCompleteEvent(WbCPSRequestType type,Object obj)
		{
			if (InvokeRequired){
				Console.WriteLine("InvokeRequired : OnCompleteEvent");
				//異なるスレッドから呼び出された場合、Invoke メソッドを使用して、適切なスレッドへの呼び出しをマーシャリングします。
				Invoke(new WbCPSCompleteEventHandler(OnCompleteEvent),new object[2] { type,obj });
				//for(int i = 0;i < 3;i++) {
				//	Console.WriteLine("■");
				//	System.Threading.Thread.Sleep(1000);
				//}
				return;
			}

			Console.WriteLine("*** OnCompleteEvent");
			try{
			switch(type){
				case WbCPSRequestType.Pay:
					WbCPSPayResponse pay_response = obj as WbCPSPayResponse;
					preOrderId = pay_response.OrderId;

					Console.WriteLine("PayResponse");
					Console.WriteLine("pay_response.OrderId:{0}",pay_response.OrderId);
					Console.WriteLine("pay_response.TransTime:{0}",pay_response.TransTime);
					Console.WriteLine("pay_response.Amount:{0}",pay_response.Amount);
					Console.WriteLine("pay_response.TransStatus:{0}",pay_response.TransStatus);
					Console.WriteLine("pay_response.PayType:{0}",pay_response.PayType);

					if(pay_response.TransStatus == 14) {
							Console.WriteLine("Call PayConfirm");
						WbCPSConfirmRequest request = new WbCPSConfirmRequest();
						request.OrderId = pay_response.OrderId;
						//request.OrderId = pay_response.OrderId + "X";

						cps_pay.PayConfirm(request);
					}

						break;
				case WbCPSRequestType.Refund:
					WbCPSRefundResponse refund_response = obj as WbCPSRefundResponse;
					Console.WriteLine("RefundResponse");
					Console.WriteLine("refund_response.OrderId:{0}",refund_response.OrderId);
					Console.WriteLine("rerund_response.TransStatus:{0}",refund_response.TransStatus);
					Console.WriteLine("refund_response.RefundAmount:{0}",refund_response.RefundAmount);
					Console.WriteLine("rerund_response.TransTime:{0}",refund_response.TransTime);
					break;
				case WbCPSRequestType.Reverse:
					WbCPSReverseResponse reverse_response = obj as WbCPSReverseResponse;
					Console.WriteLine("ReverseResponse");
					Console.WriteLine("reverse_response.OrderId:{0}",reverse_response.OrderId);
					Console.WriteLine("reverse_response.TransStatus:{0}",reverse_response.TransStatus);
					Console.WriteLine("reverse_response.Amount:{0}",reverse_response.Amount);
					Console.WriteLine("reverse_response.TransTime:{0}",reverse_response.TransTime);
					break;
				case WbCPSRequestType.PayConfirm:
				case WbCPSRequestType.Confirm:
					WbCPSConfirmResponse confirm_response = obj as WbCPSConfirmResponse;
					Console.WriteLine("ConfirmResponse");
					Console.WriteLine("confirm_response.TransStatus:{0}",confirm_response.TransStatus);
					Console.WriteLine("confirm_response.PayCheckDate:{0}",confirm_response.PayCheckDate);
					Console.WriteLine("confirm_response.TransTime:{0}",confirm_response.TransTime);
					Console.WriteLine("confirm_response.OrderId:{0}",confirm_response.OrderId);
					Console.WriteLine("confirm_response.PayType:{0}",confirm_response.PayType);
					break;
				case WbCPSRequestType.Deposit:
					WbCPSDepositResponse deposit_response = obj as WbCPSDepositResponse;
					preOrderId = deposit_response.OrderId;
					Console.WriteLine("DeositResponse");
					Console.WriteLine("deposit_response.OrderId:{0}",deposit_response.OrderId);
					Console.WriteLine("deposit_response.TransTime:{0}",deposit_response.TransTime);
					Console.WriteLine("deposit_response.Amount:{0}",deposit_response.Amount);
					Console.WriteLine("deposit_response.TransStatus:{0}",deposit_response.TransStatus);
					break;
				default:
					break;
			}

			
			//for(int i = 0;i < 5;i++) {
			//		Console.WriteLine("▲");
			//		System.Threading.Thread.Sleep(1000);
			//}
			
			}catch(Exception ex){
				Console.WriteLine("Exceptin in button4_Click : {0}",ex);
			}
		}
		void OnErrorEvent(WbCPSRequestType type,WbCPSResponseError error)
		{
			if (InvokeRequired){
				Console.WriteLine("InvokeRequired : OnErrorEvent");
				//異なるスレッドから呼び出された場合、Invoke メソッドを使用して、適切なスレッドへの呼び出しをマーシャリングします。
				Invoke(new WbCPSErrorEventHandler(OnErrorEvent),new object[2] { type,error });
				return;
			}

			Console.WriteLine("*** OnErrorEvent");
			Console.WriteLine("{0}RequestError",type);
			Console.WriteLine("error.Type:{0}",error.Type);
			//Console.WriteLine("error.SubType:{0}",error.SubType);
			Console.WriteLine("error.Code:{0}",error.Code);
			Console.WriteLine("error.SubCode:{0}",error.SubCode);
			Console.WriteLine("error.Message:{0}",error.Message);
			Console.WriteLine("error.RequestAction:{0}",error.RequestAction);

			//for(int i = 0;i < 5;i++) {
			//	Console.WriteLine("■");
			//	System.Threading.Thread.Sleep(1000);
			//}
		}

		
	}
}
