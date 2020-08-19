using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TestCPSOCX
{
    [DataContract]
	class EVRWPayRequestAux
	{
		[DataMember(EmitDefaultValue = false)]
		public string remark {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string extendInfo {get; set;}
	}
}
