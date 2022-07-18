using DirectDebitJob.Connections;
using DirectDebitJob.Interfaces;
using DirectDebitJob.Methods;
using DirectDebitJob.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectDebitJob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug()
            .WriteTo.File("Logs\\logs.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
            //CreateHostBuilder(args).Build().Run();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<IDirectDebitProcess, DirectDebitProcess>();
                    services.AddSingleton<ISqlConnect, SqlConnect>();
                    services.AddSingleton<IBasisConnection, BasisConnection>();
                    services.AddSingleton<IAccountUtility, AccountUtility>();                    
                    services.AddSingleton<IApiConnection, ApiConnection>();                    
                    services.AddSingleton<ISecurity, Security>();
                    services.AddSingleton<IDateUtility, DateUtility>();
                });
    }
}
