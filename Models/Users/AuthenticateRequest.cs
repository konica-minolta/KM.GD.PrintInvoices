using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KM.GD.PrintInvoices.Models.Users
{
   public class AuthenticateRequest
   {
      [Required]
      public string Username { get; set; }

    
      public string Password { get; set; }
   }
}
