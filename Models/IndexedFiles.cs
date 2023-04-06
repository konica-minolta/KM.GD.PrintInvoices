using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KM.GD.PrintInvoices.Models
{
   public class IndexedFile
   {
      [Key]
      public int ID { get; set; } 
      [Column(TypeName="varchar(255)")]
      public string FILE_NAME { get; set; }
      [Column(TypeName = "varchar(250)")]
      public string FULL_PATH { get; set; }
     
   }

}
