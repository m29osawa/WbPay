using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TestCPSOCX
{
	[DataContract]
    class EVRWRefundRequestAux
    {
		[DataMember(EmitDefaultValue = false)]
		public string orderDetailId {get; set; }
		[DataMember(EmitDefaultValue = false)]
		public string refundReason {get; set; }
		[DataMember(EmitDefaultValue = false)]
		public string remark {get; set; }
		[DataMember(EmitDefaultValue = false)]
		public string qryToken {get; set; }
    }
}
