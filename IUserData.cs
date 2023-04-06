using KM.GD.PrintInvoices.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KM.GD.PrintInvoices
{
   public interface IUserData 
   {
      Task<IEnumerable<User>> Get();
      Task<User> GetCustomerById(int id);
      Task<User> Update(User user);
      Task<User> Add(User user);
      Task<int> Delete(int userId);
   }
}