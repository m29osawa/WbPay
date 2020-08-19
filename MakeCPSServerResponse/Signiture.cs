using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MakeCPSServerResponse
{
	class Signiture
	{
        public static string GetSigniture(Object obj)
        {
            Encoding enc = Encoding.UTF8;
            string sig = "";
  
            try{
                RSAParameters private_key = LoadKey("serverPrivateKey.pem");
                using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider()){
                    rsa.ImportParameters(private_key);

                    string sigst = GetSigString(obj);
Console.WriteLine("SigString = {0}", sigst);

                    byte[] hashed_sigst_b;
                    using(SHA256 sha256 = SHA256.Create()) {
                        hashed_sigst_b = sha256.ComputeHash(enc.GetBytes(sigst));
                    }
                    string hashed_sigst = ConvBytesToHexString(hashed_sigst_b);
                    using(SHA1Managed sha1 = new SHA1Managed()){
                        byte[] hash_b = sha1.ComputeHash(enc.GetBytes(hashed_sigst));

                        RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(rsa);
                        byte[] sig1_b = formatter.CreateSignature(sha1);
                        string sig1_base64 = Convert.ToBase64String(sig1_b);
                        sig = Convert.ToBase64String(enc.GetBytes(sig1_base64));
                    }
                }
            }catch(CryptographicException e){
                Console.WriteLine(e.Message);
            }
//Console.WriteLine("Sig = {0}",sig);
            return sig;
        }
      
        static String ConvBytesToHexString(byte[] barray) {
            StringBuilder sb = new StringBuilder();

//Console.WriteLine("ConvBytesToHexString : length = {0}", barray.Length);

            foreach(byte b in barray) {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

//        static void PrintBytesX(String st,byte[] barray)
//        {
//            String st2 = "";
//            foreach(byte b in barray){
//                st2 = st2 + b.ToString("X");
//           }
//            Console.WriteLine("{0} : {1}",st,st2);
//        }
//        static void PrintBytesS(String st,byte[] barray)
//        {
//            Encoding enc = Encoding.UTF8;
//            Console.WriteLine("{0} : {1}",st,enc.GetString(barray));
//        }
        static string GetSigString(Object obj)
        {
            Type objtype = obj.GetType();

            SortedSet<string> set = new SortedSet<string>();
            
            PropertyInfo[] props = objtype.GetProperties();
            foreach(var propinfo in props){
                dynamic val = propinfo.GetValue(obj);
                if(val == null) continue;

                Type proptype = propinfo.PropertyType;
                if(proptype.IsGenericType){
                    if(proptype.GetGenericTypeDefinition() != typeof(List<>)) continue;
                    if(val.Count == 0) continue;
                }
                //else if(!(proptype.IsPrimitive || proptype.Name == "String")){
                //    continue;
                //}
                set.Add(propinfo.Name);
            }

            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append("{");
            foreach(var name in set){
                PropertyInfo propinfo = objtype.GetProperty(name);
                Type proptype = propinfo.PropertyType;
                dynamic val = propinfo.GetValue(obj); //dynamic を使わなくていいようにしたい
                if(i != 0) sb.Append("&");
                sb.Append(name);
                sb.Append("=");
                if(proptype.IsGenericType){
                    if(val.Count == 1) { //ここ違うかも。Beansは常に"[]"で囲うのかも
                        sb.Append(name);
                        sb.Append("=");
                        sb.Append(GetSigString(val[0]));
                    } else {
                        int j = 0;
                        sb.Append("[");
                        foreach(var element in val){
                            if(j != 0)sb.Append(",");
                            sb.Append(GetSigString(element));
                            j++;
                        }
                        sb.Append("]");
                    }
                }else if(!(proptype.IsPrimitive || proptype.Name == "String")){
                    sb.Append(GetSigString(val));
                }else{
                    sb.Append(val);//下位のobjに書き出すべき内容がない場合は？今は未処理
                }
                i++;
            }
            sb.Append("}");
            return sb.ToString();
        }

        public static RSAParameters LoadKey(string filename) {
            byte[] der = null;
            using(var stream = new FileStream(filename, FileMode.Open)) {
                byte[] bt = new byte[4000];

                int n = stream.Read(bt, 0, bt.Length);
//Console.WriteLine("Len = {0} , {1}", n, bt.Length);
                string st = Encoding.UTF8.GetString(bt, 0, n);
//Console.WriteLine("****" + st);
                // Base64デコードして、DER(バイナリー形式)にする
                var encoded = st.
                    Replace(@"-----BEGIN RSA PRIVATE KEY-----", string.Empty).
                    Replace(@"-----END RSA PRIVATE KEY-----", string.Empty);
                encoded = new Regex(@"\r?\n").Replace(encoded, string.Empty);
//Console.WriteLine("### ({0}) : {1}", encoded.Length, encoded);
                der = Convert.FromBase64String(encoded);
            }

            return CreateParameter(der);
        }
        private static RSAParameters CreateParameter(byte[] der) {
            byte[] sequence = null;
            using(var reader = new BinaryReader(new MemoryStream(der))) {
                sequence = Read(reader);
            }

            var parameters = new RSAParameters();
            using(var reader = new BinaryReader(new MemoryStream(sequence))) {
                Read(reader); // version
                parameters.Modulus = Read(reader);
                parameters.Exponent = Read(reader);
                parameters.D = Read(reader);
                parameters.P = Read(reader);
                parameters.Q = Read(reader);
                parameters.DP = Read(reader);
                parameters.DQ = Read(reader);
                parameters.InverseQ = Read(reader);
            }
            return parameters;
        }

        private static byte[] Read(BinaryReader reader) {
            // tag
            reader.ReadByte();

            // length
            int length = 0;
            byte b = reader.ReadByte();
            if((b & 0x80) == 0x80) // length が128 octet以上
            {
                int n = b & 0x7F;
                byte[] buf = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                for(var i = n - 1;i >= 0;--i)
                    buf[i] = reader.ReadByte();
                length = BitConverter.ToInt32(buf, 0);
            } else // length が 127 octet以下
            {
                length = b;
            }

            // value
            if(length == 0)
                return new byte[0];
            byte first = reader.ReadByte();
            if(first == 0x00) length -= 1; // 最上位byteが0x00の場合は、除いておく
            else reader.BaseStream.Seek(-1, SeekOrigin.Current); // 1byte 読んじゃったので、streamの位置を戻しておく
            return reader.ReadBytes(length);
        }



        //         // The following method is invoked by the RemoteCertificateValidationDelegate.
        //         //SSL証明書検証時に呼ばれるCallback
        //         // 強制的にtrueを返すようにすると、証明書に問題があってもエラーとならない。
        //         //　開発中専用なので注意
        //         // .Net Framework 4.6以降はセキュリティー強化で、これではだめかも。App.configに別の設定有り。
        //        public static bool ValidateServerCertificate(
        //            object sender,
        //            X509Certificate certificate,
        //            X509Chain chain,
        //            SslPolicyErrors sslPolicyErrors)
        //        {
        //            if (sslPolicyErrors == SslPolicyErrors.None)
        //                return true;

        //            return true;
        //            //Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

        //            // Do not allow this client to communicate with unauthenticated servers.
        //            //return false;
        //        }
    }
}
