using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;

namespace MakeCPSServerResponse
{
	class Program
	{
		static void Main(string[] args) {
			MakePayResponse();
			//MakeRefundResponse();
			//MakeReverseResponse();
			//MakeConfirmResponse();
			//MakeDepositResponse();
		}
        static void MakePayResponse()
        {
            WbCPSRawPayResponseV1 response = new WbCPSRawPayResponseV1();
            response.meta = new WbCPSRawResponseMeta();
            response.data = new WbCPSRawPayResponseDataV1();
            response.data.result = new WbCPSRawPayResponseResultV1();

			//標準
			response.meta.code = "00";
			response.meta.message = "SUCCESS";
			response.data.errorCode = "";
			response.data.errorInfo = "";
			response.data.subErrorCode = "";
			response.data.sign = null;
			response.data.result.orderDetailId = "J1WP20161103154004945P2J";
			response.data.result.orderId = "OWC20161104181148726rpVQ";
			response.data.result.transTime = "20160926154010";
			response.data.result.currencyCode = "JPY";
			response.data.result.amount = "1000";
			response.data.result.amountRmb = null;
			response.data.result.transStatus = "00";
			response.data.result.payType = "04";

			//顧客支払待ち
			//response.meta.code = "00";
			//response.meta.message = "SUCCESS";
			//response.data.errorCode = "";
			//response.data.errorInfo = "";
			//response.data.subErrorCode = "";
			//response.data.sign = null;
			//response.data.result.orderDetailId = "J1WP20161103154004945P2J";
			//response.data.result.orderId = "OWC20161104181148726rpVQ";
			//response.data.result.transTime = "20160926154010";
			//response.data.result.currencyCode = "JPY";
			//response.data.result.amount = "1000";
			//response.data.result.amountRmb = null;
			//response.data.result.transStatus = "14";
			//response.data.result.payType = "04";

			//顧客支払中止
			//response.meta.code = "00";
			//response.meta.message = "SUCCESS";
			//response.data.errorCode = "";
			//response.data.errorInfo = "";
			//response.data.subErrorCode = "";
			//response.data.sign = null;
			//response.data.result.orderDetailId = "J1WP20161103154004945P2J";
			//response.data.result.orderId = "OWC20161104181148726rpVQ";
			//response.data.result.transTime = "20160926154010";
			//response.data.result.currencyCode = "JPY";
			//response.data.result.amount = "1000";
			//response.data.result.amountRmb = null;
			//response.data.result.transStatus = "15";
			//response.data.result.payType = "04";

			//通信異常
			//response.meta.code = "02";
			//response.meta.message = "FAILURE";
			//response.data.errorCode = "E02001";
			//response.data.errorInfo = "通信エラーが発生しました。";
			//response.data.subErrorCode = "";
			//response.data.sign = null;

			//パラメーターエラー
			//response.meta.code = "09";
			//response.meta.message = "FAILURE";
			//response.data.errorCode = "E09101";
			//response.data.errorInfo = "リクエストパラメータ異常、＊＊＊が入力されていません。";
			//response.data.subErrorCode = "";
			//response.data.sign = null;

			//残高不足
			//response.meta.code = "09";
			//response.meta.message = "FAILURE";
			//response.data.errorCode = "E09301";
			//response.data.errorInfo = "残高不足";
			//response.data.subErrorCode = "NOT_ENOUGH";
			//response.data.sign = null;

			//残高不足XXXX
			//response.meta.code = "09";
			//response.meta.message = "FAILURE";
			//response.data.errorCode = "E09301";
			//response.data.errorInfo = "残高不足";
			//response.data.subErrorCode = @"NOT_ENOUGH \1000";
			//response.data.sign = null;

			//決済中です。
			//response.meta.code = "09";
			//response.meta.message = "FAILURE";
			//response.data.errorCode = "E09301";
			//response.data.errorInfo = "決済中です。";
			//response.data.subErrorCode = "PAYMENT_IN_PROGRESS";
			//response.data.sign = null;


			//pay.orders.Add(new PayResponseOrder(){aa ="AA1",bb = "BB1"});
			//pay.orders.Add(new PayResponseOrder(){aa ="AA2",bb = "BB2"});
			response.data.sign = Signiture.GetSigniture(response);

            using (var ms = new MemoryStream())
            using (var sr = new StreamReader(ms))
            {
                var serializer = new DataContractJsonSerializer(typeof(WbCPSRawPayResponseV1));
                serializer.WriteObject(ms, response);
                ms.Position = 0;

                var json = sr.ReadToEnd();

                Console.WriteLine($"{json}");

                using(var fs = new StreamWriter("PayData.txt")){
                    fs.Write(json);
                }
            }


		}
        static void MakeRefundResponse()
        {
            WbCPSRawRefundResponse response = new WbCPSRawRefundResponse();
            response.meta = new WbCPSRawResponseMeta();
            response.data = new WbCPSRawRefundResponseData();
            response.data.result = new WbCPSRawRefundResponseResult();

            //// 返金済
            //response.meta.code = "00";
            //response.meta.message = "SUCCESS";
            //response.data.errorCode = "";
            //response.data.errorInfo = "";
            //response.data.subErrorCode = "";
            //response.data.sign = null;
            //response.data.result.orderId = "OWC20161104181148726rpVQ";
            //response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            //response.data.result.transStatus = "06";
            //response.data.result.currencyCode = "JPY";
            //response.data.result.refundAmount = "1000";
            //response.data.result.transTime = "20160926154010";

            // E02002 外部システムとの接続にオーバータイムが発生しました。
            response.meta.code = "02";
            response.meta.message = "FAILURE";
            response.data.errorCode = "E02002";
            response.data.errorInfo = "外部システムとの接続にオーバータイムが発生しました。";
            response.data.subErrorCode = "";
            response.data.sign = null;
            response.data.result = null;
            //response.data.result.orderId = "OWC20161104181148726rpVQ";
            //response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            //response.data.result.transStatus = "06";
            //response.data.result.currencyCode = "JPY";
            //response.data.result.refundAmount = "1000";
            //response.data.result.transTime = "20160926154010";

            //// E09128 返金金額が不正です。
            //response.meta.code = "09";
            //response.meta.message = "FAILURE";
            //response.data.errorCode = "E09128";
            //response.data.errorInfo = "返金金額が不正です。";
            //response.data.subErrorCode = "";
            //response.data.sign = null;
            //response.data.result = null;
            ////response.data.result.orderId = "OWC20161104181148726rpVQ";
            ////response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            ////response.data.result.transStatus = "06";
            ////response.data.result.currencyCode = "JPY";
            ////response.data.result.refundAmount = "1000";
            ////response.data.result.transTime = "20160926154010";





            //pay.orders.Add(new PayResponseOrder(){aa ="AA1",bb = "BB1"});
            //pay.orders.Add(new PayResponseOrder(){aa ="AA2",bb = "BB2"});
            response.data.sign = Signiture.GetSigniture(response);

            using (var ms = new MemoryStream())
            using (var sr = new StreamReader(ms))
            {
                var serializer = new DataContractJsonSerializer(typeof(WbCPSRawRefundResponse));
                serializer.WriteObject(ms, response);
                ms.Position = 0;

                var json = sr.ReadToEnd();

                Console.WriteLine($"{json}");

                using(var fs = new StreamWriter("RefundData.txt")){
                    fs.Write(json);
                }
            }
        }
        static void MakeReverseResponse()
        {
            WbCPSRawReverseResponse response = new WbCPSRawReverseResponse();
            response.meta = new WbCPSRawResponseMeta();
            response.data = new WbCPSRawReverseResponseData();
            response.data.result = new WbCPSRawReverseResponseResult();

            // 成功
            //response.meta.code = "00";
            //response.meta.message = "SUCCESS";
            //response.data.errorCode = "";
            //response.data.errorInfo = "";
            //response.data.subErrorCode = "";
            //response.data.sign = null;
            //response.data.result.orderId = "OWC20161104181148726rpVQ";
            //response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            //response.data.result.transStatus = "10";
            //response.data.result.amount = "1000";
            //response.data.result.currencyCode = "JPY";
            //response.data.result.transTime = "20160926154010";

            // 取消中
            response.meta.code = "00";
            response.meta.message = "SUCCESS";
            response.data.errorCode = "";
            response.data.errorInfo = "";
            response.data.subErrorCode = "";
            response.data.sign = null;
            response.data.result.orderId = "OWC20161104181148726rpVQ";
            response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            response.data.result.transStatus = "09";
            response.data.result.amount = "1000";
            response.data.result.currencyCode = "JPY";
            response.data.result.transTime = "20160926154010";

            //// E09135 エラー取消トランザクションが返金できません。
            //response.meta.code = "09";
            //response.meta.message = "FAILURE";
            //response.data.errorCode = "E09135";
            //response.data.errorInfo = "取消トランザクションが返金できません。";
            //response.data.subErrorCode = "";
            //response.data.sign = null;
            //response.data.result = null;
            ////response.data.result.orderId = "OWC20161104181148726rpVQ";
            ////response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            ////response.data.result.transStatus = "10";
            ////response.data.result.amount = "1000";
            ////response.data.result.currencyCode = "JPY";
            ////response.data.result.transTime = "20160926154010";

            // E02001 通信エラーが発生しました。
            //response.meta.code = "02";
            //response.meta.message = "FAILURE";
            //response.data.errorCode = "E02001";
            //response.data.errorInfo = "通信エラーが発生しました。";
            //response.data.subErrorCode = "";
            //response.data.sign = null;
            //response.data.result = null;

            response.data.sign = Signiture.GetSigniture(response);

            using (var ms = new MemoryStream())
            using (var sr = new StreamReader(ms))
            {
                var serializer = new DataContractJsonSerializer(typeof(WbCPSRawReverseResponse));
                serializer.WriteObject(ms, response);
                ms.Position = 0;

                var json = sr.ReadToEnd();

                Console.WriteLine($"{json}");

                using(var fs = new StreamWriter("ReverseData.txt")){
                    fs.Write(json);
                }
            }
        }
		static void	MakeConfirmResponse()
		{
			WbCPSRawConfirmResponseV1 response = new WbCPSRawConfirmResponseV1();
			response.meta =	new	WbCPSRawResponseMeta();
			response.data =	new	WbCPSRawConfirmResponseDataV1();
			response.data.result = new WbCPSRawConfirmResponseResultV1();

            ////正常
            //response.meta.code = "00";
            //response.meta.message	= "SUCCESS";
            //response.data.errorCode =	"";
            //response.data.errorInfo =	"";
            //response.data.subErrorCode = "";
            //response.data.sign = null;
            //response.data.result.transStatus = "00";
            //response.data.result.payCheckDate	= "20160926";
            //response.data.result.transTime = "20160926154010";
            //response.data.result.orderId = "OWC20161104181148726rpVQ";
            //response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            //response.data.result.payType = "04";

            //顧客支払待ち
            response.meta.code = "00";
            response.meta.message = "SUCCESS";
            response.data.errorCode = "";
            response.data.errorInfo = "";
            response.data.subErrorCode = "";
            response.data.sign = null;
            response.data.result.transStatus = "14";
            response.data.result.payCheckDate = "20160926";
            response.data.result.transTime = "20160926154010";
            response.data.result.orderId = "OWC20161104181148726rpVQ";
            response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            response.data.result.payType = "04";

            ////E02001通信エラーが発生しました
            //response.meta.code = "02";
            //response.meta.message = "FAILURE";
            //response.data.errorCode = "E02001";
            //response.data.errorInfo = "通信エラーが発生しました。";
            //response.data.subErrorCode = "";
            //response.data.sign = null;
            //response.data.result = null;

            ////支払失敗
            //response.meta.code = "00";
            //response.meta.message	= "SUCCESS";
            //response.data.errorCode =	"";
            //response.data.errorInfo =	"";
            //response.data.subErrorCode = "";
            //response.data.sign = null;
            //response.data.result.transStatus = "01";
            //response.data.result.payCheckDate	= "20160926";
            //response.data.result.transTime = "20160926154010";
            //response.data.result.orderId = "OWC20161104181148726rpVQ";
            //response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            //response.data.result.payType = "04";

            ////支払待ち
            //response.meta.code = "00";
            //response.meta.message	= "SUCCESS";
            //response.data.errorCode =	"";
            //response.data.errorInfo =	"";
            //response.data.subErrorCode = "";
            //response.data.sign = null;
            //response.data.result.transStatus = "02";
            //response.data.result.payCheckDate	= "20160926";
            //response.data.result.transTime = "20160926154010";
            //response.data.result.orderId = "OWC20161104181148726rpVQ";
            //response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            //response.data.result.payType = "04";

            ////支払中
            //response.meta.code = "00";
            //response.meta.message	= "SUCCESS";
            //response.data.errorCode =	"";
            //response.data.errorInfo =	"";
            //response.data.subErrorCode = "";
            //response.data.sign = null;
            //response.data.result.transStatus = "03";
            //response.data.result.payCheckDate	= "20160926";
            //response.data.result.transTime = "20160926154010";
            //response.data.result.orderId = "OWC20161104181148726rpVQ";
            //response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            //response.data.result.payType = "04";

            //顧客支払中止
            //response.meta.code = "00";
            //response.meta.message	= "SUCCESS";
            //response.data.errorCode =	"";
            //response.data.errorInfo =	"";
            //response.data.subErrorCode = "";
            //response.data.sign = null;
            //response.data.result.transStatus = "15";
            //response.data.result.payCheckDate	= "20160926";
            //response.data.result.transTime = "20160926154010";
            //response.data.result.orderId = "OWC20161104181148726rpVQ";
            //response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            //response.data.result.payType = "04";

            //返金済
            //response.meta.code = "00";
            //response.meta.message = "SUCCESS";
            //response.data.errorCode = "";
            //response.data.errorInfo = "";
            //response.data.subErrorCode = "";
            //response.data.sign = null;
            //response.data.result.transStatus = "06";
            //response.data.result.payCheckDate = "20160926";
            //response.data.result.transTime = "20160926154010";
            //response.data.result.orderId = "OWC20161104181148726rpVQ";
            //response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            //response.data.result.payType = "04";


            //取消成功
            //response.meta.code = "00";
            //response.meta.message = "SUCCESS";
            //response.data.errorCode = "";
            //response.data.errorInfo = "";
            //response.data.subErrorCode = "";
            //response.data.sign = null;
            //response.data.result.transStatus = "10";
            //response.data.result.payCheckDate = "20160926";
            //response.data.result.transTime = "20160926154010";
            //response.data.result.orderId = "OWC20161104181148726rpVQ";
            //response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            //response.data.result.payType = "04";

            //E09103オーダが存在しません
            //response.meta.code = "09";
            //response.meta.message = "FAILURE";
            //response.data.errorCode = "E09103";
            //response.data.errorInfo = "オーダが存在しません。";
            //response.data.subErrorCode = null;
            //response.data.sign = null;
            //response.data.result = null;
            //response.data.result.transStatus = "15";
            //response.data.result.payCheckDate	= "20160926";
            //response.data.result.transTime = "20160926154010";
            //response.data.result.orderId = "OWC20161104181148726rpVQ";
            //response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            //response.data.result.payType = "04";


            //pay.orders.Add(new PayResponseOrder(){aa ="AA1",bb = "BB1"});
            //pay.orders.Add(new PayResponseOrder(){aa ="AA2",bb = "BB2"});
            response.data.sign = Signiture.GetSigniture(response);

			using (var ms = new MemoryStream())
			using (var sr = new StreamReader(ms))
			{
				var	serializer = new DataContractJsonSerializer(typeof(WbCPSRawConfirmResponseV1));
				serializer.WriteObject(ms, response);
				ms.Position	= 0;

				var json = sr.ReadToEnd();

				Console.WriteLine($"{json}");

				using(var fs = new StreamWriter("ConfirmData.txt")){
				fs.Write(json);
			}
		}


		}
        static void MakeDepositResponse()
        {
            WbCPSRawDepositResponse response = new WbCPSRawDepositResponse();
            response.meta = new WbCPSRawResponseMeta();
            response.data = new WbCPSRawDepositResponseData();
            response.data.result = new WbCPSRawDepositResponseResult();

            response.meta.code = "00";
            response.meta.message = "SUCCESS";
            response.data.errorCode = "";
            response.data.errorInfo = "";
            response.data.subErrorCode = "";
            response.data.sign = null;
            response.data.result.orderDetailId = "J1WP20161103154004945P2J";
            response.data.result.orderId = "OWC20161104181148726rpVQ";
            response.data.result.transTime = "20160926154010";
            response.data.result.currencyCode = "JPY";
            response.data.result.amount = "1000";
            response.data.result.amountRmb = null;
            response.data.result.transStatus = "18";
            
            //pay.orders.Add(new PayResponseOrder(){aa ="AA1",bb = "BB1"});
            //pay.orders.Add(new PayResponseOrder(){aa ="AA2",bb = "BB2"});
            response.data.sign = Signiture.GetSigniture(response);

            using (var ms = new MemoryStream())
            using (var sr = new StreamReader(ms))
            {
                var serializer = new DataContractJsonSerializer(typeof(WbCPSRawDepositResponse));
                serializer.WriteObject(ms, response);
                ms.Position = 0;

                var json = sr.ReadToEnd();

                Console.WriteLine($"{json}");

                using(var fs = new StreamWriter("DepositData.txt")){
                    fs.Write(json);
                }
            }


		}
	}
}
