using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace WbPay
{
	class WbCPSSignature
	{
		static RSAParameters privateKey;
		static RSAParameters publicKey;

		internal static void LoadKey(string private_key_file,string public_key_file){
			privateKey = ReadPrivateKey(private_key_file);
			publicKey =	ReadPublicKey(public_key_file);
		}
		internal static string CreateSignature(Object obj)
		{
			Encoding enc = Encoding.UTF8;
			
			string signst = GetSigString(obj);
			
			using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider()){
				rsa.ImportParameters(privateKey);

				byte[] signst_hashed_b;
				using(SHA256 sha256	= SHA256.Create()) {
					signst_hashed_b = sha256.ComputeHash(enc.GetBytes(signst));
				}
				string hashed_signst = ConvBytesToHexString(signst_hashed_b);
				using(SHA1Managed sha1 = new SHA1Managed()){
					byte[] hash_b =	sha1.ComputeHash(enc.GetBytes(hashed_signst));

					RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(rsa);
					byte[] sign1_b =	formatter.CreateSignature(sha1);
					string sign1_base64 = Convert.ToBase64String(sign1_b);
					string sign	= Convert.ToBase64String(enc.GetBytes(sign1_base64));
					return sign;
				}
			}
		}
		internal static bool VerifySignature(Object obj,string target_sign)
		{
			Encoding enc = Encoding.UTF8;

			string signst = GetSigString(obj);

			byte[] target_sign_base64_b = Convert.FromBase64String(target_sign);
			byte[] target_sign_b = Convert.FromBase64String(enc.GetString(target_sign_base64_b));
						// sign文字列にBase64のエラーがあると、例外FormatExceptionが発生するので注意
			
			using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider()){
				rsa.ImportParameters(publicKey);

				byte[] signst_hashed_b;
				using(SHA256 sha256	= SHA256.Create()) {
					signst_hashed_b = sha256.ComputeHash(enc.GetBytes(signst));
				}
				string hashed_signst = ConvBytesToHexString(signst_hashed_b);
				using(SHA1Managed sha1 = new SHA1Managed()){
					byte[] hash_b =	sha1.ComputeHash(enc.GetBytes(hashed_signst));

					RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(rsa);
					return deformatter.VerifySignature(sha1,target_sign_b);
				}
			}
		}
	  
		static StringBuilder workHexString = new StringBuilder();

		static String ConvBytesToHexString(byte[] barray)
		{
			workHexString.Clear();			
			foreach(byte b in barray) {
				workHexString.Append(b.ToString("X2"));
			}
			return workHexString.ToString();
		}

		static string GetSigString(Object obj)	// 再帰呼び出し注意
		{
			Type objtype = obj.GetType();

			SortedSet<string> set =	new	SortedSet<string>();
			PropertyInfo[] props = objtype.GetProperties();
			foreach(var	propinfo in	props){
				dynamic	val	= propinfo.GetValue(obj);
				if(val == null)	continue;

				Type proptype =	propinfo.PropertyType;
				if(proptype.IsGenericType){
					if(proptype.GetGenericTypeDefinition() != typeof(List<>)) continue;
					if(val.Count ==	0) continue;
				}
				set.Add(propinfo.Name);
			}

			StringBuilder sb = new StringBuilder();
			int	i =	0;
			sb.Append("{");
			foreach(var	name in	set){
				PropertyInfo propinfo =	objtype.GetProperty(name);
				Type proptype =	propinfo.PropertyType;
				dynamic	val	= propinfo.GetValue(obj); //dynamic注意！！
				if(i !=	0) sb.Append("&");
				sb.Append(name);
				sb.Append("=");
				if(proptype.IsGenericType){
					if(val.Count ==	1) { //ここ違うかも。Beansは常に"[]"で囲うのかも
						sb.Append(name);
						sb.Append("=");
						sb.Append(GetSigString(val[0]));
					} else {
						int	j =	0;
						sb.Append("[");
						foreach(var	element	in val){
							if(j !=	0)sb.Append(",");
							sb.Append(GetSigString(element));
							j++;
						}
						sb.Append("]");
					}
				}else if(!(proptype.IsPrimitive	|| proptype.Name ==	"String")){
					sb.Append(GetSigString(val));
				}else{
					sb.Append(val);//下位のobjに書き出すべき内容がない場合、今は未処理
				}
				i++;
			}
			sb.Append("}");
			return sb.ToString();
		}

		static RSAParameters ReadPrivateKey(string filename) {
			byte[] der = null;
			using(var stream = new FileStream(filename,	FileMode.Open))	{
				byte[] bt =	new	byte[4000];

				int	n =	stream.Read(bt,	0, bt.Length);
				string st =	Encoding.UTF8.GetString(bt,	0, n);
				// Base64デコードして、DER(バイナリー形式)にする
				var	encoded	= st.
					Replace(@"-----BEGIN RSA PRIVATE KEY-----",	string.Empty).
					Replace(@"-----END RSA PRIVATE KEY-----", string.Empty);
				encoded	= new Regex(@"\r?\n").Replace(encoded, string.Empty);
				der	= Convert.FromBase64String(encoded);
			}
			return CreatePrivateParameter(der);
		}
		static RSAParameters CreatePrivateParameter(byte[]	der) {
			byte[] sequence	= null;
			using(var reader = new BinaryReader(new	MemoryStream(der)))	{
				sequence = Read(reader);
			}

			var	parameters = new RSAParameters();
			using(var reader = new BinaryReader(new	MemoryStream(sequence))) {
				Read(reader); // version
				parameters.Modulus = Read(reader);
				parameters.Exponent	= Read(reader);
				parameters.D = Read(reader);
				parameters.P = Read(reader);
				parameters.Q = Read(reader);
				parameters.DP =	Read(reader);
				parameters.DQ =	Read(reader);
				parameters.InverseQ	= Read(reader);
			}
			return parameters;
		}
		static RSAParameters ReadPublicKey(string filename)
		{
			byte[] der = null;
			using (var stream = new FileStream(filename, FileMode.Open)){
				byte[] bt = new byte[4000];

				int n = stream.Read(bt,0,bt.Length);
				string st =	Encoding.UTF8.GetString(bt,	0, n);
				// Base64デコードして、DER(バイナリー形式)にする
				var encoded = st.
					Replace(@"-----BEGIN PUBLIC KEY-----", string.Empty).
					Replace(@"-----END PUBLIC KEY-----", string.Empty);
				encoded = new Regex(@"\r?\n").Replace(encoded, string.Empty);
				der = Convert.FromBase64String(encoded);
			}
			return CreatePublicParameter(der);
		}
		private static RSAParameters CreatePublicParameter(byte[] der)
		{
			byte[] sequence1 = null;
			using(var reader = new BinaryReader(new MemoryStream(der))) {
				sequence1 = Read(reader);
			}

			byte[] sequence2 = null;
			using(var reader = new BinaryReader(new MemoryStream(sequence1))) {
				Read(reader); // sequence
				sequence2 = Read(reader); // bit string
			}

			byte[] sequence3 = null;
			using(var reader = new BinaryReader(new MemoryStream(sequence2))) {
				sequence3 = Read(reader); // sequence
			}

			var parameters = new RSAParameters();
			using (var reader = new BinaryReader(new MemoryStream(sequence3))){
				//Read(reader);	// version
				parameters.Modulus = Read(reader); // モジュラス
				parameters.Exponent = Read(reader); // 公開指数
			}
			return parameters;
		}
		private	static byte[] Read(BinaryReader	reader)	{
			// tag
			reader.ReadByte();
			// length
			int	length = 0;
			byte b = reader.ReadByte();
			if((b &	0x80) == 0x80){	// length が128 octet以上
				int	n =	b &	0x7F;
				byte[] buf = new byte[]	{ 0x00,	0x00, 0x00,	0x00 };
				for(var	i =	n -	1;i	>= 0;--i){
					buf[i] = reader.ReadByte();
				}
				length = BitConverter.ToInt32(buf, 0);
			}else{					// length が 127 octet以下
				length = b;
			}
			// value
			if(length == 0) return new byte[0];
			byte first = reader.ReadByte();
			if(first ==	0x00){
				length -=	1; // 最上位byteが0x00の場合は除く
			}else{
				reader.BaseStream.Seek(-1,	SeekOrigin.Current); //	1byte 先読みを戻す
			}
			return reader.ReadBytes(length);
		}
	}
}
