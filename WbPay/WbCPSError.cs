using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WbPay
{
	class WbCPSError
	{
		internal const int CPS_NETWORK			= 2;
		internal const int LIB					= 5;
		internal const int POST_PROCESS			= 6;
		internal const int CPS					= 9;

		internal class Lib
		{
			internal const int PARAMETER			= 1;
			internal const int HTTP					= 2;
			internal const int REQUEST				= 3;
			internal const int RESPONSE				= 4;
			internal const int PROGRAM				= 9;

			internal class Parameter
			{
				internal const int CONFIG_NOT_FOUND		= 1;
				internal const int ILLEGAL_CONFIG_VALUE	= 2;
				internal const int PARAMETER_REQUIRED	= 3;
				internal const int ILLEGAL_PARAMETER	= 4;
				internal const int UNKOWN				= 99;
			}
			internal class Http
			{
				internal const int POST					= 1;
				internal const int UNKOWN				= 99;
			}
			internal class Request
			{
				internal const int CREATE_SIGNATURE		= 1;
				internal const int CREATE_CONTENT		= 2;
				internal const int CREATE_URL			= 3;
				internal const int REQUEST_DUPLICATE	= 4;
				internal const int ILLEGAL_PAYCONFIRM	= 5;
			}
			internal class Response
			{
				internal const int PARSE_CONTENT		= 1;
				internal const int PARSE_SIGNATURE		= 2;
				internal const int VERIFY_SIGNATURE		= 3;
				internal const int ILLEGAL_PARAMETER	= 4;
				internal const int UNKNOWN				= 99;
			}
			internal class Program
			{
				internal const int PROGRAM_ERROR		= 1;
				internal const int UNKOWN				= 99;
			}
		}
		internal class PostProcess
		{
			internal const int STYPE				= 0;

			static Dictionary<int, string> transStatusMessgeDict = new Dictionary<int, string>(){
					{ 0,"支払成功"},
					{ 1,"支払失敗"},
					{ 2,"支払待ち"},
					{ 3,"支払中"},
					{ 4,"支払タイムアウト"},
					{ 5,"返金中"},
					{ 6,"返金済"},
					{ 7,"返金失敗"},
					{ 8,"返金タイムアウト"},
					{ 9,"取消中"},
					{10,"取消成功"},
					{11,"取消失敗"},
					{12,"取消タイムアウト"},
					{13,"オーダクローズ"},
					{14,"顧客支払待ち"},
					{15,"顧客支払中止"},
					{16,"入金中"},
					{17,"入金タイムアウト"},
					{18,"入金成功"},
					{19,"入金失敗"},
					{20,"返金取消中"},
					{21,"返金取消成功"},
					{22,"返金取消失敗"},
					{23,"返金取消タイムアウト"},
					{24,"入金取消中"},
					{25,"入金取消成功"},
					{26,"入金取消失敗"},
					{27,"入金取消タイムアウト"},
			};
		
			static internal string GetErrorMessage(int status) {
				string mess;
				if(transStatusMessgeDict.TryGetValue(status,out mess)){
					return mess;
				}else{
					return "不明のエラー";
				}
			}
		}
		internal class Cps
		{
			internal const int PARAMETER			= 1;
			internal const int SYSTEM				= 2;
			internal const int PROCESSOR			= 3;
			internal const int SECURITY				= 9;

			internal class Processor
			{
				internal const int CODE					= 1;

				internal const string PAYMENT_IN_PROGRESS	= "PAYMENT_IN_PROGRESS";
				internal const string REQUEST_DUPLICATED	= "REQUEST_DUPLICATED";
				internal const string TRADE_NOT_EXIST		= "TRADE_NOT_EXIST";
				internal const string TRANSACTION_NOT_FOUND = "TRANSACTION_NOT_FOUND";

				static Dictionary<string,WbCPSAction> subCodeDict = new Dictionary<string,WbCPSAction>(){
						{"ACCOUNT_REMAINS_CHANGED"		,WbCPSAction.Customer},
						{"ACOUNT_ERROR"					,WbCPSAction.Customer},
						{"AMOUNT_NOT_MATCH"				,WbCPSAction.CPSCall},
						{"BALANCE_LIMIT_OVER"			,WbCPSAction.Customer},
						{"BLACK_LIST_CARD"				,WbCPSAction.Customer},
						{"CARD_AUTH_ERROR"				,WbCPSAction.Customer},
						{"CARD_CHANGED"					,WbCPSAction.Customer},
						{"CARD_DECLINED"				,WbCPSAction.Customer},
						{"CARD_EXPIRED"					,WbCPSAction.Customer},
						{"CARD_LIMIT_EXCEEDED"			,WbCPSAction.Customer},
						{"CARD_NOT_ACTIVE"				,WbCPSAction.Customer},
						{"CARD_NOT_ENOUGH"				,WbCPSAction.Customer},
						{"CARD_PAY_ERROR"				,WbCPSAction.Customer},
						{"CARD_REPORT_STOLEN"			,WbCPSAction.Customer},
						{"CARD_TEMP_ERROR"				,WbCPSAction.Customer},
						{"CARD_TEMP_SUSPENDED"			,WbCPSAction.Customer},
						{"CARD_USE_SUSPENDED"			,WbCPSAction.Customer},
						{"DEPOSIT_MAX_PAYMENT_LIMITS_OVER"	,WbCPSAction.Customer},
						{"DEPOSIT_NOT_SUPPORT"			,WbCPSAction.Customer},
						{"INVALID_AMOUNT"				,WbCPSAction.Customer},
						{"INVALID_CARD"					,WbCPSAction.Customer},
						{"INVALID_CARD_PAY_INFO"		,WbCPSAction.Customer},
						{"INVALID_CURRENCY"				,WbCPSAction.CPSCall},
						{"INVALID_CVN"					,WbCPSAction.Customer},
						{"INVALID_FORMAT"				,WbCPSAction.CPSCall},
						{"INVALID_HEAD"					,WbCPSAction.CPSCall},
						{"INVALID_ONETIME_KEY"			,WbCPSAction.Customer},
						{"INVALID_PARAM"				,WbCPSAction.CPSCall},
						{"INVALID_PIN_CODE"				,WbCPSAction.Customer},
						{"INVALID_REQUEST"				,WbCPSAction.CPSCall},
						{"INVALID_RETRY"				,WbCPSAction.CPSCall},
						{"INVALID_STATUS"				,WbCPSAction.Customer},
						{"INVALID_TRANSACTION"			,WbCPSAction.CPSCall},
						{"INVALID_VERSION"				,WbCPSAction.CPSCall},
						{"INVALID_VOLUME"				,WbCPSAction.CPSCall},
						{"LACK_CARD_INFO"				,WbCPSAction.Customer},
						{"MERCHANT_NOT_EXIST"			,WbCPSAction.CPSCall},
						{"NO_ACCOUNT"					,WbCPSAction.Customer},
						{"NO_AVAILABLE_CARD"			,WbCPSAction.Customer},
						{"NO_AVAILABLE_VALUE"			,WbCPSAction.Customer},
						{"NO_PAYMENT_INFO"				,WbCPSAction.Customer},
						{"NOT_ENOUGH"					,WbCPSAction.Customer},
						{@"NOT_ENOUGH\\\\\"				,WbCPSAction.Customer},
						{"NOT_USER"						,WbCPSAction.Customer},
						{"ONETIME_KEY_EXPIRED"			,WbCPSAction.Customer},
						{"ORDER_ID_USED"				,WbCPSAction.CPSCall},
						{"PAY_LIMIT_EXCEEDED"			,WbCPSAction.Customer},
						{"PAY_NOT_ALLOWED"				,WbCPSAction.Customer},
						{"PAYMENT_DECLINED"				,WbCPSAction.Customer},
						{PAYMENT_IN_PROGRESS			,WbCPSAction.CPSCall},
						{"PAYMENT_NOT_ALLOWED"			,WbCPSAction.CPSCall},
						{"REFUND_AMOUNT_EXCEEDED"		,WbCPSAction.Customer},
						{"REFUND_EXPIRE"				,WbCPSAction.Customer},
						{REQUEST_DUPLICATED				,WbCPSAction.CPSCall},
						{"RETRY_AGAIN_LATER"			,WbCPSAction.CPSCall},
						{"REVERSE_EXPIRE"				,WbCPSAction.Customer},
						{"SERVICE_NOT_SUPPORT"			,WbCPSAction.Customer},
						{"SETTING_LIMIT_OVER"			,WbCPSAction.Customer},
						{"SYSTEM_ERROR"					,WbCPSAction.CPSCall},
						{"TERM_NOT_SAME"				,WbCPSAction.CPSCall},
						{TRADE_NOT_EXIST				,WbCPSAction.CPSCall},
						{"TRANSACION_ALREADY_REFUND"	,WbCPSAction.Customer},
						{"TRANSACTION_ALREADY_MADE"		,WbCPSAction.CPSCall},
						{"TRANSACTION_ERROR"			,WbCPSAction.Customer},
						{"TRANSACTION_MAX_EXCEEDED"		,WbCPSAction.CPSCall},
						{TRANSACTION_NOT_FOUND			,WbCPSAction.CPSCall},
						{"UNDER_MAINTENANCE"			,WbCPSAction.Customer},
						{"USER_SUSPENDED_TRANSACTION"	,WbCPSAction.Customer},
						{"UNKNOWNERROR"					,WbCPSAction.CPSCall},
				};

				static internal WbCPSAction GetRequsetAction(string code){
					WbCPSAction	request;
					try{
						if(subCodeDict.TryGetValue(code,out request)){
							return request;
						}else{
							string new_code = convSubCode(code);
							if(subCodeDict.TryGetValue(new_code,out request)){
								return request;
							}
						}
					}catch(Exception){
					}
					return WbCPSAction.CPSCall;
				}

				static StringBuilder workConvSubCode = new StringBuilder();
				static string convSubCode(string from){
					workConvSubCode.Clear();
					for(int i = 0;i < from.Length;i++){
						Char c = from[i];
						if(Char.IsLetter(c) || c == '_') workConvSubCode.Append(c);
					}
					return workConvSubCode.ToString();
				}		

			}
			
		}
	}
}
