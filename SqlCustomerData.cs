using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using KM.GD.PrintInvoices.Models;

namespace KM.GD.PrintInvoices
{
   public class SqlCustomerData : IUserData
   {
      public WinAuthAngApiDbContext DbContext { get; }

      public SqlCustomerData(WinAuthAngApiDbContext dbContext)
      {
         DbContext = dbContext;
      }

      public async Task<User> Add(User customer)
      {
         DbContext.Add(customer);
         await DbContext.SaveChangesAsync();
         return customer;
      }

      public async Task<int> Delete(int userId)
      {
         User c = await this.GetCustomerById(userId);
         if (c != null)
         {
            this.DbContext.Remove(c);
            await DbContext.SaveChangesAsync();
            return userId;
         }
         return -1;
      }

      public async Task<IEnumerable<User>> Get()
      {
         return await DbContext.User.ToListAsync();
      }

      public async Task<User> GetCustomerById(int id)
      {
         User c = await this.DbContext.User.FindAsync(id);
         if (c != null)
            return c;
         return null;
      }

      public async Task<User> Update(User user)
      {
         User c = await GetCustomerById(user.UserId);
         if (c != null)
         {
            c.FirstName = user.FirstName;
            c.LastName = user.LastName;
            c.UpdatedBy = user.UpdatedBy;
            await DbContext.SaveChangesAsync();
            return c;
         }
         return null;
      }
   }
}