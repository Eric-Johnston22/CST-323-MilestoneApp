using Azure.Identity;
using CST_323_MilestoneApp.Controllers;
using CST_323_MilestoneApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace CST_323_MilestoneApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add user secrets configuration for local development
            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>();
            }
            else
            {
                // Add Azure Key Vault configuration
                var keyVaultName = builder.Configuration["KeyVaultName"];
                var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

                builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
            }

            builder.Services.AddScoped<BookDAO>();
            builder.Services.AddScoped<AuthorDAO>();
            builder.Services.AddScoped<UserDAO>();

            // Retrieve the connection string
            var connectionString = builder.Configuration.GetConnectionString("LibraryContext");
            Console.WriteLine($"Connection String: {connectionString}");  // Debug output


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
