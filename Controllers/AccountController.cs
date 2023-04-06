using KM.GD.PrintInvoices.Entities;
using KM.GD.PrintInvoices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Principal;

namespace KM.GD.PrintInvoices.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class AccountController : ControllerBase
   {
      private readonly ILogger<UsersController> _logger;
      private readonly IConfiguration _configuration;

      public AccountController(ILogger<UsersController> logger, IConfiguration configuration)
      {
         _logger = logger;
         _configuration = configuration;
      }
      // GET: api/<AccountController>
      /// <summary>
      /// Return the current windows user
      /// </summary>
      /// <returns></returns>
      [HttpGet]
      public User Get()
      {
         _logger.LogInformation("START - Get Windows user");
         User user = new User();
         IPrincipal p = HttpContext.User;
         user.Username = p.Identity.Name;
         //user.FirstName = "First Name";
         //user.LastName = "Last Name";
         //user.UpdatedBy = "Me";
         //user.Token = "my-user-token";
         _logger.LogDebug("Username: {0}; AuthenticationType: {1}; IsAuthenticated: {2} ", p.Identity.Name, p.Identity.AuthenticationType, p.Identity.IsAuthenticated);
         _logger.LogInformation("END - GetPrinters");
         return user;
      }
      [HttpGet("ClientAppSettings")]
      public IActionResult ClientAppSettings()
      {
         return Ok(new Dictionary<string, object> {
            { "ApiServer", _configuration["WebClientConfig:ApiServer"] } ,
            { "AdminUsers", _configuration.GetSection("WebClientConfig:AdminUsers").Get<string[]>() },
            { "WfStatePrinted", _configuration["WebClientConfig:WfStatePrinted"] },
            { "WfStateDelivered", _configuration["WebClientConfig:WfStateDelivered"] },
            { "PageSizeOptions", _configuration.GetSection("WebClientConfig:PageSizeOptions").Get<int[]>() }
         });
      }
   }
}