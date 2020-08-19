using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WbPay
{
	class WbCPSRawCode
	{
		static Dictionary<WbCPSPayType, string> dictPayCode
									= new Dictionary<WbCPSPayType, string>(){
			{WbCPSPayType.LinePay,				"04" },
			{WbCPSPayType.RegionalElectricMony,	"05" },
			{WbCPSPayType.OrigamiPay,			"07" },
			{WbCPSPayType.RakutenPay,			"09" },
			{WbCPSPayType.PayPay,				"10" },
			{WbCPSPayType.dPayment,				"11" },
			{WbCPSPayType.CpayAlipay,			"13" },
			{WbCPSPayType.CpayWechat,			"14" },
			{WbCPSPayType.MerPay,				"15" },
			{WbCPSPayType.AuPay,				"16" },
			{WbCPSPayType.JCBAliPay,			"17" },
			{WbCPSPayType.QUOCardPay,			"18" },
			{WbCPSPayType.YuchoPay,				"19" },
			{WbCPSPayType.KPlus,				"21" },
			{WbCPSPayType.ResonaWallet,			"22" },
			{WbCPSPayType.EPOSPay,				"23" },
			{WbCPSPayType.JCoinPay,				"24" },
			{WbCPSPayType.pring,				"27" },
			{WbCPSPayType.atone,				"28" },
			{WbCPSPayType.aftee,				"29" },
			{WbCPSPayType.BankPay,				"30" },
			{WbCPSPayType.OnepayAlipay,			"O01" },
			{WbCPSPayType.OnepayWechat,			"O02" },
			{WbCPSPayType.OnepayGNAlipay,		"GN01" },
			{WbCPSPayType.OnepayGNAlipayHK,		"GN02" },
			{WbCPSPayType.OnepayGNkakaopay,		"GN03" },
			{WbCPSPayType.SmartCode,			"90" },

			{WbCPSPayType.AutoDetect,			"99" },
		};
		public static string GetPayCode(WbCPSPayType paytype){
			return dictPayCode[paytype];
		}
		public static WbCPSPayType GetPayType(string st) {
			if(st ==  null) throw new ArgumentNullException("payType");

			foreach(KeyValuePair<WbCPSPayType, string> kvp in dictPayCode) {
				if(kvp.Value == st) return kvp.Key;
			}
			throw new ArgumentException("payType");
		}

		static Dictionary<WbCPSCurrencyType, string> dictCurrencyCode
						= new Dictionary<WbCPSCurrencyType, string>(){
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
		public static string GetCurrencyCode(WbCPSCurrencyType currencytype){
			return dictCurrencyCode[currencytype];
		}
		public static WbCPSCurrencyType GetCurrencyType(string st) {
			if(st == null) throw new ArgumentNullException("currencyCode");

			foreach(KeyValuePair<WbCPSCurrencyType, string> kvp in dictCurrencyCode) {
				if(kvp.Value == st) return kvp.Key;
			}
			throw new ArgumentException("currenecyCode");
		}

		static Dictionary<WbCPSValueType, string> dictValueCode
							= new Dictionary<WbCPSValueType, string>(){
					{WbCPSValueType.Basic,	"A"},
					{WbCPSValueType.Bonus,	"B"},
					{WbCPSValueType.Coupon,	"C"},
					{WbCPSValueType.Point,	"D"},
		};
		public static string GetValueCode(WbCPSValueType valuetype) {
			return dictValueCode[valuetype];
		}
		public static WbCPSValueType GetValueType(string st) {
			if(st == null) throw new ArgumentException("valueCode");

			foreach(KeyValuePair<WbCPSValueType, string> kvp in dictValueCode) {
				if(kvp.Value == st) return kvp.Key;
			}
			throw new ArgumentException("valueCode");
		}
	}
}
