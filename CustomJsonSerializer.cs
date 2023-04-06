using RestSharp;
using RestSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace KM.GD.PrintInvoices
{
   public class CustomJsonSerializer : IRestSerializer
   {
     
      public string Serialize(object obj) => JsonSerializer.Serialize(obj, new JsonSerializerOptions() { IgnoreNullValues = true } );

      public string Serialize(Parameter bodyParameter) => Serialize(bodyParameter.Value);

      public T Deserialize<T>(IRestResponse response) => JsonSerializer.Deserialize<T>(response.Content);
      public List<T> DeserializeList<T>(IRestResponse response) => JsonSerializer.Deserialize<List<T>>(response.Content);

      public string[] SupportedContentTypes { get; } =
      {
        "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
      };

      public string ContentType { get; set; } = "application/json";

      public DataFormat DataFormat { get; } = DataFormat.Json;
   }
}