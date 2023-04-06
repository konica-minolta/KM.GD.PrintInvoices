using Microsoft.EntityFrameworkCore;
using System;
namespace KM.GD.PrintInvoices.Models
{
   public class WinAuthAngApiDbContext : DbContext
   {
      public DbSet<User> User { get; set; }

      public WinAuthAngApiDbContext(DbContextOptions<WinAuthAngApiDbContext> options)
          : base(options)
      {
      }
   }

}