using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;


namespace TestCPS
{
	class TestClient
	{
	    static readonly HttpClient client;
        
        static TestClient()
        {
            client = new HttpClient();

             //証明書検証時によばれるCallbackを設定する。エラー対策。開発中専用
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
            //new RemoteCertificateValidationCallback(ValidateServerCertificate);
        }

        internal static async Task Test1()
        {
            //Console.WriteLine("★");
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine("★");
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try {

                //string uri_st = "http://localhost:8080/Test/Sample.html";
                //string uri_st = "https://localhost:8443/Test/Sample.html";
                //string uri_st = "https://www.google.co.jp";
                //string uri_st = "http://localhost:8080/Test/Sample1?abc=123&def=ABCDEFG";
                //string uri_st = "http://localhost:8080/gateway/pay/C01/00/0/1003685910426110040bQQW23GEWEAJUgEmW4S3pY4z?abc=123&def=ABCDEFG";
                string uri_st = "https://localhost:8443/gateway/pay/C01/00/0/1003685910426110040bQQW23GEWEAJUgEmW4S3pY4z?abc=123&def=ABCDEFG";
                //string uri_st = "https://localhost:8443/Test/Sample1";

                //Befor Any Operation
                Console.WriteLine("DefaultRequestHeaders : {0}",client.DefaultRequestHeaders.Connection);

                //HttpResponseMessage response = await client.GetAsync(uri_st);

                var ms = new MemoryStream();
                byte[] buf =Encoding.UTF8.GetBytes("ABCDEFG12345678");
                ms.Write(buf,0,buf.Length);
                ms.Position =0;
                StreamContent cont = new StreamContent(ms);
                HttpResponseMessage response = await client.PostAsync(uri_st,cont);

                response.EnsureSuccessStatusCode();
                //string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                //Console.WriteLine("Now After Read");
                //Console.WriteLine(responseBody);
            Console.WriteLine("★");
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("★");

            Console.WriteLine("********");
            Console.WriteLine("Content : {0}",response.Content);
                Console.WriteLine("    Header : {0}",response.Content.Headers);
                String st = await response.Content.ReadAsStringAsync();
                Console.WriteLine("    Content : {0}",st);
            Console.WriteLine("Headers : {0}",response.Headers);
            Console.WriteLine("isSuccessStatusCode : {0}",response.IsSuccessStatusCode);
            Console.WriteLine("ReasonPhrase : {0}",response.ReasonPhrase);
            Console.WriteLine("RequestMessage : {0}",response.RequestMessage);
            Console.WriteLine("StatusCode : {0} {1}",(int)response.StatusCode,response.StatusCode);
            Console.WriteLine("Version : {0}",response.Version);

            for(int i = 0;i < 10;i++) {
				Console.WriteLine("★");
				System.Threading.Thread.Sleep(1000);
				//System.Threading.Tasks.Task.Delay(1000);
				//t.Wait();
				//while(!t.IsCompleted){
				//    t.Wait(100);
				//Console.WriteLine("●");
			}
            Console.WriteLine("END");
            }  
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }
        
         // The following method is invoked by the RemoteCertificateValidationDelegate.
         //SSL証明書検証時に呼ばれるCallback
         // 強制的にtrueを返すようにすると、証明書に問題があってもエラーとならない。
         //　開発中専用なので注意
         // .Net Framework 4.6以降はセキュリティー強化で、これではだめかも。App.configに別の設定有り。
        public static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

           
            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
             return true;
      
            // Do not allow this client to communicate with unauthenticated servers.
            //return false;
        }
	}
}
