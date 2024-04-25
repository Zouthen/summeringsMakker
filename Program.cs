using Microsoft.EntityFrameworkCore;
using summeringsmakker.Data;
using DotNetEnv;

namespace summeringsMakker;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Load environment variables from .env file
        Env.Load("EnvVariables/.env");

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Add DbContext to the services
        builder.Services.AddDbContext<CaseDbContext>(options =>
            options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECT_STRING")));
            // options.UseSqlServer("Server=localhost;Database=summeringsmakker;User Id=sa;Password=;"));

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