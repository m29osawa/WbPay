using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TestCPSOCX
{
    [DataContract]
	class CPSRefundResponseMeta
	{
		[DataMember(EmitDefaultValue = false)]
		public string code {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string message {get; set;}
	}
	[DataContract]
	class CPSRefundResponseResult
	{
		[DataMember(EmitDefaultValue = false)]
		public string orderId {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string orderDetailId {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string transStatus {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string currencyCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string refundAmount {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string transTime {get; set;}
	}
	[DataContract]
	class CPSRefundResponseData
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
		public CPSRefundResponseResult result {get; set;}= new CPSRefundResponseResult();
	}
	[DataContract]	
	class CPSRefundResponse
	{
		[DataMember(EmitDefaultValue = false)]
		public CPSRefundResponseMeta meta {get; set;} = new CPSRefundResponseMeta();
		[DataMember(EmitDefaultValue = false)]
		public CPSRefundResponseData data {get; set;} = new CPSRefundResponseData();
	}
}
