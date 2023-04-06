using KM.GD.PrintInvoices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KM.GD.PrintInvoices.Controllers
{
   [AllowAnonymous]
   [Route("api/[controller]")]
   [ApiController]
   public class PrintWorkflowController : ControllerBase
   {
      private readonly ILogger<PrintFilesController> _logger;
      // requires using Microsoft.Extensions.Configuration;
      private readonly IConfiguration _configuration;
      private readonly PrintInvoiceContext _context;
      public PrintWorkflowController(ILogger<PrintFilesController> logger, IConfiguration configuration, PrintInvoiceContext context)
      {
         _logger = logger;
         _configuration = configuration;
        
         //bypass ssl validation check globally for whole application.
         System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
         _context = context;
      }
      [HttpGet("setwfstate")]
      public IActionResult SetWfStateToItem(string orderItemId, string wfstate)
      {
         try
         {
            // need to using a projection. Include always loads the whole collection
            // with additional "where" exclude the partents with empty child-*
            var _pPrintedOrder = _context.PrintedOrders.Where(p=>p.PritedOrderItems.Any(opi => opi.ORDER_ITEM_ID == orderItemId)).Select(op => new { PrintedOrder = op, PrintedOrderItems = op.PritedOrderItems.Where(opi => opi.ORDER_ITEM_ID == orderItemId) }) ;         
            foreach (var itemPO in _pPrintedOrder)
            {
               itemPO.PrintedOrder.STATE = wfstate;
               foreach (var itemPOI in itemPO.PrintedOrderItems)
               {
                  itemPOI.STATE = wfstate;
               }
            } 
            _context.SaveChanges();
            return Ok();
         }
         catch (Exception e )
         {
            return StatusCode(500, e.Message);
         }     
      }
   }
}
