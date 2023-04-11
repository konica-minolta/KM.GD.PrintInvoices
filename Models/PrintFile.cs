using System;
using System.Collections.Generic;

namespace KM.GD.PrintInvoices
{
   public class PrintInvoiceFilesList
   {
      public List<PrintInvoiceFiles> PrintInvoiceFiles { get; set; }
      public string Note { get; set; }
      public string PrinterToUse { get; set; }
   }
   public class PrintInvoiceFiles
   {
      public string FileName { get; set; } 
      public int NumDocs { get; set;  }
      public List<InvoiceFile> InvoiceFiles { get; set; }
      public string ParentFolder { get; set; }
   }

   public class InvoiceFile
   {
      public string FullPath { get; set; }
      public int Position { get; set; }
      public string ShortPath { get; set; }
      public string FileName { get; set; }
      public int NumCopy { get; set; }
      public bool ToSign { get; set; }
      public string SignPosition { get; set; }
      public string SignOrientation { get; set; }
      public string Extension { get; set; }
      public string Type { get; set; }
      public string Icon { get; set; }
   }

   public class Printer
   {
      public string id { get; set; }
      public string name { get; set; }
      public string description { get; set; }
      public bool useStandardWorksteps { get; set; }
   }
}
