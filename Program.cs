using Azure.Identity;
using CST_323_MilestoneApp.Controllers;
using CST_323_MilestoneApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;
using System;

namespace CST_323_MilestoneApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Configure logging
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.AddAzureWebAppDiagnostics(); // Add Azure Web App logging

            // Ensure logging to Azure App Services
            builder.Services.Configure<AzureFileLoggerOptions>(options =>
            {
                options.FileName = "azure-diagnostics-";
                options.FileSizeLimit = 50 * 1024; // 50 MB
                options.RetainedFileCountLimit = null;
            });
            builder.Services.Configure<AzureBlobLoggerOptions>(options =>
            {
                options.BlobName = "log.txt";
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var environment = builder.Environment;



            //Add Azure Key Vault configuration
            var keyVaultName = builder.Configuration["KeyVaultName"];
            var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
            Console.WriteLine($"Key Vault name: {keyVaultName}");

            builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

            builder.Services.AddScoped<BookDAO>();
            builder.Services.AddScoped<AuthorDAO>();
            builder.Services.AddScoped<UserDAO>();

            // Retrieve the connection string
            //var connectionString = builder.Configuration.GetConnectionString("LibraryContext");
            var connectionString = builder.Configuration["LibraryContext"];
            


            builder.Services.AddDbContext<LibraryContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                    .UseLoggerFactory(LibraryContext.MyLoggerFactory) // Use the logger factory
                    .EnableSensitiveDataLogging() // Enable detailed logging
            );

            // Add services to the container
            builder.Services.AddDbContext<LibraryContext>(options =>
                options.UseMySQL(connectionString));

            // Add authentication services
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/User/Login";
                    options.AccessDeniedPath = "/User/Login";
                });

            var app = builder.Build();
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            // Log the current environment
            logger.LogInformation($"Current Environment: {environment.EnvironmentName}");

            logger.LogInformation($"Connection String: {connectionString}");  // Log the connection string

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Add authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
