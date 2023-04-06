using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KM.GD.PrintInvoices.Models
{
   //ORDER POST DATA
   public class SubmitterAddress
   {
      public string name { get; set; }
      public string shortName { get; set; }
      public string organisation { get; set; }
      public string street { get; set; }
      public string postalCode { get; set; }
      public string city { get; set; }
      public string region { get; set; }
      public string state { get; set; }
      public string tel1 { get; set; }
      public string tel2 { get; set; }
      public string telfax { get; set; }
      public string email { get; set; }
      public string project { get; set; }
      public string projectNumber { get; set; }
      public string custom1 { get; set; }
      public string custom2 { get; set; }
      public string custom3 { get; set; }
   }

   public class DeliveryAddress
   {
      public string name { get; set; }
      public string shortName { get; set; }
      public string organisation { get; set; }
      public string street { get; set; }
      public string postalCode { get; set; }
      public string city { get; set; }
      public string region { get; set; }
      public string state { get; set; }
      public string tel1 { get; set; }
      public string tel2 { get; set; }
      public string telfax { get; set; }
      public string email { get; set; }
      public string project { get; set; }
      public string projectNumber { get; set; }
      public string custom1 { get; set; }
      public string custom2 { get; set; }
      public string custom3 { get; set; }
   }

   public class BillingAddress
   {
      public string name { get; set; }
      public string shortName { get; set; }
      public string organisation { get; set; }
      public string street { get; set; }
      public string postalCode { get; set; }
      public string city { get; set; }
      public string region { get; set; }
      public string state { get; set; }
      public string tel1 { get; set; }
      public string tel2 { get; set; }
      public string telfax { get; set; }
      public string email { get; set; }
      public string project { get; set; }
      public string projectNumber { get; set; }
      public string custom1 { get; set; }
      public string custom2 { get; set; }
      public string custom3 { get; set; }
   }

   public class Price
   {
      public string currency { get; set; }
      public double subTotal { get; set; }
      public double delivery { get; set; }
      public int deliveryVat { get; set; }
      public int totalNet { get; set; }
      public int total { get; set; }
      public int copy { get; set; }
      public int aux { get; set; }
      public int sum { get; set; }
      public int vat { get; set; }
   }

   public class Attachment
   {
      public string url { get; set; }
      public string tempFileName { get; set; }
   }

   public class Services
   {
      [JsonProperty("Color print")]
      public string ColorPrint { get; set; }

      [JsonProperty("Double-sided")]
      public string DoubleSided { get; set; }
      public string Punching { get; set; }
   }

   public class PageSource
   {
      public string url { get; set; }
      public string tempFileName { get; set; }
      public string originalFileName { get; set; }
   }

   public class OrderItem
   {
      public string note { get; set; }
      public string title { get; set; }
      public string product { get; set; }
      public string printerName { get; set; }
      public int copies { get; set; }
      public Services services { get; set; }
      public List<PageSource> pageSources { get; set; }
      public Price price { get; set; }
   }

   public class PrintOrder
   {
      public string projectName { get; set; }
      public string orderNote { get; set; }
      public string prefix { get; set; }
      public SubmitterAddress submitterAddress { get; set; }
      public DeliveryAddress deliveryAddress { get; set; }
      public BillingAddress billingAddress { get; set; }
      public string customerAccountId { get; set; }
      public string billingAddressId { get; set; }
      public string deliveryAddressId { get; set; }
      public string deliveryType { get; set; }
      public DateTime? deliveryDate { get; set; }
      public DateTime? orderDate { get; set; }
      public Price price { get; set; }
      public List<Attachment> attachments { get; set; }
      public List<OrderItem> orderItems { get; set; }
   }

   public class AccurioWF
   {
      public string workStepId { get; set; }
      public string stateId { get; set; }
   }

   public class AccurioState
   {
      public string status { get; set; }
   }
}