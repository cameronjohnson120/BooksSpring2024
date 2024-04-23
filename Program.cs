using BooksSpring2024.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

namespace BooksSpring2024
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //1) fetch information about the connection string
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");
            //2) Add the context class to the set of services and define the option to use sql server on that connection string that has been fetched in the previous step 
            builder.Services.AddDbContext<BooksDBContext>(options => options.UseSqlServer(connString));

            //stripe information
            //fetch information from configuration file and plug it into the stripe settings file
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));




            //use add identity to add the idea of roles within the database
            builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<BooksDBContext>().AddDefaultTokenProviders();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account.AccessDenied";
            });



            builder.Services.AddRazorPages();
            builder.Services.AddScoped<IEmailSender, EmailSender>(); 
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

            app.UseAuthentication(); //checking credentials 
            app.UseAuthorization(); //access to a particular functionality or not/permissions to do something within the app
            app.MapRazorPages();

            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
