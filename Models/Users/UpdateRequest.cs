using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KM.GD.PrintInvoices.Models.Users
{
   public class UpdateRequest
   {
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public string Username { get; set; }
      public string Password { get; set; }
   }
}
