using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WbPay
{
	public class WbPayException : Exception
	{
		public int		Type { get; internal set; }
		public string	Code { get; internal set; }
		public string	SubCode { get; internal set;} = "" ;

		internal WbPayException(int type,int subtype,int code,string message)
			: base(message)
		{
			this.Type = type;
			this.Code = String.Format("E{0:D2}{1:D1}{2:D2}",type,subtype,code);
		}
		internal WbPayException(int type,int subtype,int code,string subcode,string message)
			: base(message)
		{
			this.Type = type;
			this.Code = String.Format("E{0:D2}{1:D1}{2:D2}",type,subtype,code);
			this.SubCode = subcode;
		}
		internal WbPayException(int type,int subtype,int code,string message, Exception innerException)
			: base(message, innerException)
		{
			this.Type = type;
			this.Code = String.Format("E{0:D2}{1:D1}{2:D2}",type,subtype,code);
		}
		internal WbPayException(int type,int subtype,int code,string subcode,string message, Exception innerException)
			: base(message, innerException)
		{
			this.Type = type;
			this.Code = String.Format("E{0:D2}{1:D1}{2:D2}",type,subtype,code);
			this.SubCode = subcode;
		}
		public override string ToString()
		{
			string st =  "エラーコード: " + Code;
			if(!String.IsNullOrEmpty(SubCode)){
				st = st + ":" + SubCode;
			}
			st = st + ": ";
			return st + base.ToString();
		}
	}
}
