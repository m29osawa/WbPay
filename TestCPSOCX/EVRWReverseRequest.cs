using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TestCPSOCX
{
    [DataContract]
    class EVRWReverseRequestAux
    {
		[DataMember(EmitDefaultValue = false)]
		public string qryToken {get; set; }
    }
}
