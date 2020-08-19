using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MakeServerResponse
{
	[DataContract]
	class PayResponseMeta
	{
		//[DataMember(EmitDefaultValue = false)]
		//public string aa {get; set;}
		//[DataMember(EmitDefaultValue = false)]
		//public string bb {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string code {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string message {get; set;}
	}
	[DataContract]
	class PayResponseResult
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
	class PayResponseData
	{
		//[DataMember(EmitDefaultValue = false)]
		//public string aa {get; set;}
		//[DataMember(EmitDefaultValue =false)]
		//public string bb {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string errorCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string errorInfo {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string subErrorCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string sign {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public PayResponseResult result {get; set;}= new PayResponseResult();
	}
	[DataContract]	
	class PayResponse
	{
		//[DataMember(EmitDefaultValue = false)]
		//public int a { get; set; }
		//[DataMember(EmitDefaultValue = false)]
		//public string b {get; set; }
		//[DataMember(EmitDefaultValue = false)]
		//public PayResponseMeta c {get; set;} = new PayResponseMeta();
		//[DataMember(EmitDefaultValue = false)]
		//public List<PayResponseOrder> orders{get; set;} = new List<PayResponseOrder>();
		[DataMember(EmitDefaultValue = false)]
		public PayResponseMeta meta {get; set;} = new PayResponseMeta();
		[DataMember(EmitDefaultValue = false)]
		public PayResponseData data {get; set;} = new PayResponseData();
	}
}
