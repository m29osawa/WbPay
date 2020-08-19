using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WbPay
{
	class WbCPSHttpException : Exception
	{
		internal WbCPSHttpException(string message)
			: base(message)
		{
		}
		internal WbCPSHttpException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
