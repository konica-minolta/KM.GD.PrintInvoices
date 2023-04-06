using KM.GD.PrintInvoices.Entities;
using Microsoft.EntityFrameworkCore;
using System;
namespace KM.GD.PrintInvoices.Models
{
   public class UserContext : DbContext
   {
      public UserContext(DbContextOptions<UserContext> options):base(options)
      {

      }
      public DbSet<User> Users { get; set; }
   }

}