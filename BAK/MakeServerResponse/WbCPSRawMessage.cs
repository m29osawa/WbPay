using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MakeServerResponse
{
	//
	// 支払 Pay
	//
	[DataContract]
	class WbCPSRawPayRequest
	{
		[DataMember(EmitDefaultValue = false)]
		public string locale {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string timeZone {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string payType {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string branchCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string terminalCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string currencyCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string amount {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string receiptNo {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string userCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string appVersion {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string sign {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string remark {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string extendInfo {get; set;}
	}
	[DataContract]
	class WbCPSRawPayResponseV1
	{
		[DataMember(EmitDefaultValue = false)]
		public WbCPSRawResponseMeta meta {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public WbCPSRawPayResponseDataV1 data {get; set;}
	}
	[DataContract]
	class WbCPSRawPayResponseDataV1
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
		public WbCPSRawPayResponseResultV1 result {get; set;}
	}
	[DataContract]
	class WbCPSRawPayResponseResultV1
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
	//
	// 返金 Refund
	//
	[DataContract]
	class WbCPSRawRefundRequest
	{
		[DataMember(EmitDefaultValue = false)]
		public string locale {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string timeZone {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string branchCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string terminalCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string refundAmount {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string orderId {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string orderDetailId {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string currencyCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string refundReason {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string remark {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string appVersion {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string sign {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string qryToken {get; set;}
	}
	[DataContract]	
	class WbCPSRawRefundResponse
	{
		[DataMember(EmitDefaultValue = false)]
		public WbCPSRawResponseMeta meta {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public WbCPSRawRefundResponseData data {get; set;}
	}
	[DataContract]
	class WbCPSRawRefundResponseData
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
		public WbCPSRawRefundResponseResult result {get; set;}
	}
	[DataContract]
	class WbCPSRawRefundResponseResult
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
	//
	// 取消 Reverse
	//
	[DataContract]
	class WbCPSRawReverseRequest
	{
		[DataMember(EmitDefaultValue = false)]
		public string locale {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string timeZone {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string branchCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string terminalCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string orderId {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string sign {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string appVersion {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string qryToken {get; set;}
	}
	[DataContract]	
	class WbCPSRawReverseResponse
	{
		[DataMember(EmitDefaultValue = false)]
		public WbCPSRawResponseMeta meta {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public WbCPSRawReverseResponseData data {get; set;}
	}
	[DataContract]
	class WbCPSRawReverseResponseData
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
		public WbCPSRawReverseResponseResult result {get; set;}
	}
	[DataContract]
	class WbCPSRawReverseResponseResult
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
	//
	// 確認 Confirm
	//
	[DataContract]
	class WbCPSRawConfirmRequest
	{
		[DataMember(EmitDefaultValue = false)]
		public string locale {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string timeZone {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string branchCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string terminalCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string orderId {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string orderDetailId {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string sign {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string qryToken {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string queryFlg {get; set;}
	}
	[DataContract]	
	class WbCPSRawConfirmResponseV1
	{
		[DataMember(EmitDefaultValue = false)]
		public WbCPSRawResponseMeta meta {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public WbCPSRawConfirmResponseDataV1 data {get; set;}
	}
	[DataContract]
	class WbCPSRawConfirmResponseDataV1
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
		public WbCPSRawConfirmResponseResultV1 result {get; set;}
	}
	[DataContract]
	class WbCPSRawConfirmResponseResultV1
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
	//
	// 入金 Deposit
	//
	[DataContract]
	class WbCPSRawDepositRequest
	{
		[DataMember(EmitDefaultValue = false)]
		public string locale {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string timeZone {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string payType {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string branchCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string terminalCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string currencyCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string valueType {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string amount {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string receiptNo {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string userCode {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string appVersion {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string sign {get; set;}
	}
	[DataContract]	
	class WbCPSRawDepositResponse
	{
		[DataMember(EmitDefaultValue = false)]
		public WbCPSRawResponseMeta meta {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public WbCPSRawDepositResponseData data {get; set;}
	}
	[DataContract]
	class WbCPSRawDepositResponseData
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
		public WbCPSRawDepositResponseResult result {get; set;}
	}
	[DataContract]
	class WbCPSRawDepositResponseResult
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
	//
	// 共通レスポンスパラメータ meta
	//
	[DataContract]
	class WbCPSRawResponseMeta
	{
		[DataMember(EmitDefaultValue = false)]
		public string code {get; set;}
		[DataMember(EmitDefaultValue = false)]
		public string message {get; set;}
	}
}
