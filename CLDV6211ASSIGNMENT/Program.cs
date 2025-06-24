using Azure.Storage.Blobs;
using CLDV6211ASSIGNMENT.Models;
using Microsoft.EntityFrameworkCore;

namespace CLDV6211ASSIGNMENT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // Database Context
            builder.Services.AddDbContext<ApplicationDBcontext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Blob Storage Service
            builder.Services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                return new BlobServiceClient(config["AzureBlobStorage:ConnectionString"]);
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
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.UseStaticFiles();
            app.Run();
        }
    }
}
