using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KM.GD.PrintInvoices.Models.Users
{
   public class RegisterRequest
   {
      [Required]
      public string FirstName { get; set; }

      [Required]
      public string LastName { get; set; }

      [Required]
      public string Username { get; set; }

      
      public string Password { get; set; }
   }
}
