using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KM.GD.PrintInvoices.Models
{
   public class ConfigDocumentsType
   {
      public List<DocSettings> Settings { get; set; }
   }
   public class DocSettings
   {
      public string DisplayName { get; set; }
      public List<string> DocType { get; set; }
      public int Copies { get; set; }
      public int Position { get; set; }
      public bool ToSign { get; set; }
      public string SignPosition { get; set; }
      public string SignOrientation { get; set; }
   }
}