using Microsoft.EntityFrameworkCore;
using summeringsmakker.Data;
using DotNetEnv;
using summeringsmakker.Repository;
using summeringsMakker.Services;
using summeringsmakker.Models;
using summeringsMakker.Repository;

namespace summeringsMakker;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Load environment variables from .env file
        Env.Load("EnvVariables/.env");

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Add DbContext to the services
        AddDatabaseContext(builder);

        // Register your services
        builder.Services.AddScoped<CaseProcessor>();
        builder.Services.AddScoped<ICaseRepository, CaseRepository>(); // Adjust CaseRepository to your actual implementation class
        builder.Services.AddScoped<ICaseSummaryRepository, CaseSummaryRepository>(); // Also ensure you register interfaces, not concrete classes


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
    
    // Add the database context to the services
    public static void AddDatabaseContext(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<SummeringsMakkerDbContext>(options =>
        {
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (connectionString == null)
            {
                throw new InvalidOperationException("Database connection string not found in environment variables.");
            }

            var mysqlVersion = Environment.GetEnvironmentVariable("MYSQL_VERSION");
            if (mysqlVersion == null)
            {
                throw new InvalidOperationException("MySQL version not found in environment variables.");
            }

            var versionNumbers = mysqlVersion.Split('.').Select(int.Parse).ToArray();
            if (versionNumbers.Length != 3)
            {
                throw new InvalidOperationException("MySQL version should be in the format 'major.minor.build'.");
            }

            var serverVersion = new MySqlServerVersion(new Version(versionNumbers[0], versionNumbers[1], versionNumbers[2]));

            options.UseMySql(connectionString, serverVersion);
        });
    }
}