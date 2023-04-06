using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Options;
using KM.GD.PrintInvoices.Models;
using System.Threading.Tasks;

using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing.Imaging;
using RestSharp;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Principal;
using System.Text;

namespace KM.GD.PrintInvoices.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class PrintFilesController : ControllerBase
   {
      private readonly ILogger<PrintFilesController> _logger;
      // requires using Microsoft.Extensions.Configuration;
      private readonly IConfiguration _configuration;
      private readonly IOptions<ConfigDocumentsType> _docsSetting;
      private readonly PrintInvoiceContext _context;

      public PrintFilesController(ILogger<PrintFilesController> logger, IConfiguration configuration, IOptions<ConfigDocumentsType> docSettings, PrintInvoiceContext context)
      {
         _logger = logger;
         _configuration = configuration;
         _docsSetting = docSettings;
         //bypass ssl validation check globally for whole application.
         System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
         _context = context;
      }

      private DocSettings GetSettingByFileName(string fileName)
      {
         foreach (DocSettings itemDocSetting in _docsSetting.Value.Settings)
         {
            foreach (string itemtype in itemDocSetting.DocType)
            {
               if (fileName.Contains(itemtype))
               {
                  return itemDocSetting;
               }
            }
         }
         return _docsSetting.Value.Settings[0];
      }

      private string GetIcon(string fileName)
      {
         switch (Path.GetExtension(fileName).Substring(1))
         {
            case "pdf":
               return "file-pdf";
            case "xls":
            case "xlsx":
               return "file-excel";         
            default:
               return "file-alt";
         }
        
      }

      //[System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
      //private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword,
      //int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

      //[System.Runtime.InteropServices.DllImport("kernel32.dll")]
      //private static extern Boolean CloseHandle(IntPtr hObject);

      [HttpGet]
      public IActionResult GetFiles(string fileNameList)
      {
         List<PrintInvoiceFiles> retVal = new List<PrintInvoiceFiles>();
         try
         {
            _logger.LogInformation("START - Search file name: {0}", fileNameList);

            string folderToday = _configuration["TodayFolderPath"];
            string folderThisYear = _configuration["ThisYearFolderPath"];
            string currentUser = WindowsIdentity.GetCurrent().Name;
            _logger.LogDebug("Try to access with user: {0}", currentUser);
            _logger.LogDebug("Configured folders: TODAY: {0}, THIS YEAR: {1}", folderToday, folderThisYear);
           
            PrintInvoiceFiles pf = new PrintInvoiceFiles();


            //TO OPTIMIZE RESULT DURING SEARCH 
            //
            //foreach (string itemFile in Directory.EnumerateFiles(folder, string.Format("*{0}*.pdf; *{0}*.xls; *{0}*.xlsx", fileName)))
            //{
            //   string _fileName = Path.GetFileName(itemFile);

            //   PrintInvoiceFiles pif = new PrintInvoiceFiles();
            //   pif.FileName = _fileName;
            //   retVal.Add(pf);
            //} 


            //Regex reg = new Regex("("+fileName+")");

            //List<string> files = Directory.GetFiles(folder, string.Format("*{0}*.pdf; *{0}*.xls; *{0}*.xlsx", fileName)).ToList();
            //.Where(path => reg.IsMatch(path))
            //.ToList();


            //UserCredentials credentials = new UserCredentials(domain, username, password);
            //using SafeAccessTokenHandle userHandle = credentials.LogonUser(LogonType.Interactive);
            //var someResult = System.Security.Principal.WindowsIdentity.RunImpersonated(userHandle, () => {
            //   // do whatever you want as this user.
            //   return something;
            //});

            if (!Directory.Exists(folderToday))
            {
               Directory.CreateDirectory(folderToday);
               _logger.LogWarning("Created Folder: {0}", folderToday);
            }
            if (!Directory.Exists(folderThisYear))
            {
               Directory.CreateDirectory(folderThisYear);
               _logger.LogWarning("Created Folder: {0}", folderThisYear);
            }

            //IntPtr token = IntPtr.Zero;
            //var success = LogonUser("username", "domainname", "password", 2, 0, ref token);
            //if (success)
            //{

            //   using (WindowsImpersonationContext person = new WindowsIdentity(token).Impersonate())
            //   {
            //      string[] allImgs = Directory.GetFiles(@"\\remotemachine\share\folder");

            //      person.Undo();
            //      CloseHandle(token);
            //   }
            //}

            string[] arrfileName = fileNameList.Split(',');
            List<string> files = new List<string>();
            foreach (string fileName in arrfileName)
            {
               files.AddRange(Directory.GetFiles(folderToday, string.Format("*{0}*", fileName.Trim())).ToList());
               files.AddRange(Directory.GetFiles(folderThisYear, string.Format("*{0}*", fileName.Trim())).ToList());
            }
            _logger.LogDebug("File list: {0}", files);
            _logger.Log(LogLevel.Debug, "File list: {0}", files);
            retVal = ConvertFilesFullPathToObj(files, DateTime.Now.Year);

         }
         catch (Exception e)
         {
            _logger.LogCritical(e, "Exception in Function - GetFiles - while searcing name: {0}", fileNameList);
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseBase { Message = e.Message });
         }
         return Ok( retVal);
      }
      [HttpGet("ondb")]
      public List<PrintInvoiceFiles> GetFilesOnDb(string fileName)
      {
         _logger.LogInformation("START - Search File on DB: {0}", fileName);
         List<PrintInvoiceFiles> retVal = new List<PrintInvoiceFiles>();
         PrintInvoiceFiles pf = new PrintInvoiceFiles();
         //string folder = _configuration["PastYearsFolderPath"];
         //pf.FileName = "MyDB_FileTEST_"+fileName ;
         //pf.NumDocs = 1;
         //pf.InvoiceFiles = new List<InvoiceFile>() { new InvoiceFile() { FileName = fileName } };or
         //retVal.Add(pf);

         List<IndexedFile> listIdexedFiles = _context.IndexedFiles.Where(f => f.FILE_NAME.Contains(fileName)).ToList();
         retVal = ConvertFilesFullPathToObj( listIdexedFiles.Select(fl => fl.FULL_PATH).ToList(), DateTime.Now.Year-1);
         return retVal;
      }

      [HttpGet("searchInAllPath")]
      public IActionResult GetFilesInAllPath(string fileNameList)
      {
         List<PrintInvoiceFiles> retVal = new List<PrintInvoiceFiles>();
         try
         {
            _logger.LogInformation("START - Search file name: {0}", fileNameList);

            string folderToday = _configuration["TodayFolderPath"];
            string folderThisYear = _configuration["ThisYearFolderPath"];
            string currentUser = WindowsIdentity.GetCurrent().Name;
            _logger.LogDebug("Try to access with user: {0}", currentUser);
            _logger.LogDebug("Configured folders: TODAY: {0}, THIS YEAR: {1}", folderToday, folderThisYear);

            PrintInvoiceFiles pf = new PrintInvoiceFiles();

            if (!Directory.Exists(folderToday))
            {
               Directory.CreateDirectory(folderToday);
               _logger.LogWarning("Created Folder: {0}", folderToday);
            }
            if (!Directory.Exists(folderThisYear))
            {
               Directory.CreateDirectory(folderThisYear);
               _logger.LogWarning("Created Folder: {0}", folderThisYear);
            }

            string[] arrfileName = fileNameList.Split(',');
            List<string> files = new List<string>();
            foreach (string fileName in arrfileName)
            {
               if (fileName.Trim() != string.Empty)
               {
                  List<string> currFilesRes = new List<string>();
                  currFilesRes.AddRange(Directory.GetFiles(folderToday, string.Format("*{0}*", fileName.Trim())).ToList());
                  currFilesRes.AddRange(Directory.GetFiles(folderThisYear, string.Format("*{0}*", fileName.Trim())).ToList());
                  // if I do not found any invoice (or PF) file in above folder
                  if (GetBaseRootFiles(currFilesRes).Count()<=0)
                  {
                     List<IndexedFile> listIdexedFiles = _context.IndexedFiles.Where(f => f.FILE_NAME.Contains(fileName)).ToList();
                     currFilesRes.AddRange(listIdexedFiles.Select(fl => fl.FULL_PATH).ToList());
                     retVal.AddRange(ConvertFilesFullPathToObj(currFilesRes, DateTime.Now.Year - 1));
                  }
                  else
                  {
                     _logger.LogDebug("File list: {0}", currFilesRes);
                     _logger.Log(LogLevel.Debug, "File list: {0}", currFilesRes);
                     retVal.AddRange(ConvertFilesFullPathToObj(currFilesRes, DateTime.Now.Year));
                  }

                  //if (currFilesRes.Count == 0)
                  //{
                  //   List<IndexedFile> listIdexedFiles = _context.IndexedFiles.Where(f => f.FILE_NAME.Contains(fileName)).ToList();
                  //   retVal.AddRange(ConvertFilesFullPathToObj(listIdexedFiles.Select(fl => fl.FULL_PATH).ToList(), DateTime.Now.Year - 1));
                  //}
                  //else
                  //{
                  //   _logger.LogDebug("File list: {0}", currFilesRes);
                  //   _logger.Log(LogLevel.Debug, "File list: {0}", currFilesRes);
                  //   retVal.AddRange(ConvertFilesFullPathToObj(currFilesRes, DateTime.Now.Year));
                  //}
               }
            }
         }
         catch (Exception e)
         {
            _logger.LogCritical(e, "Exception in Function - GetFiles - while searcing name: {0}", fileNameList);
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseBase { Message = e.Message });
         }
         return Ok(retVal.OrderBy(p => p.FileName));
      }

      [HttpPost("printedfilelist")]
      public PagedResult<PrintedOrder> GetPritedOrder(FilterOnPrintedOrder filterOnPrintedOrder)
      {
         if (filterOnPrintedOrder.DateTo == DateTime.MinValue)
         {
            filterOnPrintedOrder.DateTo = DateTime.Now;
         }
         if (filterOnPrintedOrder.DateFrom == DateTime.MinValue)
         {
            filterOnPrintedOrder.DateFrom = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
         }
         filterOnPrintedOrder.DateFrom = new DateTime(filterOnPrintedOrder.DateFrom.Year, filterOnPrintedOrder.DateFrom.Month, filterOnPrintedOrder.DateFrom.Day, 0, 0, 0);
         filterOnPrintedOrder.DateTo = new DateTime(filterOnPrintedOrder.DateTo.Year, filterOnPrintedOrder.DateTo.Month, filterOnPrintedOrder.DateTo.Day,23, 59, 59);
         var q = _context.PrintedOrders.Include(item => item.PritedOrderItems).Where(i => i.INVOICE_FILE.Contains(filterOnPrintedOrder.InvoiceName) && ( i.DATE >= filterOnPrintedOrder.DateFrom && i.DATE <= filterOnPrintedOrder.DateTo ) ).Select(p => p).OrderByDescending(p => p.DATE).ToList();

         PagedResult<PrintedOrder> retVal = new PagedResult<PrintedOrder>();
         retVal.Results= q
        .Skip((filterOnPrintedOrder.PageNumber ) * filterOnPrintedOrder.PageSize)
        .Take(filterOnPrintedOrder.PageSize)
        .ToList();

         retVal.RowCount = q.Count();
         retVal.PageSize = filterOnPrintedOrder.PageSize;
         retVal.PageCount = (int)Math.Ceiling((decimal)(retVal.RowCount / retVal.PageSize));
         return retVal;
      }

      [HttpGet("reprintorder")]
      public ResponseBase ReprintOrder(int orderId)
      {
         try
         {
            _logger.LogInformation("START - ReprintOrder: internal orderId: {0}; ", orderId);

            var myQuery = _context.PrintedOrders.Include(item => item.PritedOrderItems).Where(i => i.ID == orderId);
            //reprint all document
            RestClient client = new RestClient(_configuration["UrlAccurio"]);
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.ClearHandlers();

            CustomJsonSerializer cs = new CustomJsonSerializer();
            foreach (PrintedOrder itemPO in myQuery)
            {
               PrintedOrder pdo = new PrintedOrder();
               pdo.INVOICE_FILE = itemPO.INVOICE_FILE;
               pdo.ISSUBMISSIONVALID = itemPO.ISSUBMISSIONVALID;
               pdo.N_DOCS = itemPO.N_DOCS;
               pdo.ORDER_ID = itemPO.ORDER_ID;
               pdo.STATE = _configuration["WebClientConfig:WfStatePrinted"]; //"Stampato";
               pdo.DATE = DateTime.Now;
               pdo.PritedOrderItems = new List<PrintedOrderItems>();
               foreach (PrintedOrderItems itemPOI in itemPO.PritedOrderItems)
               {
                  ////set printed status
                  //RestRequest reqChangeStatus = new RestRequest(string.Format("api/order-item/{0}/update", itemPOI.ORDER_ITEM_ID), DataFormat.Json);
                  //reqChangeStatus.Parameters.Clear();
                  //reqChangeStatus.AddHeader("apikey", _configuration["AccurioApiKey"]);
                  //reqChangeStatus.AddHeader("Content-Type", "application/json");
                  //reqChangeStatus.AddBody("{'workStepId': 'STAMPATO', 'stateId': 'STAMPATODONE' }", RestSharp.Serialization.ContentType.Json);
                  //var changeStatusResponse = client.Post<ResponsePrintingOrderItems>(reqChangeStatus);

                  // get ids of WorkFlow and state
                  client.ClearHandlers();
                  var requestGetProducts = new RestRequest("api/products");
                  requestGetProducts.Parameters.Clear();
                  requestGetProducts.AddHeader("apikey", _configuration["AccurioApiKey"]);
                  requestGetProducts.AddHeader("Content-Type", "application/json");
                  requestGetProducts.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

                  var responsePP = client.Get<List<ResponsePrinterProduct>>(requestGetProducts);
                  List<ResponsePrinterProduct> respPP = cs.Deserialize<List<ResponsePrinterProduct>>(responsePP);

                  string wfname = _configuration["ConfigWFState:Pickedup"].ToUpper();// "RITIRATO"; 
                  string stateName = "";

                  var workStep = respPP.SelectMany(w => w.workSteps).Where(ws => ws.name == wfname).FirstOrDefault();
                  var stateOf = workStep.states.Where(s => s.name == stateName).FirstOrDefault();

                  //Updating order item workstep status
                  client.ClearHandlers();
                  var requestSetWF = new RestRequest(string.Format("api/order-item/{0}/update", itemPOI.ORDER_ITEM_ID));
                  requestSetWF.Parameters.Clear();
                  requestSetWF.AddHeader("apikey", _configuration["AccurioApiKey"]);
                  requestSetWF.AddHeader("Content-Type", "application/json");
                  requestSetWF.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
                  AccurioWF awf = new AccurioWF();
                  awf.workStepId = workStep.id;
                  awf.stateId = stateOf.id;
                  requestSetWF.AddJsonBody(awf);
                  var setWFResponse = client.Post(requestSetWF);


                  // Reprint item in accurio
                  RestRequest request = new RestRequest(string.Format("api/order-item/{0}/print", itemPOI.ORDER_ITEM_ID), DataFormat.Json);
                  request.Parameters.Clear();
                  request.AddHeader("apikey", _configuration["AccurioApiKey"]);
                  request.AddHeader("Content-Type", "application/json");
                  AccurioState accurioState = new AccurioState() { status = "read" };
                  request.AddJsonBody(accurioState);
                  var reprintOrderResponse = client.Post(request);
                  
                  
                  //rpoi.details.orderItemId
                  //pdo.PritedOrderItems.Add(new PrintedOrderItems() { ORDER_ITEM_ID = rpoi.details.orderItemId, STATE = rpoi.details.workstep });



                  itemPOI.STATE = _configuration["WebClientConfig:WfStatePrinted"]; //"Stampato";
                  
               }
               itemPO.STATE = _configuration["WebClientConfig:WfStatePrinted"];// "Stampato";
               itemPO.DATE = DateTime.Now;
               //_context.PrintedOrders.Add(pdo);

               _logger.LogInformation("END - ReprintOrder: orderId {0}; orderItemId: {1} ", orderId, string.Join(',', pdo.PritedOrderItems.Select(a => a.ORDER_ITEM_ID).ToArray()));
            }

            _context.SaveChanges();
            return new ResponseBase() { Code = "0", Message = "OK" };
         }
         catch (Exception e)
         {
            _logger.LogCritical(e, "ERROR - ReprintInvoiceFiles internal id: {0}", orderId);
            return new ResponseBase() { Code = "-1", Message = string.Format("Problema con la ristampa del file Errore: {0}", e.Message) };
         }
        
      }
      [HttpGet("downloadfile")]
      public IActionResult DownloadPrintedDocument(string orderId)
      {
         _logger.LogInformation("START - DownloadPrintedDocument: {0}", orderId);
         RestClient client = new RestClient(_configuration["UrlAccurio"]);
         client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
         client.ClearHandlers();
         RestRequest request = new RestRequest(string.Format("api/order-item/{0}/download", orderId), DataFormat.Json);
         request.Parameters.Clear();
         request.AddHeader("apikey", _configuration["AccurioApiKey"]);
         byte[] downloadFileResponse = client.DownloadData(request);
         return File( downloadFileResponse, "application/pdf", orderId+".pdf");
      }

      [HttpGet("printers")]
      public List<Printer> GetPrinters()
      { 
         _logger.LogInformation("START - GetPrinters");
         RestClient client = new RestClient(_configuration["UrlAccurio"]);
         //bypass ssl validation check by using RestClient object
         client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
         client.ClearHandlers();
         RestRequest request = new RestRequest("api/products", DataFormat.Json);
         request.Parameters.Clear();
         request.AddHeader("apikey", _configuration["AccurioApiKey"]);
         request.AddHeader("Content-Type", "application/json");
         request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
         _logger.LogDebug("Request: {0}; {1}; {2} - {3}", request.Parameters, request.Method, request.Resource, request.Body);
         List<Printer> printProductsList;
         if (_configuration.GetValue<bool>("DemoMode:IsDemoMode", false))
         {
            printProductsList = new List<Printer>() { new Printer() { id = "DemoId", name = "DemoMode", description = "Demo Mode Enabled" } };
         }
         else
         {
            IRestResponse response = client.Get(request);
            _logger.LogDebug("Response: {0}; Status:{1}, {2}, {3}; Headers:{4}, Error:{5} {6}; Content:{7}", response.ResponseUri, response.StatusCode, response.StatusDescription, response.ResponseStatus, response.Headers, response.ErrorMessage, response.ErrorException, response.Content);
            CustomJsonSerializer cs = new CustomJsonSerializer();
            printProductsList = cs.DeserializeList<Printer>(response);
            printProductsList.RemoveAll(p => !p.name.Contains(_configuration["AccurioPrinterContainsName"]));
         }
            
         _logger.LogInformation("END - GetPrinters");
         return printProductsList;
      }
      [HttpPost("printinvoice")]
      public ResponseBase PrintInvoiceFiles(PrintInvoiceFilesList filesToPrint)
      {
         _logger.LogInformation("START - PrintInvoiceFiles");
         string strInvoiceFileName = string.Empty;
         try
         {
            string _tempTitlePagePdfFile = Path.GetTempFileName();
            string _tempConcatenatedPdfFile = Path.GetTempFileName();
            strInvoiceFileName = string.Join(",", filesToPrint.PrintInvoiceFiles.OrderBy(fn => fn.FileName).Select(f => f.FileName).ToArray());

            //Create Temp PDF file for the first page 
            CreateTitlePage(strInvoiceFileName, filesToPrint.Note,  _tempTitlePagePdfFile);

            List<string> fileListToPrint = new List<string>() { _tempTitlePagePdfFile };//List of files to concatene in single result file to print
            List<string> tempFileToDelete = new List<string>() { _tempTitlePagePdfFile, _tempConcatenatedPdfFile }; // list of all temp files generated
            List<InvoiceFile> lstFileToPrint = new List<InvoiceFile>();
            foreach (var itemPIF in filesToPrint.PrintInvoiceFiles.OrderBy(fn=>fn.FileName))
            {
               lstFileToPrint.AddRange(itemPIF.InvoiceFiles.OrderBy(p => p.Position));
            }
            //lstFileToPrint.AddRange((List<InvoiceFile>)filesToPrint.PrintInvoiceFiles.Select(invF => invF.InvoiceFiles));

            foreach (InvoiceFile itemInvoiceFile in lstFileToPrint)
            {
               _logger.LogDebug("PrintInvoiceFiles file: {0} ", itemInvoiceFile.FullPath);
               if (itemInvoiceFile.ToSign)
               {
                  string _tempSignedPdfFile = Path.GetTempFileName();
                  SignOnLastPage(itemInvoiceFile.FullPath, _tempSignedPdfFile);
                  tempFileToDelete.Add(_tempSignedPdfFile);
                  //make a renamed copy in today folder
                  string fileSignedCopied = Path.Combine(_configuration["TodayFolderPath"], Path.GetFileNameWithoutExtension(itemInvoiceFile.FileName) + "_SIGNED.PDF");
                  if (System.IO.File.Exists(fileSignedCopied))
                  {
                     System.IO.File.Delete(fileSignedCopied);
                  }
                  System.IO.File.Copy(_tempSignedPdfFile, fileSignedCopied);
                  for (int i = 0; i < itemInvoiceFile.NumCopy; i++)
                  {
                     fileListToPrint.Add(_tempSignedPdfFile);
                  }
               }
               else
               {
                  for (int i = 0; i < itemInvoiceFile.NumCopy; i++)
                  {
                     fileListToPrint.Add(itemInvoiceFile.FullPath);
                  }
               }
            }
            int totalPages;
            CombineMultiplePDFs(fileListToPrint.ToArray(), _tempConcatenatedPdfFile, out totalPages);

            string _tempTitlePageNumberPdfFile = Path.GetTempFileName();
            tempFileToDelete.Add(_tempTitlePageNumberPdfFile);
            SetTotalPageInFirstPage(totalPages.ToString(), _tempConcatenatedPdfFile, _tempTitlePageNumberPdfFile);

            if (_configuration.GetValue<bool>("DemoMode:IsDemoMode", false))
            {
               //If demo mode is configured copy generated file to print
               if (!string.IsNullOrEmpty(_configuration["DemoMode:PathOutput"]))
               {
                  if (!Directory.Exists(_configuration["DemoMode:PathOutput"]))
                  {
                     Directory.CreateDirectory(_configuration["DemoMode:PathOutput"]);
                  }
                  System.IO.File.Copy(_tempTitlePageNumberPdfFile, Path.Combine( _configuration["DemoMode:PathOutput"] , Path.GetFileNameWithoutExtension(_tempTitlePageNumberPdfFile) + ".pdf"));
               }
               else
               {
                  _logger.LogWarning("Check Configuration File: Demo Mode is true but the path is not properly configurated");
               }
            }
            else
            {
               //send print concatened file
               RestClient client = new RestClient(_configuration["UrlAccurio"]);
               client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
               client.ClearHandlers();

               // 1) Upload Stream
               byte[] arrBytes = System.IO.File.ReadAllBytes(_tempTitlePageNumberPdfFile);
               HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(new Uri(_configuration["UrlAccurio"]), "api/uploadStream"));
               webRequest.Method = "POST";
               webRequest.ContentType = "application/json";
               webRequest.Headers.Add("apikey", _configuration["AccurioApiKey"]);
               webRequest.ContentLength = arrBytes.Length;
               using (Stream postStream = webRequest.GetRequestStream())
               {
                  // Send the data.
                  postStream.Write(arrBytes, 0, arrBytes.Length);
                  postStream.Close();
               }
               HttpWebResponse resp = webRequest.GetResponse() as HttpWebResponse;
               UploadFileResponse ufResp;
               CustomJsonSerializer cs = new CustomJsonSerializer();
               if (resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.Created)
               {
                  using (Stream respStream = resp.GetResponseStream())
                  {
                     StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                     string data = reader.ReadToEnd();
                     if (!string.IsNullOrEmpty(data))
                     {
                           ufResp = JsonConvert.DeserializeObject<UploadFileResponse>(data);
                     }
                     else
                     {
                        throw new Exception("Upload file Response is empty");
                     }
                  }
               }
               else
               {
                  throw new Exception(resp.StatusDescription);
               }

               //    var requestUpload = new RestRequest("api/uploadStream", DataFormat.Json);
               // requestUpload.Parameters.Clear();
               // requestUpload.AddHeader("apikey", _configuration["AccurioApiKey"]);
               //// requestUpload.AddHeader("Content-Type", "application/json");
               // requestUpload.AddHeader("Content-Type", "application/octet-stream");
               // requestUpload.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
               // requestUpload.Method = Method.POST;

               // requestUpload.AddFile(filesToPrint.FileName, _tempConcatenatedPdfFile);
               // //var arrBytes = System.IO.File.ReadAllBytes(_tempConcatenatedPdfFile);
               // //requestUpload.AddJsonBody(arrBytes);
               // _logger.LogDebug("Request: {0}; {1}; {2} - {3}", requestUpload.Parameters, requestUpload.Method, requestUpload.Resource, requestUpload.Body);
               // //var response = client.Post(requestUpload);
               // var response = client.Execute(requestUpload);
               // _logger.LogDebug("Response: {0}; Status:{1}, {2}, {3}; Headers:{4}, Error:{5} {6}; Content:{7}", response.ResponseUri, response.StatusCode, response.StatusDescription, response.ResponseStatus, response.Headers, response.ErrorMessage, response.ErrorException, response.Content);
               // CustomJsonSerializer cs = new CustomJsonSerializer();
               // UploadFileResponse ufResp = cs.Deserialize<UploadFileResponse>(response);

               // 2) Create Order

               //webRequest = (HttpWebRequest)WebRequest.Create(new Uri(new Uri(_configuration["UrlAccurio"]), "api/createOrder"));
               //webRequest.Method = "POST";
               //webRequest.ContentType = "application/json";
               //webRequest.Headers.Add("apikey", _configuration["AccurioApiKey"]);

               //PrintOrder po = new PrintOrder();
               //po.orderItems = new List<OrderItem>();
               //po.orderItems.Add(new OrderItem() { product = filesToPrint.PrinterToUse, pageSources = new List<PageSource>() { new PageSource() { tempFileName = ufResp.tempFileName, originalFileName= filesToPrint.FileName } } });
               //po.prefix = "XI";

               //string requestInputSerialized = JsonConvert.SerializeObject(po,new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
               //webRequest.ContentLength = requestInputSerialized.Length;
               //using (var sw = new StreamWriter(webRequest.GetRequestStream()))
               //{
               //   sw.Write(requestInputSerialized);
               //   sw.Flush();
               //   sw.Close();
               //}
               //resp = webRequest.GetResponse() as HttpWebResponse;
               //ResponsePrintOrder rpo;
               //if (resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.Created)
               //{
               //   using (Stream respStream = resp.GetResponseStream())
               //   {
               //      StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
               //      string data = reader.ReadToEnd();
               //      if (!string.IsNullOrEmpty(data))
               //      {
               //         rpo = JsonConvert.DeserializeObject<ResponsePrintOrder>(data);
               //      }
               //      else
               //      {
               //         throw new Exception("Create Order Response is empty");
               //      }
               //   }
               //}
               //else
               //{
               //   throw new Exception(resp.StatusDescription);
               //}

               var requestCreateOrder = new RestRequest("api/createOrder", DataFormat.Json);
               requestCreateOrder.Parameters.Clear();
               requestCreateOrder.AddHeader("apikey", _configuration["AccurioApiKey"]);
               requestCreateOrder.AddHeader("Content-Type", "application/json");
               requestCreateOrder.OnBeforeDeserialization = resp => { resp.ContentType = RestSharp.Serialization.ContentType.Json; };

               PrintOrder po = new PrintOrder();
               po.orderItems = new List<OrderItem>();
               po.orderItems.Add(new OrderItem() { product = filesToPrint.PrinterToUse, pageSources = new List<PageSource>() { new PageSource() { tempFileName = ufResp.tempFileName, originalFileName = strInvoiceFileName } } });
               po.prefix = "XI";

               requestCreateOrder.AddBody(cs.Serialize(po), RestSharp.Serialization.ContentType.Json);
               _logger.LogDebug("Request: {0}; {1}; {2} - {3}", requestCreateOrder.Parameters, requestCreateOrder.Method, requestCreateOrder.Resource, requestCreateOrder.Body);
               var orderResponse = client.Post<ResponsePrintOrder>(requestCreateOrder);
               _logger.LogDebug("Response: {0}; Status:{1}, {2}, {3}; Headers:{4}, Error:{5} {6}; Content:{7}", orderResponse.ResponseUri, orderResponse.StatusCode, orderResponse.StatusDescription, orderResponse.ResponseStatus, orderResponse.Headers, orderResponse.ErrorMessage, orderResponse.ErrorException, orderResponse.Content);
               ResponsePrintOrder rpo = null; ;
               if (orderResponse.IsSuccessful)
               {
                   rpo = cs.Deserialize<ResponsePrintOrder>(orderResponse);
               }
               else
               {
                  foreach (string itemFileToDelete in tempFileToDelete)
                  {
                     System.IO.File.Delete(itemFileToDelete);
                  }
                  throw new Exception(orderResponse.StatusDescription);
               }
               try
               {
                  // get ids of WorkFlow and state
                  client.ClearHandlers();
                  var requestGetProducts = new RestRequest("api/products");
                  requestGetProducts.Parameters.Clear();
                  requestGetProducts.AddHeader("apikey", _configuration["AccurioApiKey"]);
                  requestGetProducts.AddHeader("Content-Type", "application/json");
                  requestGetProducts.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
                  _logger.LogDebug("Request GET PRODUCTS: {0}; {1}; {2} - {3}", requestGetProducts.Parameters, requestGetProducts.Method, requestGetProducts.Resource, requestGetProducts.Body);
                  var responsePP = client.Get<List<ResponsePrinterProduct>>(requestGetProducts);
                  _logger.LogDebug("Response GET PRODUCTS: {0}; Status:{1}, {2}, {3}; Headers:{4}, Error:{5} {6}; Content:{7}", responsePP.ResponseUri, responsePP.StatusCode, responsePP.StatusDescription, responsePP.ResponseStatus, responsePP.Headers, responsePP.ErrorMessage, responsePP.ErrorException, responsePP.Content);
                  List<ResponsePrinterProduct> respPP = cs.Deserialize<List<ResponsePrinterProduct>>(responsePP);

                  string wfname = _configuration["ConfigWFState:Printed"].ToUpper();// "STAMPATO";
                  string stateName = _configuration["ConfigWFState:StateDone"]; // "Done";

                  var workStep = respPP.SelectMany(w => w.workSteps).Where(ws => ws.name.Trim().ToUpper() == wfname).FirstOrDefault();
                  var stateOf = workStep.states.Where(s => s.name.Trim() == stateName).FirstOrDefault();
                  _logger.LogDebug("Try to update Work Step Status: Woerkstep={0}; state= {1}",workStep,stateOf);
                  //Updating order item workstep status
                  client.ClearHandlers();
                  var requestSetWF = new RestRequest(string.Format("api/order-item/{0}/update", rpo.result.orderItemIds[0]));
                  requestSetWF.Parameters.Clear();
                  requestSetWF.AddHeader("apikey", _configuration["AccurioApiKey"]);
                  requestSetWF.AddHeader("Content-Type", "application/json");
                  requestSetWF.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
                  AccurioWF awf = new AccurioWF();
                  awf.workStepId = workStep.id;
                  awf.stateId = stateOf.id;

                  requestSetWF.AddJsonBody(awf, RestSharp.Serialization.ContentType.Json);
                  _logger.LogDebug("Request SET WF State: {0}; {1}; {2} - {3}", requestSetWF.Parameters, requestSetWF.Method, requestSetWF.Resource, requestSetWF.Body);
                  var setWFResponse = client.Post(requestSetWF);
                  _logger.LogDebug("Response SET WF State: {0}; Status:{1}, {2}, {3}; Headers:{4}, Error:{5} {6}; Content:{7}", setWFResponse.ResponseUri, setWFResponse.StatusCode, setWFResponse.StatusDescription, setWFResponse.ResponseStatus, setWFResponse.Headers, setWFResponse.ErrorMessage, setWFResponse.ErrorException, setWFResponse.Content);
               }
               catch (Exception e)
               {
                  _logger.LogError(e, "ERROR - PrintInvoiceFile Update Accurio WF");
               }
         


               PrintedOrder pdo = new PrintedOrder();
               pdo.INVOICE_FILE = strInvoiceFileName;
               pdo.ISSUBMISSIONVALID = rpo.result.isSubmissionValid;
               pdo.ORDER_ID = rpo.result.orderId;
               pdo.N_DOCS = lstFileToPrint.Count;
               pdo.TOTAL_PAGES = totalPages;
               pdo.DATE = DateTime.Now;//.ToString("dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
               pdo.STATE = _configuration["WebClientConfig:WfStatePrinted"];// "Stampato";
               pdo.PritedOrderItems = new List<PrintedOrderItems>();
               foreach (string itemOrder in rpo.result.orderItemIds)
               {
                  pdo.PritedOrderItems.Add(new PrintedOrderItems() { ORDER_ITEM_ID = itemOrder, STATE =  _configuration["WebClientConfig:WfStatePrinted"] });
               }
               _context.PrintedOrders.Add(pdo);
               _context.SaveChanges();
            }

            //Remove temp files
            foreach (string itemFileToDelete in tempFileToDelete)
            {
               System.IO.File.Delete(itemFileToDelete);
            }
            return new ResponseBase() { Code = "0", Message = "OK" };
         }
         catch (Exception e)
         {
            _logger.LogCritical(e, "ERROR - PrintInvoiceFiles - {0}", filesToPrint);
            return new ResponseBase() { Code = "-1", Message = string.Format("Problema di stampa del file: {0}; Errore: {1}", strInvoiceFileName, e.Message)};
         }
      }
      private void CreateTitlePage(string filesToPrint, string note, string outFile)
      {
         try
         {
            _logger.LogDebug("CreateTitlePage: {0}", filesToPrint);
            System.IO.FileStream fs = new FileStream(outFile, System.IO.FileMode.Create);
            // Create an instance of the document class which represents the PDF document itself.  
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF   
            // Writer class using the document and the filestrem in the constructor.  

            // Add meta information to the document  
            document.AddAuthor("Konica Minolta");
            document.AddCreator("Konica Minolta");
            document.AddKeywords("GD X Invoice");
            document.AddSubject("Document GD X Invoice - Printed documents");
            document.AddTitle("Document GD X Invoice - Printed documents");

            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Open the document to enable you to write to the document  
            document.Open();
            // Get signed image configured
            _logger.LogDebug("LogoImagePath");
            string imageURL = _configuration["LogoImagePath"];
            var preImage = System.Drawing.Image.FromFile(imageURL);
            var image = Image.GetInstance(preImage, ImageFormat.Png);
            preImage.Dispose();
            image.ScaleAbsolute(200, 60);
            image.SetAbsolutePosition(30, PageSize.A4.Height - image.ScaledHeight - 30);
            image.SpacingAfter = 12f;
            document.Add(image);
            Font verdana = FontFactory.GetFont("Verdana", 20, Font.BOLD, BaseColor.Black);
            Paragraph parTitle = new Paragraph("Fattura numero: " + filesToPrint, verdana);
            parTitle.Alignment = Element.ALIGN_CENTER;
            parTitle.SpacingBefore = 80f;
            document.Add(parTitle);
            document.Add(new Paragraph("Note inserite", verdana));
            // Add a invoice print note 
            if (note != string.Empty)
            {
               _logger.LogDebug("Add edited note: {0}", note);
               Paragraph parNote = new Paragraph(note);
               parNote.Font = FontFactory.GetFont("Verdana", 14, Font.NORMAL, BaseColor.Black);
               parNote.Alignment = Element.ALIGN_JUSTIFIED;
               parNote.SpacingBefore = 12f;
               document.Add(parNote);
            }

            // Close the document  
            document.Close();
            // Close the writer instance  
            writer.Close();
            // Always close open filehandles explicity  
            fs.Close();
            fs.Dispose();
         }
         catch (Exception e)
         {
            _logger.LogCritical(e, "ERROR - CreateTitlePage - {0}", filesToPrint);
            throw ;
         }
      }
      private void CombineMultiplePDFs(string[] fileNames, string outFile, out int totalPagesNumber)
      {
         totalPagesNumber = 0;
         try
         {
            _logger.LogDebug("CombineMultiplePDFs: {0}", fileNames);
            // step 1: creation of a document-object
            Document document = new Document();
            //create newFileStream object which will be disposed at the end
            using (FileStream newFileStream = new FileStream(outFile, FileMode.Create))
            {
               // step 2: we create a writer that listens to the document
               PdfCopy writer = new PdfCopy(document, newFileStream);

               // step 3: we open the document
               document.Open();

               foreach (string fileName in fileNames)
               {
                  _logger.LogDebug("Concatene file: {0}", fileName);
                  // we create a reader for a certain document
                  PdfReader reader = new PdfReader(fileName);
                  reader.ConsolidateNamedDestinations();
                  totalPagesNumber += reader.NumberOfPages;
                  // step 4: we add content
                  for (int i = 1; i <= reader.NumberOfPages; i++)
                  {
                     PdfImportedPage page = writer.GetImportedPage(reader, i);
                     writer.AddPage(page);
                  }
                  PrAcroForm form = reader.AcroForm;
                  if (form != null)
                  {
                     writer.CopyAcroForm(reader);
                  }
                  reader.Close();
               }

               // step 5: we close the document and writer
               writer.Close();
               document.Close();
            }//disposes the newFileStream object
         }
         catch (Exception e)
         {
            _logger.LogCritical(e, "ERROR - CombineMultiplePDFs");
            throw;
         }
      }
      private void SignOnLastPage(string fileToSign, string fileSigned)
      {
         try
         {
            // Get signed image configured
            string imageURL = _configuration["Sign:Path"];
            string signWidth = _configuration["Sign:Width"];
            string signHeight = _configuration["Sign:Height"]; 
            string signPosX = _configuration["Sign:PositionX"];
            string signPosY = _configuration["Sign:PositionY"];
            float fSignWidth, fSignHeight;

            int posX, posY;

            //To acheve image transparency in pdf, image has to be opened in a correct format
            var preImage = System.Drawing.Image.FromFile(imageURL);
            var image = Image.GetInstance(preImage, ImageFormat.Png);
            preImage.Dispose();

            if(!float.TryParse(signWidth, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out fSignWidth))
            {
               fSignWidth = 100;
            }
            if(!float.TryParse(signHeight, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out fSignHeight))
            {
               fSignHeight = 100;
            }
            if (!int.TryParse(signPosX, out posX))
            {
               posX = 10;
            }
            if (!int.TryParse(signPosY, out posY))
            {
               posY = 10;
            }
            //load pdf file
            byte[] pdfBytes = System.IO.File.ReadAllBytes(fileToSign);
            PdfReader oldFile = new PdfReader(pdfBytes);

            //optional: if image is wider than the page, scale down the image to fit the page
            var sizeWithRotation = oldFile.GetPageSizeWithRotation(oldFile.NumberOfPages);
            if (image.Width > sizeWithRotation.Width)
               image.ScalePercent(sizeWithRotation.Width / image.Width * 100);

            image.ScaleAbsolute(fSignWidth, fSignHeight);
            //set image position in top left corner
            //NB: in pdf files, cooridinates start in the left bottom corner
            image.SetAbsolutePosition(sizeWithRotation.Width-image.ScaledWidth-posX,  posY);

            // Change to use MemoryStream
            using (var newFileStream = new FileStream(fileSigned, FileMode.Create))
            {
               //setup PdfStamper
               var stamper = new PdfStamper(oldFile, newFileStream);

               ////iterate through the pages in the original file
               //for (var i = 1; i <= oldFile.NumberOfPages; i++)
               //{
               //   //get canvas for current page
               //   var canvas = stamper.GetOverContent(i);
               //   //add image with pre-set position and size
               //   canvas.AddImage(image);
               //}

               //get canvas for last page
               PdfContentByte canvas = stamper.GetOverContent(oldFile.NumberOfPages);
               //add image with pre-set position and size
               canvas.AddImage(image);

               stamper.Close();
            }

         }
         catch (Exception e)
         {
            _logger.LogCritical(e, "ERROR - SignOnLastPage - {0}", fileToSign);
            throw;
         }
      }

      private void SetTotalPageInFirstPage(string totPage,string inputFile, string outputFile)
      {
         // open the reader
         PdfReader reader = new PdfReader(inputFile);
         Rectangle firstSize = reader.GetPageSizeWithRotation(1);
         Document document = new Document(firstSize);

         // open the writer
         using (FileStream newFileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
         { 
            PdfWriter writer = PdfWriter.GetInstance(document, newFileStream);
            document.Open();
            string pageText = "Totale Pagine: " + totPage;
            for (var i = 1; i <= reader.NumberOfPages; i++)
            {
               Rectangle pageSize = reader.GetPageSizeWithRotation(i);
               document.SetPageSize(pageSize.Width > pageSize.Height ? PageSize.A4.Rotate() : PageSize.A4);
               document.NewPage();
               PdfImportedPage importedPage = writer.GetImportedPage(reader, i);
               
               // the pdf content
               PdfContentByte cb = writer.DirectContent;
               if (i == 1)
               {
                  // select the font properties
                  BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                  cb.SetColorFill(BaseColor.DarkGray);
                  cb.SetFontAndSize(bf, 8);

                  // write the text in the pdf content
                  cb.BeginText();
                  // put the alignment and coordinates here
                  cb.ShowTextAligned(1, pageText, pageSize.Width - 50, pageSize.Bottom + 40, pageSize.Rotation);
                  cb.EndText();
               }
               switch (pageSize.Rotation)
               {
                  case 0:
                     writer.DirectContent.AddTemplate(importedPage, 1f, 0, 0, 1f, 0, 0);
                     break;

                  case 90:
                     writer.DirectContent.AddTemplate(importedPage, 0, -1f, 1f, 0, 0, pageSize.Height);
                     break;

                  case 180:
                     writer.DirectContent.AddTemplate(importedPage, -1f, 0, 0, -1f, pageSize.Width, pageSize.Height);
                     break;

                  case 270:
                     writer.DirectContent.AddTemplate(importedPage, 0, 1f, -1f, 0, pageSize.Width, 0);
                     break;

                  default:
                     throw new InvalidOperationException(string.Format("Unexpected page rotation: [{0}].", pageSize.Rotation));
               }
            }
            // close the streams and voilá the file should be changed :)
            document.Close();
            newFileStream.Close();
            writer.Close();
            reader.Close();
         }
      }
      private List<PrintInvoiceFiles> ConvertFilesFullPathToObj(List<string> fullPath, int year)
      {
         // Find the root file
         List<PrintInvoiceFiles> retVal = new List<PrintInvoiceFiles>();
         var filesRoot = GetBaseRootFiles(fullPath);
         //var filesRoot = fullPath.Where(f => (!(Path.GetFileName(f)).Contains('_') || (Path.GetFileNameWithoutExtension(f)).ToUpper().EndsWith("_PF")) && (Path.GetExtension(f).ToLower() == ".pdf")).OrderBy(f => f);
         _logger.LogDebug("File Invoice list: {0}", filesRoot);
         foreach (string itemFile in filesRoot)
         {
            // Set the root file
            _logger.LogDebug("Collecting info for file: {0}", itemFile);
            int pos = 1;
            DocSettings settingsConfigInvoice = _docsSetting.Value.Settings.FirstOrDefault(s => s.DisplayName == "Fattura");
            _logger.LogDebug("Configured info:{0}", settingsConfigInvoice.DisplayName);
            PrintInvoiceFiles pif = new PrintInvoiceFiles();
            DocSettings currentDocSetting = GetSettingByFileName(itemFile);
            pif.FileName = Path.GetFileNameWithoutExtension(itemFile);
            pif.ParentFolder = Path.GetFileName(Path.GetDirectoryName(itemFile));
            pif.InvoiceFiles = new List<InvoiceFile>() { new InvoiceFile() {
               FullPath = itemFile,
               Position=settingsConfigInvoice.Position,
               ShortPath =Path.GetFileName( Path.GetDirectoryName( itemFile) ),
               FileName = Path.GetFileName(itemFile),
               Extension = Path.GetExtension(itemFile).Substring(1),
               NumCopy =  settingsConfigInvoice.Copies,
               Type = settingsConfigInvoice.DisplayName,
               ToSign= settingsConfigInvoice.ToSign,
               SignOrientation = settingsConfigInvoice.SignOrientation,
               SignPosition = settingsConfigInvoice.SignPosition,
               Icon= GetIcon(itemFile)
            }};
            pif.NumDocs++;
            // Find all child files

            //Separate root file for every directory
            //var filesChild = fullPath.Where(f => (Path.GetFullPath(f).Contains(Path.Combine(Path.GetDirectoryName(itemFile), pif.FileName) + '_'  ) || 
            //                                 Path.GetFullPath(f).Contains(Path.Combine(Path.GetDirectoryName(itemFile), "SD"+pif.FileName) + '_')|| 
            //                                 Path.GetFullPath(f).Contains(Path.Combine(Path.GetDirectoryName(itemFile), "FI" + year + pif.FileName) + '_') ) &&
            //                                 (!f.Contains("_SIGNED")) &&
            //                                 (Path.GetExtension(f).ToLower() == ".pdf")
            //                                 );
            string normalizedFileName = pif.FileName;
            if (normalizedFileName.ToUpper().EndsWith("_PF"))
            {
               normalizedFileName = normalizedFileName.Substring(0, normalizedFileName.Length - 3);
            }

           // take single root file and child for every directory
           var filesChild = fullPath.Where(f => (Path.GetFileNameWithoutExtension(f).Contains(normalizedFileName + '_') ||
                                              Path.GetFileNameWithoutExtension(f).Contains("SD" + normalizedFileName + '_')  ||
                                              Path.GetFileNameWithoutExtension(f).Contains("FI" + year + normalizedFileName + '_')  ) &&
                                              (!f.Contains("_SIGNED") && !Path.GetFileNameWithoutExtension(f).ToUpper().EndsWith("_PF")) &&
                                              (Path.GetExtension(f).ToLower() == ".pdf")
            );
            _logger.LogDebug("Invoice child files founded: {0}", filesChild);
            // Set child files
            foreach (string itemFileChild in filesChild)
            {
               _logger.LogDebug("Invoice child file: {0}", itemFileChild);
               DocSettings currentChildDocSetting = GetSettingByFileName(itemFileChild);
               pif.InvoiceFiles.Add(new InvoiceFile()
               {
                  FullPath = itemFileChild,
                  Position = currentChildDocSetting.Position,
                  ShortPath = Path.GetFileName(Path.GetDirectoryName(itemFileChild)),
                  FileName = Path.GetFileName(itemFileChild),
                  Extension = Path.GetExtension(itemFileChild),
                  NumCopy = currentChildDocSetting.Copies,
                  Type = currentChildDocSetting.DisplayName,
                  ToSign = currentChildDocSetting.ToSign,
                  SignOrientation = currentChildDocSetting.SignOrientation,
                  SignPosition = currentChildDocSetting.SignPosition,
                  Icon = GetIcon(itemFileChild)
               });
               pif.NumDocs++;
            }
            retVal.Add(pif);
            _logger.LogDebug("END GetFiles result: {0}", retVal);
         }
         return retVal;
      }

      private IEnumerable<string> GetBaseRootFiles(List<string> fullPath)
      {
         return fullPath.Where(f => (!(Path.GetFileName(f)).Contains('_') || (Path.GetFileNameWithoutExtension(f)).ToUpper().EndsWith("_PF")) && (Path.GetExtension(f).ToLower() == ".pdf")).OrderBy(f => f);
      }
   }

   public class FilterOnPrintedOrder
   {
      public int PageNumber { get; set; }
      public int PageSize { get; set; }
      public int Length { get; set; }
      public string InvoiceName { get; set; }
      public DateTime DateFrom { get; set; }
      public DateTime DateTo { get; set; }
   }

   public abstract class PagedResultBase
   {
      public int CurrentPage { get; set; }
      public int PageCount { get; set; }
      public int PageSize { get; set; }
      public int RowCount { get; set; }

      public int FirstRowOnPage
      {

         get { return (CurrentPage - 1) * PageSize + 1; }
      }

      public int LastRowOnPage
      {
         get { return Math.Min(CurrentPage * PageSize, RowCount); }
      }
   }

   public class PagedResult<T> : PagedResultBase where T : class
   {
      public IList<T> Results { get; set; }

      public PagedResult()
      {
         Results = new List<T>();
      }
   }
}