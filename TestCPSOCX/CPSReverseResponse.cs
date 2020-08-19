using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TestCPSOCX
{
    [DataContract]
	class CPSReverseResponseMeta
	{
		[DataMember(EmitDefaultValue = false)]
		public string code {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string message {get; set;}
	}
	[DataContract]
	class CPSReverseResponseResult
	{
		[DataMember(EmitDefaultValue = false)]
		public string orderId {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string orderDetailId {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string transStatus {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string amount {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string currencyCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string transTime {get; set;}
	}
	[DataContract]
	class CPSReverseResponseData
	{
		[DataMember(EmitDefaultValue = false)]
		public string errorCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string errorInfo {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string subErrorCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string sign {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public CPSReverseResponseResult result {get; set;}= new CPSReverseResponseResult();
	}
	[DataContract]	
	class CPSReverseResponse
	{
		[DataMember(EmitDefaultValue = false)]
		public CPSReverseResponseMeta meta {get; set;} = new CPSReverseResponseMeta();
		[DataMember(EmitDefaultValue = false)]
		public CPSReverseResponseData data {get; set;} = new CPSReverseResponseData();
	}
}
