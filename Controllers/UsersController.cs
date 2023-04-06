using KM.GD.PrintInvoices.Entities;
using KM.GD.PrintInvoices.Helpers;
using KM.GD.PrintInvoices.Models;
using KM.GD.PrintInvoices.Models.Users;
using KM.GD.PrintInvoices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Principal;

namespace KM.GD.PrintInvoices.Controllers
{
   //[Authorize]
   //[AllowAnonymous]
   [Route("api/[controller]")]
   [ApiController]
   public class UsersController : ControllerBase
   {
      private IUserService _userService;

      private readonly ILogger<UsersController> _logger;
      // requires using Microsoft.Extensions.Configuration;
      private readonly IConfiguration _configuration;
      private readonly IOptions<ConfigDocumentsType> _docsSetting;
      private readonly UserContext _context;

      public UsersController(IUserService userService, ILogger<UsersController> logger, IConfiguration configuration, IOptions<ConfigDocumentsType> docSettings, UserContext context)
      {
         _userService = userService;
         _logger = logger;
         _configuration = configuration;
         _docsSetting = docSettings;
         //bypass ssl validation check globally for whole application.
         System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
         _context = context;
      }

      //[AllowAnonymous]
      [HttpPost("authenticate")]
      public IActionResult Authenticate(AuthenticateRequest model)
      {
         try
         {
            var response = _userService.Authenticate(model);
            return Ok(response);
         }
         catch (AppException ae)
         {

            return Forbid();
         }
        
        
      }

      //[AllowAnonymous]
      [HttpPost("register")]
      public IActionResult Register(RegisterRequest model)
      {
         _userService.Register(model);
         return Ok(new { message = "Registration successful" });
      }

      [HttpGet]
      public IActionResult GetAll()
      {
         var users = _userService.GetAll();
         return Ok(users);
      }

      [HttpGet("{id}")]
      public IActionResult GetById(int id)
      {
         var user = _userService.GetById(id);
         return Ok(user);
      }

      [HttpPut("{id}")]
      public IActionResult Update(int id, UpdateRequest model)
      {
         _userService.Update(id, model);
         return Ok(new { message = "User updated successfully" });
      }

      [HttpDelete("{id}")]
      public IActionResult Delete(int id)
      {
         _userService.Delete(id);
         return Ok(new { message = "User deleted successfully" });
      }
   }
}