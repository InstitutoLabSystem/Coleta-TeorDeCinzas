using Coleta_TeorDeCinzas.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Coleta_TeorDeCinzas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<BancoContext>
            (options => options.UseMySql(
                "server=novolab.c82dqw5tullb.sa-east-1.rds.amazonaws.com;user id=Sistemas;password=#7847awsE2024;database=labdados",
                Microsoft.EntityFrameworkCore.ServerVersion.Parse("13.2.0-mysql")));

            builder.Services.AddDbContext<QuimicoContext>(options =>
               options.UseMySql(
                 "server=novolab.c82dqw5tullb.sa-east-1.rds.amazonaws.com;user id=Sistemas;password=#7847awsE2024;database=quimico",
                 Microsoft.EntityFrameworkCore.ServerVersion.Parse("13.2.0-mysql")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(
             CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
             {
                 option.LoginPath = "/Acess/Index";
                 option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Acess}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
