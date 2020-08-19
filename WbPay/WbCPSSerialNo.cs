using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WbPay
{
	class WbCPSSerialNo
	{
		const string FILE_EXTENT	= ".serial";

		static string fullFileName;

		static long serialNo = 0L;	// 15桁　最大値=999_999_999_999_999L 端末内でユニークにする必要有
		
		internal static void Start(string home_dir,string app_name)
		{
			fullFileName = home_dir + @"\" + app_name + FILE_EXTENT;

			if(!Directory.Exists(home_dir)){
				throw new DirectoryNotFoundException("AppHome directory \"" + home_dir + "\" not found.");
			}

			try{
				serialNo = ReadSerialNo();
			}catch{
				DateTime startdate = new DateTime(2020,1,1);
				DateTime nowdate = DateTime.Now;

				TimeSpan diff = nowdate.Subtract(startdate);
				serialNo = (long)diff.TotalSeconds * 10L;
				if(serialNo < 0L || serialNo >= 1_000_000_000_000_000L) serialNo = 0L;

				WriteSerialNo(serialNo);
			}
		}
		internal static void Close()
		{
		}
		internal static long Next()		// 例外有り
		{
			serialNo += 1;
			if(serialNo >= 1_000_000_000_000_000L) serialNo = 1L;

			WriteSerialNo(serialNo);
			return serialNo;
		}
		static long ReadSerialNo(){
			using(StreamReader reader = new StreamReader(fullFileName)){
					return Int64.Parse(reader.ReadLine());
			}
		}
		static void WriteSerialNo(long no){
			using(StreamWriter writer = new StreamWriter(fullFileName)){
				writer.WriteLine(serialNo);
			}
		}

		private WbCPSSerialNo(){ }
	}
}
