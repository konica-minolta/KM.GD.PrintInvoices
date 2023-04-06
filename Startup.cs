using KM.GD.PrintInvoices.Models;
using KM.GD.PrintInvoices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace KM.GD.PrintInvoices
{
   public class Startup
   {
      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
      }

      public IConfiguration Configuration { get; }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddControllersWithViews();
         // In production, the Angular files will be served from this directory
         services.AddSpaStaticFiles(configuration =>
         {
            configuration.RootPath = "ClientApp/dist";
         });
         services.Configure<ConfigDocumentsType>(Configuration.GetSection("ConfigDocumentsType"));

         //add windows authentication for http options request
         services.AddAuthentication(IISDefaults.AuthenticationScheme);
         services.AddMvc(config =>
         {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            config.Filters.Add(new AuthorizeFilter(policy));
         });


         services.AddCors();
         
         services.AddDbContext<PrintInvoiceContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("SqlConnection")));
         services.AddDbContext<UserContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("SqlConnection")));
         services.AddScoped<IUserService, UserService>();

         // Register Hosted Services
         //services.AddHostedService<MyHostedServiceA>();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
      {
         var path = Directory.GetCurrentDirectory();
         //string logFileName = Configuration["Logging:LogFileName"];
         //string logPath = Configuration["Logging:LogPath"];
         // loggerFactory.AddFile($"{path}\\{logPath}\\{logFileName}");
         loggerFactory.AddFile(Configuration.GetSection("Logging"));
         //loggerFactory.AddDebug();
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }
         else
         {
            app.UseExceptionHandler("/Error");
         }

         app.UseStaticFiles();
         if (!env.IsDevelopment())
         {
            app.UseSpaStaticFiles();
         }

         app.UseRouting();

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller}/{action=Index}/{id?}");
         });

         app.UseSpa(spa =>
         {
               // To learn more about options for serving an Angular SPA from ASP.NET Core,
               // see https://go.microsoft.com/fwlink/?linkid=864501

               spa.Options.SourcePath = "ClientApp";

            if (env.IsDevelopment())
            {
               spa.UseAngularCliServer(npmScript: "start");
            }
         });
      }
   }
}
