using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using WbPay;

namespace SampleCPS
{
	public partial class Form1 : Form
	{
		WbCPSPay cpsPay;
		decimal requestAmount;
		string	orderId;

		public Form1() {
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e) {
			cpsPay = new WbCPSPay();
			cpsPay.CompleteEvent += OnCompleteEvent;
			cpsPay.ErrorEvent += OnErrorEvent;
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
			cpsPay.Close();

		}

		private void buttonPay_Click(object sender, EventArgs e)
		{
			try{
				if(textAmount.Text.Length <= 0)	return;
				if(textUserCode.Text.Length <= 0) return;

				WbCPSPayRequest request = new WbCPSPayRequest();
				request.Amount = Decimal.Parse(textAmount.Text);
				request.UserCode = textUserCode.Text;

				requestAmount = request.Amount;
				cpsPay.Pay(request);
			}catch(Exception ex){
				MessageBox.Show(ex.Message,
								"エラー",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error);
			}
		}
		private void buttonRefund_Click(object sender, EventArgs e)
		{
			try{	
				if(textRefundAmount.Text.Length <= 0) return;

				object item = listOrder.SelectedItem;
				if(item == null) return;
			
				string st = item.ToString();
				int n = st.IndexOf('\t');
				if(n <= 0) return;

				WbCPSRefundRequest request = new WbCPSRefundRequest();
				request.RefundAmount = Decimal.Parse(textRefundAmount.Text);
				request.OrderId = st.Substring(0,n);

				cpsPay.Refund(request);
			}catch(Exception ex){
				MessageBox.Show(ex.Message,
								"エラー",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error);
			}
		}
		private void buttonConfirm_Click(object sender, EventArgs e)
		{
			//try {
			//	object item = listOrder.SelectedItem;
			//	if(item == null) return;

			//	string st = item.ToString();
			//	int n = st.IndexOf('\t');
			//	if(n <= 0) return;

			//	WbCPSConfirmRequest request = new WbCPSConfirmRequest();
			//	request.OrderId = st.Substring(0, n);

			//	cpsPay.Confirm(request);
			//} catch(Exception ex) {
			//	MessageBox.Show(ex.Message,
			//					"エラー",
			//					MessageBoxButtons.OK,
			//					MessageBoxIcon.Error);
			//}
		}

		void OnCompleteEventPay(WbCPSPayResponse response)
		{
			orderId = response.OrderId;

			if(response.TransStatus == 14) {
				DialogResult ret = MessageBox.Show("お客様はパスコードを入力されましたか？",
								"パスコード入力確認",
								MessageBoxButtons.OKCancel,
								MessageBoxIcon.Question);
				try{
					if(ret == DialogResult.OK){
						cpsPay.PayConfirm(true);
					}else{
						cpsPay.PayConfirm(false);
					}
				}catch(Exception ex){
					MessageBox.Show(ex.Message,
									"エラー",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error);
				}
			}else if(response.TransStatus == 0) {
				string st = response.OrderId + "\t"
							+ "支払" + "\t"
							+ response.Amount.ToString() + "\t"
							+ response.PayType.ToString() + "\t" 
							+ response.TransTime;
				listOrder.Items.Add(st);
			}
		}
		//void OnCompleteEventPayConfirm(WbCPSConfirmResponse response)
		//{
		//	if(response.TransStatus == 0) {
		//		string st = response.OrderId + "\t"
		//					+ "支払" + "\t"
		//					+ requestAmount.ToString() + "\t"
		//					+ response.PayType.ToString() + "\t" 
		//					+ response.TransTime;
		//		listOrder.Items.Add(st);
		//	}
		//}
		void OnCompleteEventRefund(WbCPSRefundResponse response)
		{
			if(response.TransStatus == 6) {
				string st = response.OrderId + "\t"
							+ "返金" + "\t"
							+ response.RefundAmount.ToString() + "\t"
							+ "      " + "\t" 
							+ response.TransTime;
				listOrder.Items.Add(st);
			}
		}
		void OnCompleteEventReverse(WbCPSReverseResponse response)
		{
			if(response.TransStatus == 10) {
				string st = response.OrderId + "\t"
							+ "取消" + "\t"
							+ response.Amount.ToString() + "\t"
							+ "      " + "\t" 
							+ response.TransTime;
				listOrder.Items.Add(st);
			}
		}
		void OnCompleteEventConfirm(WbCPSConfirmResponse response)
		{
			MessageBox.Show("オーダＩＤ　　　：" + response.OrderId + "\n"		
							+ "取引ステータス：" + response.TransStatus.ToString() + "\n"
							+ "支払チャンネル：" + response.PayType + "\n"
							+ "取引日時　　　：" + response.TransTime + "\n"
							+ "確認日　　　　：" + response.PayCheckDate,
							"オーダ確認",
							MessageBoxButtons.OK,
							MessageBoxIcon.Information);
		}
		
		void OnCompleteEvent(WbCPSRequestType type, Object obj) {
			if(InvokeRequired) {
				//異なるスレッドから呼び出された場合、Invoke メソッドを使用して、適切なスレッドへの呼び出しをマーシャリングします。
				Invoke(new WbCPSCompleteEventHandler(OnCompleteEvent), new object[2] { type, obj });
				return;
			}

			switch(type) {
				case WbCPSRequestType.Pay:
					OnCompleteEventPay(obj as WbCPSPayResponse);
					break;
				case WbCPSRequestType.Refund:
					OnCompleteEventRefund(obj as WbCPSRefundResponse);
					break;
				case WbCPSRequestType.Reverse:
					OnCompleteEventReverse(obj as WbCPSReverseResponse);
					break;
				//case WbCPSRequestType.PayConfirm:
				//	OnCompleteEventPayConfirm(obj as WbCPSConfirmResponse);
				//	break;
				case WbCPSRequestType.Confirm:
					OnCompleteEventConfirm(obj as WbCPSConfirmResponse);
					break;
				case WbCPSRequestType.Deposit:
					break;
				default:
					break;
			}
		}
		void OnErrorEvent(WbCPSRequestType type, WbCPSResponseError error) {
			if(InvokeRequired) {
				//異なるスレッドから呼び出された場合、Invoke メソッドを使用して、適切なスレッドへの呼び出しをマーシャリングします。
				Invoke(new WbCPSErrorEventHandler(OnErrorEvent), new object[2] { type, error });
				return;
			}
			if(error.Code == "E06014"){
				DialogResult ret =  MessageBox.Show("パスコードがまだ入力されていません\n"
														+"再度確認しますか、キャンセルしますか？",
													"パスコード入力確認",
													MessageBoxButtons.OKCancel,
													MessageBoxIcon.Question);
				try{
					if(ret == DialogResult.OK){
						cpsPay.PayConfirm(true);
					}else{
						cpsPay.PayConfirm(false);
					}
				}catch(Exception ex){
						MessageBox.Show(ex.Message,
										"エラー",
										MessageBoxButtons.OK,
										MessageBoxIcon.Error);
				}
			}else{
				MessageBox.Show("コード　　：" + error.Code + " " + error.SubCode + "\n"
								+ "メッセージ：" + error.Message + "\n"
								+ "連絡先　　：" + error.RequestAction.ToString(),
								"エラー",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error);
			}
		}
	}
}
