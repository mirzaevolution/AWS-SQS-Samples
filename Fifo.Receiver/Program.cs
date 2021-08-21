using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.SQS;
namespace Fifo.Receiver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("** SQS Fifo Receiver **\n");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDefaultAWSOptions(hostContext.Configuration.GetAWSOptions());
                    services.AddAWSService<IAmazonSQS>();
                    services.AddHostedService<Worker>();
                });
    }
}
