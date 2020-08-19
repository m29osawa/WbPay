using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WbPay
{
	public enum WbCPSPayType
	{
		AutoDetect,
		LinePay,				// LINE Pay
		RegionalElectricMony,	// 地域電子マネー
		OrigamiPay,				// 
		RakutenPay,				// 
		PayPay,					// 
		dPayment,				// d払い
		CpayAlipay,				// 
		CpayWechat,				// 
		MerPay,					// 
		AuPay,					// 
		JCBAliPay,				// 
		QUOCardPay,				// 
		YuchoPay,				// 
		KPlus,					// 
		ResonaWallet,			// 
		EPOSPay,				// 
		JCoinPay,				// 
		pring,					// 
		atone,					// 
		aftee,					// 
		BankPay,				// 
		OnepayAlipay,			// 
		OnepayWechat,			// 
		OnepayGNAlipay,			// 
		OnepayGNAlipayHK,		// 
		OnepayGNkakaopay,		// 
		SmartCode,				// 
	};
	public enum WbCPSCurrencyType
	{
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
	public enum WbCPSValueType
	{
		Basic,
		Bonus,
		Coupon,
		Point,
	}
	public enum WbCPSAction
	{
		VendorCall,	// アプリケーションベンダー
		CPSCall,	// CPSコールセンター
		Customer,	// 顧客
	}

	public enum WbCPSRequestType
	{
		Pay,		// 支払
		Refund,		// 返金
		Reverse,	// 取消
		Confirm,	// 確認
		Deposit,	// 入金
	}

	public class WbCPSResponseError
	{
		public int					Type;
		public string				Code;
		public string				SubCode;
		public string				Message;
		public WbCPSAction			RequestAction;
	}

	public class WbCPSPayRequest
	{
		public WbCPSPayType			PayType;
		public decimal				Amount;
		public string				ReceiptNo;
		public string				UserCode;
		public string				Remark;
	}
	public class WbCPSPayResponse
	{
		public string				OrderId;
		public string				TransTime;
		public decimal				Amount;
		public int					TransStatus;
		public WbCPSPayType			PayType;
	}
	public class WbCPSRefundRequest
	{
		public decimal				RefundAmount;
		public string				OrderId;
		public string				RefundReason;
		public string				Remark;
	}
	public class WbCPSRefundResponse
	{
		public string				OrderId;
		public int					TransStatus;
		public decimal				RefundAmount;
		public string				TransTime;
	}
	public class WbCPSReverseRequest
	{
		public string				OrderId;
	}
	public class WbCPSReverseResponse
	{
		public string				OrderId;
		public int					TransStatus;
		public decimal				Amount;
		public string				TransTime;
	}
	public class WbCPSConfirmRequest
	{
		public string				OrderId;
	}
	public class WbCPSConfirmResponse
	{
		public int					TransStatus;
		public string				PayCheckDate;
		public string				TransTime;
		public string				OrderId;
		public WbCPSPayType			PayType;
	}
	public class WbCPSDepositRequest
	{
		public WbCPSPayType			PayType;
		public WbCPSValueType		ValueType;
		public decimal				Amount;
		public string				ReceiptNo;
		public string				UserCode;
	}
	public class WbCPSDepositResponse
	{
		public string				OrderId;
		public string				TransTime;
		public decimal				Amount;
		public int					TransStatus;
	}

	class WbCPSErrorResult
	{
		internal int					Type;
		internal int					SubType;
		internal int					Code;
		internal string					SubCode = "";
		internal string					Message = "";
	}
	class WbCPSPayResult
	{
		internal bool					IsSuccess;
		internal WbCPSErrorResult		Error;
		internal WbCPSPayResponse		Response;
		internal string					Token;
	}
	class WbCPSRefundResult
	{
		internal bool					IsSuccess;
		internal WbCPSErrorResult		Error;
		internal WbCPSRefundResponse	Response;
		internal string					Token;
	}
	class WbCPSReverseResult
	{
		internal bool					IsSuccess;
		internal WbCPSErrorResult		Error;
		internal WbCPSReverseResponse	Response;
		internal string					Token;
	}
	class WbCPSConfirmResult
	{
		internal bool					IsSuccess;
		internal WbCPSErrorResult		Error;
		internal WbCPSConfirmResponse	Response;
		internal string					Token;
	}
	class WbCPSDepositResult
	{
		internal bool					IsSuccess;
		internal WbCPSErrorResult		Error;
		internal WbCPSDepositResponse	Response;
		internal string					Token;
	}
}
