using Microsoft.EntityFrameworkCore;
using System;
namespace KM.GD.PrintInvoices.Models
{
   public class PrintInvoiceContext : DbContext
   {
      public PrintInvoiceContext(DbContextOptions<PrintInvoiceContext> options):base(options)
      {

      }
      public DbSet<PrintedOrder> PrintedOrders { get; set; }
      public DbSet<PrintedOrderItems> PrintedOrderItems { get; set; }
      public DbSet<IndexedFile> IndexedFiles { get; set; }
   }

}