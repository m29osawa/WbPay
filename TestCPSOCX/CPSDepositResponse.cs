using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TestCPSOCX
{
	[DataContract]
	class CPSDepositResponseMeta
	{
		[DataMember(EmitDefaultValue = false)]
		public string code {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string message {get; set;}
	}
	[DataContract]
	class CPSDepositResponseResult
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
	}
	[DataContract]
	class CPSDepositResponseData
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
		public CPSDepositResponseResult result {get; set;}= new CPSDepositResponseResult();
	}
	[DataContract]	
	class CPSDepositResponse
	{
		[DataMember(EmitDefaultValue = false)]
		public CPSDepositResponseMeta meta {get; set;} = new CPSDepositResponseMeta();
		[DataMember(EmitDefaultValue = false)]
		public CPSDepositResponseData data {get; set;} = new CPSDepositResponseData();
	}
}
