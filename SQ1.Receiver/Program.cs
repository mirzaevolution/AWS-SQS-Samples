using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Configuration;
using Amazon.SQS;

namespace SQ1.Receiver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("** Queue Host Receiver **\n");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddDefaultAWSOptions(hostContext.Configuration.GetAWSOptions());
                    services.AddAWSService<IAmazonSQS>();
                });
    }
}
