﻿using System.Diagnostics;
using eCommerceConsumerPlayground.Models;
using ECommerceConsumerPlayground.Services;
using ECommerceConsumerPlayground.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;

//String? connectionstring = "";

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        // Read appsettings
        // connectionstring = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("DBSTRING")) ? Environment.GetEnvironmentVariable("DBSTRING") : configuration.GetConnectionString("SqlServer");
        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlServer")));

        // DI services
        services.AddScoped<App>();
        services.AddScoped<IOrderStore, OrderStore>();
        services.AddScoped<IWorkerService, WorkerService>();

        // Definition of startup service
        services.AddHostedService<App>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetService<AppDbContext>();

    // SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionstring);
    // sqlConnectionStringBuilder.InitialCatalog = "master";

    // context.Database.SetConnectionString(sqlConnectionStringBuilder.ConnectionString);


    // Console.WriteLine("Waiting for DB connection...");

    // while (!context.Database.CanConnect())
    // {
    //     int milliseconds = 2000;
    //     Thread.Sleep(milliseconds);
    //     Console.WriteLine("Trying to connect to DB...");
    //     // we need to wait, since we need to run migrations
    // }

    // Console.WriteLine("DB connected");

    // context.Database.SetConnectionString(connectionstring);

    //var context = services.GetRequiredService<AppDbContext>();

    try
    {

        if (context.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("Running migrations");
            context.Database.Migrate();
            Console.WriteLine("Migrations have been successfull");
        }
    } catch (Exception ex)
    {
        Console.WriteLine($"Error: Either the Database does not exist or migrations were not succesfull: {ex.Message}");
    }
}

await host.RunAsync();

public partial class Program { }