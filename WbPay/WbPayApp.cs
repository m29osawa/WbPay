using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace WbPay
{
	class WbPayApp
	{
		static string FILE_EXTENT = ".running";

		string homeDir;
		string appName;
		string fullFileName;

		FileStream stream = null;

		internal WbPayApp(string home_dir,string app_name)
		{
			homeDir = home_dir;
			appName = app_name;
		}
		internal bool StartExclulsive()
		{
			if(stream != null) return true;

			Process ps = Process.GetCurrentProcess();

			fullFileName = homeDir + @"\" + appName + FILE_EXTENT;
			try{
				stream = new FileStream(fullFileName,FileMode.Create,FileAccess.ReadWrite,FileShare.Read);

				StreamWriter writer = new StreamWriter(stream);
				writer.WriteLine("Module:{0}",ps.MainModule.ModuleName);
				writer.WriteLine("File:{0}",ps.MainModule.FileName);
				writer.WriteLine("Pid:{0}",ps.Id);
				writer.WriteLine("Time:{0}",DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
				writer.Flush();

			}catch(Exception){
				return false;
			}
			return true;
		}
		internal void Stop()
		{
			stream?.Dispose();
			stream = null;

			if(File.Exists(fullFileName)){
				File.Delete(fullFileName);
			}
		}
	}
}
