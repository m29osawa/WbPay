using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace WbPay
{
	class WbCPSHttpClient
	{
		static WbPayLog	log;
		static bool		ignoreCertificateError = false;
		
		static readonly HttpClient httpClient;

		static WbCPSHttpClient()
		{
			httpClient = new HttpClient();
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;

			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
			httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue();
			httpClient.DefaultRequestHeaders.CacheControl.NoCache = true;
			httpClient.DefaultRequestHeaders.ConnectionClose = true;
			httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("WbCPSHttpClient","1.0"));
		}

		internal static int Timeout{ // sec
			set {
				httpClient.Timeout = new TimeSpan((long)value * 10_000_000L);
			}
		}
		internal static WbPayLog Log {
			set { log = value; }
		}
		internal static bool IgnoreCertificateError {
			set { ignoreCertificateError = value; }
		}
		internal static void SetUserAget(string name,string version)
		{
			httpClient.DefaultRequestHeaders.UserAgent.Clear();
			httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(name,version));
		}
		
		private WbCPSHttpClient() {}

		internal static async Task<string> Post(string url,string request_st)
		{	
			log?.Write(WbPayLogLevel.Transaction,"HttpPost","url=" + url);
			log?.Write(WbPayLogLevel.Transaction,"HttpPost","content=" + request_st);
			try{
				byte[] request_by =Encoding.UTF8.GetBytes(request_st);
				
				using(MemoryStream request_stream = new MemoryStream()){
					request_stream.Write(request_by,0,request_by.Length);
					request_stream.Position =0;

					using(StreamContent request_content = new StreamContent(request_stream)){
						request_content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
						request_content.Headers.ContentType.CharSet = "UTF-8";

						using(HttpResponseMessage response = await httpClient.PostAsync(url,request_content)){
						
							if((int)response.StatusCode != 200){	// 200以外の可能性注意！！
								throw new WbCPSHttpException((int)response.StatusCode + " " + response.StatusCode.ToString());
							}
			
							String response_st = await response.Content.ReadAsStringAsync();
							log?.Write(WbPayLogLevel.Transaction,"HttpResponse","content=" + response_st);
							return(response_st);
						}
					}
				}
			}catch(Exception ex){
				string error_mess = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
				log?.Write(WbPayLogLevel.Error,"HttpPost",error_mess);
				throw new WbCPSHttpException(error_mess);
			}
		}
		
		// SSL証明書検証時に呼ばれるCallback
		// .Net Framework 4.6以降はセキュリティー強化で、App.configに別の設定有り。
		static bool ValidateServerCertificate(object sender,X509Certificate certificate,
													X509Chain chain,SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None) return true;

			log?.Write(WbPayLogLevel.Error,"HttpPost","サーバのSSL証明書は無効です。");

			if(ignoreCertificateError) return true;

			return false;
		}
	}
}
