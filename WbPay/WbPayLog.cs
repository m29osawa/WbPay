using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace WbPay
{
	enum WbPayLogLevel{
		Basic		= 0,
		Error		= 1,
		Warning		= 2,
		Sequence	= 3,
		Transaction	= 4,
	}
		
	class WbPayLog
	{
		const string LOG_DIR = "Log";
		const string FILE_EXTENT = ".log";

		const int FILE_LOCK_TIMEOUT	= 1000;	// msec
		const int CLOSE_WAIT		= 100;	// msec

		static object fileLock = new object();

		string logDir;
		string appName;
		string fileNamePart;

		WbPayLogLevel logLevel = WbPayLogLevel.Warning;
		internal WbPayLogLevel LogLebel{
			set{ logLevel = value; }
		}

		internal WbPayLog(string home_dir,string app_name)
		{
			logDir = home_dir + @"\" + LOG_DIR;
			appName = app_name;
			fileNamePart = logDir + @"\" + app_name + ".";
			string fullFileName = fileNamePart + DateTime.Now.ToString("yyyyMMdd") + FILE_EXTENT;

			if(!Directory.Exists(home_dir)){
				throw new DirectoryNotFoundException("アプリケーションホームにアクセスできません。" + home_dir);
			}
			if(!Directory.Exists(logDir)){
				throw new DirectoryNotFoundException("ログディレクトリーにアクセスできません。" + logDir);
			}

			using(StreamWriter writer = new StreamWriter(fullFileName,true,new UTF8Encoding(false,false))){
			}
		}
		internal void Close()
		{
			bool lock_flag = false;
			try{
				Thread.Sleep(CLOSE_WAIT);
				Monitor.TryEnter(fileLock,FILE_LOCK_TIMEOUT,ref lock_flag);
				fileNamePart = null;
			}finally{
				if(lock_flag) Monitor.Exit(fileLock);
			}
		}
		internal void Expire(int expire_day)
		{
			
			if(String.IsNullOrEmpty(fileNamePart)) return;
			if(expire_day <= 0) return;

			DirectoryInfo dir = new DirectoryInfo(logDir);
			DateTime limitdate = DateTime.Today.AddDays(-expire_day);

			FileInfo[] files = dir.GetFiles(appName + ".*" + FILE_EXTENT);
			foreach(FileInfo file in files){
				string[] names = file.Name.Split(new char[]{'.'},3,StringSplitOptions.None);
				if(names.Length != 3) continue;
				if(names[1].Length != 8) continue;

				try{
					int fyear = Int32.Parse(names[1].Substring(0,4));
					int fmonth = Int32.Parse(names[1].Substring(4,2));
					int fday = Int32.Parse(names[1].Substring(6,2));
					DateTime filedate = new DateTime(fyear,fmonth,fday);
				
					if(filedate < limitdate){
						file.Delete();
						Write(WbPayLogLevel.Basic,"WbPayLog","Log file \"" + file.Name + "\" expired.");
					}
				}catch(Exception){
					continue;
				}
			}

		}
		internal void Write(WbPayLogLevel level,string proc,string message)
		{
			try{
				if((int)level > (int)logLevel) return;
				string date_st = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ff");
				Task t = Task.Run(() =>{
					bool lock_flag = false;
					try{
						Monitor.TryEnter(fileLock,FILE_LOCK_TIMEOUT,ref lock_flag);
						if(!lock_flag) return;
						if(String.IsNullOrEmpty(fileNamePart)) return;
						string fullFileName = fileNamePart + DateTime.Now.ToString("yyyyMMdd") + FILE_EXTENT;
						using(StreamWriter writer = new StreamWriter(fullFileName,true,new UTF8Encoding(false,false))){
							writer.WriteLine("{0,-19} {1,-12} [{2,-20}] {3}",date_st,level.ToString(),proc,message);
						}
					} catch(Exception) {
					} finally{
						if(lock_flag) Monitor.Exit(fileLock);
					}
				});
			}catch(Exception){
			}
		}
	}
}
