using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KM.GD.PrintInvoices.Models
{
   public class PrintedOrder
   {
      [Key]
      public int ID { get; set; } 
      [Column(TypeName="varchar(250)")]
      public string INVOICE_FILE { get; set; }
      [Column(TypeName = "varchar(50)")]
      public string ORDER_ID { get; set; }
      [Column(TypeName ="int")]
      public int N_DOCS { get; set; }
      [Column(TypeName = "int")]
      public int TOTAL_PAGES { get; set; }
      [Column(TypeName = "datetime")]
      public DateTime DATE { get; set; }
      [Column(TypeName ="bit")]
      public bool ISSUBMISSIONVALID { get; set; }
      [Column(TypeName = "varchar(50)")]
      public string STATE { get; set; }
      public List<PrintedOrderItems> PritedOrderItems { get; set; }
   }
   public class PrintedOrderItems
   {
      [Key]
      public int ID { get; set; }
      [Column(TypeName = "varchar(50)")]
      public string ORDER_ITEM_ID { get; set; }
      [Column(TypeName = "varchar(50)")]
      public string STATE { get; set; }
   }

}
