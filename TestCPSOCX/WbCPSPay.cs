using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCPSOCX
{
    public enum WbCPSPayType{
        AutoDetect,
        LinePay,                // LINE Pay
        RegionalElectricMony,   // 地域電子マネー
        OrigamiPay,             // 
        RakutenPay,             // 
        PayPay,                 // 
        dPayment,               // d払い
        CpayAlipay,             // 
        CpayWechat,             // 
        MerPay,                 // 
        AuPay,                  // 
        JCBAliPay,              // 
        QUOCardPay,             // 
        YucoPay,                // 
        KPlus,                  // 
        ResonaWallet,           // 
        EPOSPay,                // 
        JCoinPay,               // 
        pring,                  // 
        atone,                  // 
        aftee,                  // 
        BankPay,                // 
        OnepayAlipay,           // 
        OnepayWechat,           // 
        OnepayGNAlipay,         // 
        OnepayGNAlipayHK,       // 
        OnepayGNkakaopay,       // 
        SmartCode,              // 
    };
    public enum WbCPSCurrencyType{
        CNY,
        GBP,
        HKD,
        USD,
        JPY,
        CAD,
        AUD,
        EUR,
        NZD,
        KRW,
    }
    public enum WbCPSValueType{
        Basic,
        Bonus,
        Coupon,
        Point,
    }

	public class WbCPSResult{
		//public int		resultCode;
		public int		errorType;
		public int		errorSubType;
		public int		errorCode;
		public string	subErrorCode;
        public string   errorInfo;
	}

	public class WbCPSPayRequest{
		public WbCPSPayType	payType;
		public long			amount;
		public string		receiptNo;
        public string       userCode;
		public string		remark;
		public string       extendInfo;
	}
	public class WbCPSPayResponse{
    	public WbCPSResult		    result;
		public string			    orderDetailId;
		public string			    orderId;
		public string			    transTime;
		public WbCPSCurrencyType    currencyCode;
		public long                 amount;
		public int 	    		    transStatus;
		public WbCPSPayType         payType;
	}
    public class WbCPSReverseRequest{
        public string       orderId;
    }
    public class WbCPSReverseResponse{
    	public WbCPSResult		    result;
		public string			    orderId;
        public string			    orderDetailId;
        public int 	    		    transStatus;
        public long                 amount;
		public WbCPSCurrencyType    currencyCode;
        public string			    transTime;
	}
    public class WbCPSConfirmRequest{
        public string       orderId;
        public string       orderDetailId;
    }
    public class WbCPSConfirmResponse{
        public WbCPSResult          result;
        public int                  transStatus;
        public string               payCheckDate;
        public string               transTime;
        public string               orderId;
        public string               orderDetailId;
        public WbCPSPayType         payType;
    }
    public class WbCPSRefundRequest{
        public long             amount;
        public string           orderId;
        public string           orderDetailId;
        public string           refundReason;
        public string           remark;
    }
    public class WbCPSRefundResponse{
        public WbCPSResult          result;
        public string               orderId;
        public string               orderDetailId;
        public int 	    		    transStatus;
        public WbCPSCurrencyType    currencyCode;
        public long                 amount;
        public string               transTime;
    }
    public class WbCPSDepositRequest{
        public WbCPSPayType     payType;
        public WbCPSValueType   valueType;
        public long             amount;
        public string           receiptNo;
        public string           userCode;
    }
    public class WbCPSDepositResponse{
        public WbCPSResult          result;
        public string               orderId;
        public string               orderDetailId;
        public string               transTime;
        public WbCPSCurrencyType    currencyCode;
        public int                  transStatus;
    }
    public class WbCPSPay
    {
        public static Dictionary<WbCPSPayType,string> codeDicPayType = new Dictionary<WbCPSPayType, string>(){

            {WbCPSPayType.LinePay,              "04" },
            {WbCPSPayType.RegionalElectricMony, "05" },
            {WbCPSPayType.OrigamiPay,           "07" },
            {WbCPSPayType.RakutenPay,           "09" },
            {WbCPSPayType.PayPay,               "10" },
            {WbCPSPayType.dPayment,             "11" },
            {WbCPSPayType.CpayAlipay,           "13" },
            {WbCPSPayType.CpayWechat,           "14" },
            {WbCPSPayType.MerPay,               "15" },
            {WbCPSPayType.AuPay,                "16" },
            {WbCPSPayType.JCBAliPay,            "17" },
            {WbCPSPayType.QUOCardPay,           "18" },
            {WbCPSPayType.YucoPay,              "19" },
            {WbCPSPayType.KPlus,                "21" },
            {WbCPSPayType.ResonaWallet,         "22" }, 
            {WbCPSPayType.EPOSPay,              "23" },
            {WbCPSPayType.JCoinPay,             "24" },
            {WbCPSPayType.pring,                "27" },
            {WbCPSPayType.atone,                "28" },
            {WbCPSPayType.aftee,                "29" },
            {WbCPSPayType.BankPay,              "30" },
            {WbCPSPayType.OnepayAlipay,         "O01" },
            {WbCPSPayType.OnepayWechat,         "O02" },
            {WbCPSPayType.OnepayGNAlipay,       "GN01" },
            {WbCPSPayType.OnepayGNAlipayHK,     "GN02" },
            {WbCPSPayType.OnepayGNkakaopay,     "GN03" },
            {WbCPSPayType.SmartCode,            "90" },

            {WbCPSPayType.AutoDetect,           "99" },
        };
        public static WbCPSPayType getPayType(string st)
        {
            foreach(KeyValuePair<WbCPSPayType,string> kvp in codeDicPayType) {
                if(kvp.Value == st) return kvp.Key;
            }
            return 0;   // ここダメ

        }
        public static Dictionary<WbCPSCurrencyType,string> codeDicCurrencyType = new Dictionary<WbCPSCurrencyType, string>(){
            {WbCPSCurrencyType.CNY,"CNY"},
            {WbCPSCurrencyType.GBP,"GBP"},
            {WbCPSCurrencyType.HKD,"HKD"},
            {WbCPSCurrencyType.USD,"USD"},
            {WbCPSCurrencyType.JPY,"JPY"},
            {WbCPSCurrencyType.CAD,"CAD"},
            {WbCPSCurrencyType.AUD,"AUD"},
            {WbCPSCurrencyType.EUR,"EUR"},
            {WbCPSCurrencyType.NZD,"NZD"},
            {WbCPSCurrencyType.KRW,"KRW"},
        };
        public static WbCPSCurrencyType getCurrencyType(string st)
        {
            foreach(KeyValuePair<WbCPSCurrencyType,string> kvp in codeDicCurrencyType) {
                if(kvp.Value == st) return kvp.Key;
            }
            return 0;   // ここダメ

        }
        public static Dictionary<WbCPSValueType,string> codeDicValueType = new Dictionary<WbCPSValueType, string>(){
            {WbCPSValueType.Basic,"A"},
            {WbCPSValueType.Bonus,"B"},
            {WbCPSValueType.Coupon,"C"},
            {WbCPSValueType.Point,"D"},
        }; 
        public static WbCPSValueType getValueType(string st)
        {
            foreach(KeyValuePair<WbCPSValueType,string> kvp in codeDicValueType){
                if(kvp.Value == st) return kvp.Key;
            }
            return 0;   // ここダメ
        }
    }

    public class WbCPSPay2
    {
        EVRWDevice dev;

		WbCPSPayRequest pre_pay_request;

		public bool complete_flag = false;

        public WbCPSPay2(string terminal_id)
        {
            EVRWDevice.TerminalUNQID = terminal_id;
            dev = new EVRWDevice();

        }

        public void PayRequest(WbCPSPayRequest request)
        {
			pre_pay_request = request;

			//dev.EVRW.OutputCompleteEvent += OnOutputCompleteEventPay;
			dev.EVRW.ErrorEvent += OnErrorEventPay;

			dev.PayRequest(request);
        }
		public void Close()
		{
			dev.Close();
		}
        void OnOutputCompleteEventPay(int outputID)
		{
			//Encoding enc = Encoding.UTF8;

			//var serializer = new DataContractJsonSerializer(typeof(CPSPayResponse));

			//string st = "";
			Console.WriteLine("■■OnOutputCompleteEventPay:{0}",outputID);
			
			WbCPSPayResponse pay_res = dev.PayResponse();
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
			
		}
		void OnErrorEventPay(int resultCode,int resultCodeExtended,int errorLocus,ref int pErrorResponse){
			Console.WriteLine("■■ErorEventPay: resultCode={0},resultCodeExteneded={1},errorLocus={2},pErrorResponse={3}",resultCode,resultCodeExtended,errorLocus,pErrorResponse);
			//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);
			dev.PrintInfo();
			WbCPSPayResponse pay_res = dev.PayResponse();
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

			if(resultCode == 106){
				Console.WriteLine("Error 106");
			}else if(resultCode == 111){
				Console.WriteLine("Error 111");
				if(resultCodeExtended == 111000){
					Console.WriteLine("ErrorSub 000");

					switch(pay_res.result.errorType){
						case 2:
							Console.WriteLine("Error E02");
							break;
						case 9:
							switch(pay_res.result.errorSubType){
								case 1:
									Console.WriteLine("Error E091");

									Console.WriteLine("Reverse");
									WbCPSReverseRequest reverse_request = new WbCPSReverseRequest();
									//reverse_request.orderId = pay_res.orderId;
									reverse_request.orderId = "OWC20161104181148726rpVQ";


									//dev.EVRW.OutputCompleteEvent -= OnOutputCompleteEventPay;
									//dev.EVRW.ErrorEvent -= OnErrorEventPay;
									//dev.EVRW.OutputCompleteEvent += OnOutputCompleteEventPayReverse;
									//dev.EVRW.ErrorEvent += OnErrorEventPayReverse;


									//while(dev.ReverseRequest(reverse_request)!= 0);
									




									break;
								case 2:
									Console.WriteLine("Error E092");
									break;
								case 3:
									Console.WriteLine("Error E093");
									break;
								default:
									Console.WriteLine("Error E09?");
									break;
							}
							break;
						default:
							Console.WriteLine("Error???");
							break;
					}
				}else{
					Console.WriteLine("ErrorSub other");
				}
			}else{
				Console.WriteLine("Error other");
			}

			//WbCPSConfirmResponse confirm_res = dev.ConfirmResponse();
			//Console.WriteLine("confirm_res.result.resultCode = {0}",confirm_res.result.resultCode);
			//Console.WriteLine("confirm_res.result.errorType = {0}",confirm_res.result.errorType);
			//Console.WriteLine("confirm_res.result.errorSubType = {0}",confirm_res.result.errorSubType);
			//Console.WriteLine("confirm_res.result.errorCode = {0}",confirm_res.result.errorCode);
			//Console.WriteLine("confirm_res.result.subErrorCode = {0}",confirm_res.result.subErrorCode);
			//Console.WriteLine("pay_res.result.errorInfo = {0}",confirm_res.result.errorInfo);
			//Console.WriteLine("confirm_res.transStatus = {0}",confirm_res.transStatus);
			//Console.WriteLine("confirm_res.payCheckDate = {0}",confirm_res.payCheckDate);
			//Console.WriteLine("confirm_res.transTime = {0}",confirm_res.transTime);
			//Console.WriteLine("confirm_res.orderId = {0}",confirm_res.orderId);
			//Console.WriteLine("confirm_res.orderDetailId = {0}",confirm_res.orderDetailId);
			//Console.WriteLine("confirm_res.payType = {0}",confirm_res.payType);

			complete_flag = true;
		}

		public void ExecPayRequest2()
		{
			Console.WriteLine("Error E091");

			Console.WriteLine("Reverse");
			WbCPSReverseRequest reverse_request = new WbCPSReverseRequest();
			//reverse_request.orderId = pay_res.orderId;
			reverse_request.orderId = "OWC20161104181148726rpVQ";


			dev.EVRW.OutputCompleteEvent -= OnOutputCompleteEventPay;
			dev.EVRW.ErrorEvent -= OnErrorEventPay;
			dev.EVRW.OutputCompleteEvent += OnOutputCompleteEventPayReverse;
			dev.EVRW.ErrorEvent += OnErrorEventPayReverse;


			dev.ReverseRequest(reverse_request);
		}
		 void OnOutputCompleteEventPayReverse(int outputID)
		{
			//Encoding enc = Encoding.UTF8;

			//var serializer = new DataContractJsonSerializer(typeof(CPSPayResponse));

			//string st = "";
			Console.WriteLine("■■OnOutputCompleteEventReverse:{0}",outputID);
			
			WbCPSPayResponse pay_res = dev.PayResponse();
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
			
		}
		void OnErrorEventPayReverse(int resultCode,int resultCodeExtended,int errorLocus,ref int pErrorResponse){
			Console.WriteLine("■■ErorEventPayReverse: resultCode={0},resultCodeExteneded={1},errorLocus={2},pErrorResponse={3}",resultCode,resultCodeExtended,errorLocus,pErrorResponse);
			//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);
			dev.PrintInfo();
			WbCPSPayResponse pay_res = dev.PayResponse();
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

			if(resultCode == 106){
				Console.WriteLine("Error 106");
			}else if(resultCode == 111){
				Console.WriteLine("Error 111");
				if(resultCodeExtended == 111000){
					Console.WriteLine("ErrorSub 000");

					switch(pay_res.result.errorType){
						case 2:
							Console.WriteLine("Error E02");
							break;
						case 9:
							switch(pay_res.result.errorSubType){
								case 1:
									Console.WriteLine("Error E091");

									//Console.WriteLine("Reverse");
									//WbCPSReverseRequest reverse_request = new WbCPSReverseRequest();
									//reverse_request.orderId = pay_res.orderId;


									//dev.EVRW.OutputCompleteEvent -= OnOutputCompleteEventPay;
									//dev.EVRW.ErrorEvent -= OnErrorEventPay;

									//dev.ReverseRequest(reverse_request);
									
									break;
								case 2:
									Console.WriteLine("Error E092");
									break;
								case 3:
									Console.WriteLine("Error E093");
									break;
								default:
									Console.WriteLine("Error E09?");
									break;
							}
							break;
						default:
							Console.WriteLine("Error???");
							break;
					}
				}else{
					Console.WriteLine("ErrorSub other");
				}
			}else{
				Console.WriteLine("Error other");
			}
		}


        public void Refund()
        {
        }

		WbCPSReverseRequest pre_reverse_request;

        public void Reverse(WbCPSReverseRequest request)
        {
			pre_reverse_request =  request;

			dev.ReverseRequest(request);
        }
        public void ConfirmRequest(WbCPSConfirmRequest request)
        {
			dev.ConfirmRequest(request);
        }
        public void Deposit()
        {
        }


		//void OnOutputCompleteEvent(int outputID)
		//{
		//	//Encoding enc = Encoding.UTF8;

		//	//var serializer = new DataContractJsonSerializer(typeof(CPSPayResponse));

		//	//string st = "";
		//	Console.WriteLine("■■OnOutputCompleteEvent:{0}",outputID);


		//	switch(CurrentMethod) {
		//		case WbCPSMethod.Pay:
		//			WbCPSPayResponse pay_res = PayResponse();
		//			Console.WriteLine("pay_res.result.resultCode = {0}",pay_res.result.resultCode);
		//			Console.WriteLine("pay_res.result.errorType = {0}",pay_res.result.errorType);
		//			Console.WriteLine("pay_res.result.errorSubType = {0}",pay_res.result.errorSubType);
		//			Console.WriteLine("pay_res.result.errorCode = {0}",pay_res.result.errorCode);
		//			Console.WriteLine("pay_res.result.subErrorCode = {0}",pay_res.result.subErrorCode);
		//			Console.WriteLine("pay_res.result.errorInfo = {0}",pay_res.result.errorInfo);
		//			Console.WriteLine("pay_res.orderId = {0}",pay_res.orderId);
		//			Console.WriteLine("pay_res.orderDetailId = {0}",pay_res.orderDetailId);
		//			Console.WriteLine("pay_res.transTime = {0}",pay_res.transTime);
		//			Console.WriteLine("pay_res.currencyCode = {0}",pay_res.currencyCode);
		//			Console.WriteLine("pay_res.amount = {0}",pay_res.amount);
		//			Console.WriteLine("pay_res.transStatus = {0}",pay_res.transStatus);
		//			Console.WriteLine("pay_res.payType = {0}",pay_res.payType);
		//			break;
		//		case WbCPSMethod.CancelValue:
		//			WbCPSRefundResponse refund_res = RefundResponse();
		//			Console.WriteLine("refund_res.result.resultCode = {0}",refund_res.result.resultCode);
		//			Console.WriteLine("refund_res.result.errorType = {0}",refund_res.result.errorType);
		//			Console.WriteLine("refund_res.result.errorSubType = {0}",refund_res.result.errorSubType);
		//			Console.WriteLine("refund_res.result.errorCode = {0}",refund_res.result.errorCode);
		//			Console.WriteLine("refund_res.result.subErrorCode = {0}",refund_res.result.subErrorCode);
		//			Console.WriteLine("pay_res.result.errorInfo = {0}",pay_res.result.errorInfo);
		//			Console.WriteLine("refund_res.orderId = {0}",refund_res.orderId);
		//			Console.WriteLine("refund_res.orderDetailId = {0}",refund_res.orderDetailId);
		//			Console.WriteLine("refund_res.transStatus = {0}",refund_res.transStatus);
		//			Console.WriteLine("refund_res.currencyCode = {0}",refund_res.currencyCode);
		//			Console.WriteLine("refund_res.amount = {0}",refund_res.amount);
		//			Console.WriteLine("refund_res.transTime = {0}",refund_res.transTime);
		//			break;
		//		case WbCPSMethod.Reverse:
		//			WbCPSReverseResponse reverse_res = ReverseResponse();
		//			Console.WriteLine("reverse_res.result.resultCode = {0}",reverse_res.result.resultCode);
		//			Console.WriteLine("reverse_res.result.errorType = {0}",reverse_res.result.errorType);
		//			Console.WriteLine("reverse_res.result.errorSubType = {0}",reverse_res.result.errorSubType);
		//			Console.WriteLine("reverse_res.result.errorCode = {0}",reverse_res.result.errorCode);
		//			Console.WriteLine("reverse_res.result.subErrorCode = {0}",reverse_res.result.subErrorCode);
		//			Console.WriteLine("pay_res.result.errorInfo = {0}",pay_res.result.errorInfo);
		//			Console.WriteLine("reverse_res.orderId = {0}",reverse_res.orderId);
		//			Console.WriteLine("reverse_res.orderDetailI = {0}",reverse_res.orderDetailId);
		//			Console.WriteLine("reverse_res.transStatus = {0}",reverse_res.transStatus);
		//			Console.WriteLine("reverse_res.amount = {0}",reverse_res.amount);
		//			Console.WriteLine("reverse_res.currencyCode = {0}",reverse_res.currencyCode);
		//			Console.WriteLine("reverse_res.transTime = {0}",reverse_res.transTime);
		//			break;
		//		case WbCPSMethod.Confirm:
		//			WbCPSConfirmResponse confirm_res = ConfirmResponse();
		//			Console.WriteLine("confirm_res.result.resultCode = {0}",confirm_res.result.resultCode);
		//			Console.WriteLine("confirm_res.result.errorType = {0}",confirm_res.result.errorType);
		//			Console.WriteLine("confirm_res.result.errorSubType = {0}",confirm_res.result.errorSubType);
		//			Console.WriteLine("confirm_res.result.errorCode = {0}",confirm_res.result.errorCode);
		//			Console.WriteLine("confirm_res.result.subErrorCode = {0}",confirm_res.result.subErrorCode);
		//			Console.WriteLine("pay_res.result.errorInfo = {0}",pay_res.result.errorInfo);
		//			Console.WriteLine("confirm_res.transStatus = {0}",confirm_res.transStatus);
		//			Console.WriteLine("confirm_res.payCheckDate = {0}",confirm_res.payCheckDate);
		//			Console.WriteLine("confirm_res.transTime = {0}",confirm_res.transTime);
		//			Console.WriteLine("confirm_res.orderId = {0}",confirm_res.orderId);
		//			Console.WriteLine("confirm_res.orderDetailId = {0}",confirm_res.orderDetailId);
		//			Console.WriteLine("confirm_res.payType = {0}",confirm_res.payType);
		//			break;
		//		case WbCPSMethod.Deposit:
		//			WbCPSDepositResponse deposit_res = DepositResponse();
		//			Console.WriteLine("deposit_res.result.resultCode = {0}",deposit_res.result.resultCode);
		//			Console.WriteLine("deposit_res.result.errorType = {0}",deposit_res.result.errorType);
		//			Console.WriteLine("deposit_res.result.errorSubType = {0}",deposit_res.result.errorSubType);
		//			Console.WriteLine("deposit_res.result.errorCode = {0}",deposit_res.result.errorCode);
		//			Console.WriteLine("deposit_res.result.subErrorCode = {0}",deposit_res.result.subErrorCode);
		//			Console.WriteLine("pay_res.result.errorInfo = {0}",pay_res.result.errorInfo);
		//			Console.WriteLine("deposit_res.orderId = {0}",deposit_res.orderId);
		//			Console.WriteLine("deposit_res.orderDetailId = {0}",deposit_res.orderDetailId);
		//			Console.WriteLine("deposit_res.transTime = {0}",deposit_res.transTime);
		//			Console.WriteLine("deposit_res.currencyCode = {0}",deposit_res.currencyCode);
		//			Console.WriteLine("deposit_res.transStatus = {0}",deposit_res.transStatus);
		//			break;
		//		default:
		//			break;
		//	}

		//	CurrentMethod = WbCPSMethod.None;

		//	//EVRW.RetrieveResultInformation("result",ref st);
		//	//Console.WriteLine("result:{0}",st);
		//	//EVRW.RetrieveResultInformation("message",ref st);
		//	//Console.WriteLine("message:{0}",st);
		//	//EVRW.RetrieveResultInformation("transId",ref st);
		//	//Console.WriteLine("transID:{0}",st);
		//	//EVRW.RetrieveResultInformation("transTime",ref st);
		//	//Console.WriteLine("transTime:{0}",st);
		//	//EVRW.RetrieveResultInformation("amount",ref st);
		//	//Console.WriteLine("amount:{0}",st);
		//	//EVRW.RetrieveResultInformation("payType",ref st);
		//	//Console.WriteLine("payType:{0}",st);
		//	//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);

		//	//using(var ms = new MemoryStream())
		//	//using(var sw = new StreamWriter(ms))
		//	//{
		//	//	sw.Write(EVRW.AdditionalSecurityInformation);
		//	//	sw.Flush();
		//	//	//using(var ms = new MemoryStream(enc.GetBytes(EVRW.AdditionalSecurityInformation))) {

		//	//	ms.Position = 0;
		//	//	var pay_response = serializer.ReadObject(ms) as CPSPayResponse;

		//	//	Console.WriteLine("RESPONSE:pay_response.meta.code = {0}",pay_response.meta.code);
		//	//	Console.WriteLine("RESPONSE:pay_response.meta.message = {0}",pay_response.meta.message);
		//	//	Console.WriteLine("RESPONSE:pay_response.data.errorCode = {0}",pay_response.data.errorCode);
		//	//	Console.WriteLine("RESPONSE:pay_response.data.errorInfo = {0}",pay_response.data.errorInfo);
		//	//	Console.WriteLine("RESPONSE:pay_response.data.subErrorCode = {0}",pay_response.data.subErrorCode);
		//	//	Console.WriteLine("RESPONSE:pay_response.data.sign = {0}",pay_response.data.sign);
		//	//	Console.WriteLine("RESPONSE:pay_response.data.result.orderDetailId = {0}",pay_response.data.result.orderDetailId);
		//	//	Console.WriteLine("RESPONSE:pay_response.data.result.orderId = {0}",pay_response.data.result.orderId);
		//	//	Console.WriteLine("RESPONSE:pay_response.data.result.transTime = {0}",pay_response.data.result.transTime);
		//	//	Console.WriteLine("RESPONSE:pay_response.data.result.currencyCode = {0}",pay_response.data.result.currencyCode);
		//	//	Console.WriteLine("RESPONSE:pay_response.data.result.amount = {0}",pay_response.data.result.amount);
		//	//	Console.WriteLine("RESPONSE:pay_response.data.result.amountRmb = {0}",pay_response.data.result.amountRmb);
		//	//	Console.WriteLine("RESPONSE:pay_response.data.result.transStatus = {0}",pay_response.data.result.transStatus);
		//	//	Console.WriteLine("RESPONSE:pay_response.data.result.payType = {0}",pay_response.data.result.payType);
		//	//}

		//}
		//void OnErrorEvent(int resultCode,int resultCodeExtended,int errorLocus,ref int pErrorResponse)
		//{
		//	Console.WriteLine("■■ErorEvent: resultCode={0},resultCodeExteneded={1},errorLocus={2},pErrorResponse={3}",resultCode,resultCodeExtended,errorLocus,pErrorResponse);
		//	//Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);
		//	PrintInfo();
		//	switch(CurrentMethod) {
		//		case WbCPSMethod.Pay:
		//			//WbCPSPayResponse pay_res = PayResponse();// やっぱりエラーの時はこれは動かない。なぜならjSONが入っていないから
		//			//Console.WriteLine("pay_res.result.resultCode = {0}",pay_res.result.resultCode);
		//			//Console.WriteLine("pay_res.result.errorType = {0}",pay_res.result.errorType);
		//			//Console.WriteLine("pay_res.result.errorSubType = {0}",pay_res.result.errorSubType);
		//			//Console.WriteLine("pay_res.result.errorCode = {0}",pay_res.result.errorCode);
		//			//Console.WriteLine("pay_res.result.subErrorCode = {0}",pay_res.result.subErrorCode);
		//			//Console.WriteLine("pay_res.orderId = {0}",pay_res.orderId);
		//			//Console.WriteLine("pay_res.orderDetailId = {0}",pay_res.orderDetailId);
		//			//Console.WriteLine("pay_res.transTime = {0}",pay_res.transTime);
		//			//Console.WriteLine("pay_res.currencyCode = {0}",pay_res.currencyCode);
		//			//Console.WriteLine("pay_res.amount = {0}",pay_res.amount);
		//			//Console.WriteLine("pay_res.transStatus = {0}",pay_res.transStatus);
		//			//Console.WriteLine("pay_res.payType = {0}",pay_res.payType);
		//			break;
		//		//	case WbCPSMethod.Pay:
		//		//		WbCPSPayResponse pay_res = PayResponse();// やっぱりエラーの時はこれは動かない。なぜならjSONが入っていないから
		//		//		Console.WriteLine("pay_res.result.resultCode = {0}",pay_res.result.resultCode);
		//		//		Console.WriteLine("pay_res.result.errorType = {0}",pay_res.result.errorType);
		//		//		Console.WriteLine("pay_res.result.errorSubType = {0}",pay_res.result.errorSubType);
		//		//		Console.WriteLine("pay_res.result.errorCode = {0}",pay_res.result.errorCode);
		//		//		Console.WriteLine("pay_res.result.subErrorCode = {0}",pay_res.result.subErrorCode);
		//		//		Console.WriteLine("pay_res.orderId = {0}",pay_res.orderId);
		//		//		Console.WriteLine("pay_res.orderDetailId = {0}",pay_res.orderDetailId);
		//		//		Console.WriteLine("pay_res.transTime = {0}",pay_res.transTime);
		//		//		Console.WriteLine("pay_res.currencyCode = {0}",pay_res.currencyCode);
		//		//		Console.WriteLine("pay_res.amount = {0}",pay_res.amount);
		//		//		Console.WriteLine("pay_res.transStatus = {0}",pay_res.transStatus);
		//		//		Console.WriteLine("pay_res.payType = {0}",pay_res.payType);
		//		//		break;
		//		//	case WbCPSMethod.CancelValue:
		//		//		ResultCancelValue();
		//		//		break;
		//		//	case WbCPSMethod.Reverse:
		//		//		ReverseResponse();
		//		//		break;
		//		case WbCPSMethod.Confirm:
		//			WbCPSConfirmResponse confirm_res = ConfirmResponse();
		//			Console.WriteLine("confirm_res.result.resultCode = {0}",confirm_res.result.resultCode);
		//			Console.WriteLine("confirm_res.result.errorType = {0}",confirm_res.result.errorType);
		//			Console.WriteLine("confirm_res.result.errorSubType = {0}",confirm_res.result.errorSubType);
		//			Console.WriteLine("confirm_res.result.errorCode = {0}",confirm_res.result.errorCode);
		//			Console.WriteLine("confirm_res.result.subErrorCode = {0}",confirm_res.result.subErrorCode);
		//			Console.WriteLine("pay_res.result.errorInfo = {0}",pay_res.result.errorInfo);
		//			Console.WriteLine("confirm_res.transStatus = {0}",confirm_res.transStatus);
		//			Console.WriteLine("confirm_res.payCheckDate = {0}",confirm_res.payCheckDate);
		//			Console.WriteLine("confirm_res.transTime = {0}",confirm_res.transTime);
		//			Console.WriteLine("confirm_res.orderId = {0}",confirm_res.orderId);
		//			Console.WriteLine("confirm_res.orderDetailId = {0}",confirm_res.orderDetailId);
		//			Console.WriteLine("confirm_res.payType = {0}",confirm_res.payType);
		//			break;
		//		//	case WbCPSMethod.Deposit:
		//		//		ResultDeposit();
		//		//		break;
		//		default:
		//			break;
		//	}
		//}
		//void OnDataEvent(int status)
		//{
		//	Console.WriteLine("■■DataEvent:{0}",status);
		//	Console.WriteLine("AdditionalSecurityInformation:{0}",EVRW.AdditionalSecurityInformation);
		//	PrintInfo();
		//}
	}
}
