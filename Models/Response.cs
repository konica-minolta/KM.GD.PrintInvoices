using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KM.GD.PrintInvoices.Models
{
   public class ResponseBase
   {
      public ResponseBase()
      {
     
      }

      public string Code { get; set; }
      public string Message { get; set; }
   }

   public class ResponsePrinterProduct
   {
      public string id { get; set; }
      public string name { get; set; }
      public string description { get; set; }
      public bool useStandardWorksteps { get; set; }
      public List<Service> services { get; set; }
      public List<WorkStep> workSteps { get; set; }
   }

   public class Service
   {
      public string defaultOptionId { get; set; }
      public string serviceId { get; set; }
      public string value { get; set; }
      public List<string> options { get; set; }
   }

   public class State
   {
      public string id { get; set; }
      public string name { get; set; }
   }

   public class WorkStep
   {
      public string id { get; set; }
      public string name { get; set; }
      public List<State> states { get; set; }
   }


   public class UploadFileResponse
   {
      public string tempFileName { get; set; }
   }
   public class Result
   {
      public bool isSubmissionValid { get; set; }
      public List<string> orderItemIds { get; set; }
      public string orderId { get; set; }
   }

   public class ResponsePrintOrder
   {
      public Result result { get; set; }
   }

   public class ResponsePrintingOrderItems
   {
      public bool ok { get; set; }
      public string message { get; set; }
      public Details details { get; set; }
   }
   public class Details
   {
      public string orderItemId { get; set; }
      public string printer { get; set; }
      public string paper { get; set; }
      public string workstep { get; set; }
   }
}