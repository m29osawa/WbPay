using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WbPay
{
	class WbPayConf
	{
		const string FILE_EXTENT	= ".conf";

		static WbPayLog log;
		internal static WbPayLog Log {
			set { log = value; }
		}	

		Dictionary<string,string> confDict = new Dictionary<string, string>();

		internal WbPayConf(string home_dir,string app_name)
		{
			string filename = home_dir + @"\" + app_name + FILE_EXTENT;
			try{
				using(StreamReader reader = new StreamReader(filename)){
					while(!reader.EndOfStream){
						string s = reader.ReadLine();

						int comment_pos = s.IndexOf('#');
						if(comment_pos >= 0){
							s = s.Substring(0,comment_pos);
						}
						s = s.Trim();
						if(String.IsNullOrEmpty(s)) continue;

						string[] kvs = s.Split(new char[]{'='},2,StringSplitOptions.None);
						string key = kvs[0];
						string value = "";
						if(kvs.Length > 1) value = kvs[1];

						key = key.Trim();
						value = value.Trim();
						if(String.IsNullOrEmpty(key)) continue;

						if(confDict.ContainsKey(key)){
							log?.Write(WbPayLogLevel.Warning,"WbPayConf","Configuration Key duplicated. : " + key);
						}
						confDict[key] = value;
					}
				}
			}catch(Exception){
				log?.Write(WbPayLogLevel.Error,"WbPayConf","Configuration file open error. : " + filename);
				throw;
			}
		}
		internal string Get(string key)
		{
			if(confDict.ContainsKey(key)){
				return confDict[key];
			}else{
				return null;
			}
		}
	}
}
