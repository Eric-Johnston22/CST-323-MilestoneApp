using CST_323_MilestoneApp.Controllers;
using CST_323_MilestoneApp.Services;
using Microsoft.EntityFrameworkCore;

namespace CST_323_MilestoneApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add user secrets configuration
            builder.Configuration.AddUserSecrets<Program>();

            builder.Services.AddScoped<BookDAO>();
            builder.Services.AddScoped<AuthorDAO>();

            // Retrieve the connection string
            var connectionString = builder.Configuration.GetConnectionString("LibraryContext");
            Console.WriteLine($"Connection String: {connectionString}");  // Debug output

            // Add services to the container
            builder.Services.AddDbContext<LibraryContext>(options =>
                options.UseMySQL(connectionString));

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
