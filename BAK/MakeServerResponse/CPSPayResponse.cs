using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MakeServerResponse
{
    [DataContract]
	class CPSPayResponseMeta
	{
		[DataMember(EmitDefaultValue = false)]
		public string code {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string message {get; set;}
	}
	[DataContract]
	class CPSPayResponseResult
	{
		[DataMember(EmitDefaultValue = false)]
		public string orderDetailId {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string orderId {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string transTime {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string currencyCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string amount {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string amountRmb {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string transStatus {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string payType {get; set;}
	}
	[DataContract]
	class CPSPayResponseData
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
		public CPSPayResponseResult result {get; set;}= new CPSPayResponseResult();
	}
	[DataContract]	
	class CPSPayResponse
	{
		[DataMember(EmitDefaultValue = false)]
		public CPSPayResponseMeta meta {get; set;} = new CPSPayResponseMeta();
		[DataMember(EmitDefaultValue = false)]
		public CPSPayResponseData data {get; set;} = new CPSPayResponseData();
	}
}
