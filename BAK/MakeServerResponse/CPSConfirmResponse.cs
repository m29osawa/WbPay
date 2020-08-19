using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MakeServerResponse
{
    [DataContract]
	class CPSConfirmResponseMeta
	{
		[DataMember(EmitDefaultValue = false)]
		public string code {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string message {get; set;}
	}
	[DataContract]
	class CPSConfirmResponseResult
	{
		[DataMember(EmitDefaultValue = false)]
		public string transStatus {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string payCheckDate {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string transTime {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string orderId {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string orderDetailId {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string payType {get; set;}
	}
	[DataContract]
	class CPSConfirmResponseData
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
		public CPSConfirmResponseResult result {get; set;}= new CPSConfirmResponseResult();
	}
	[DataContract]	
	class CPSConfirmResponse
	{
		[DataMember(EmitDefaultValue = false)]
		public CPSConfirmResponseMeta meta {get; set;} = new CPSConfirmResponseMeta();
		[DataMember(EmitDefaultValue = false)]
		public CPSConfirmResponseData data {get; set;} = new CPSConfirmResponseData();
	}
}
