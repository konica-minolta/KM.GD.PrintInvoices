using System.ComponentModel.DataAnnotations;

namespace KM.GD.PrintInvoices.Entities
{
   public class User 
   {
      [Required]
      public int UserId { get; set; }
      [Required, StringLength(255)]
      public string Username { get; set; }
      [ StringLength(255)]
      public string Password { get; set; }

      [ StringLength(100)]
      public string FirstName { get; set; }
      [ StringLength(100)]
      public string LastName { get; set; }
      [ StringLength(255)]
      public string Token { get; set; }

      [StringLength(80)]
      public string UpdatedBy { get; set; }  //record the windows account that updated this data
   }
}