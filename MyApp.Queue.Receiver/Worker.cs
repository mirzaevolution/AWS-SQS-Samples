using Amazon.SQS;
using Amazon.SQS.Model;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyApp.Queue.Receiver
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IAmazonSQS _sqsClient;

        public Worker(
            IAmazonSQS sqsClient,
            ILogger<Worker> logger)
        {
            _sqsClient = sqsClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"## Getting queue item...");
                
                //It will use long polling and will wait for 10 seconds if no messages avail,
                //Otherwise it will grab the messages immediately...
                ReceiveMessageResponse response = 
                    await _sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    WaitTimeSeconds = 10,
                    QueueUrl = QueueConfig.QueueUrl,
                    MaxNumberOfMessages = 1
                });
                
                Console.WriteLine("## Polled...");
                if(response.HttpStatusCode == System.Net.HttpStatusCode.OK && 
                    response.Messages.Count>0)
                {
                    foreach(var message in response.Messages)
                    {
                        //Console.WriteLine($"#Raw -> {message.Body}");

                        try
                        {
                            Profile profile = JsonConvert.DeserializeObject<Profile>(message.Body);
                            if (profile != null)
                            {
                                Console.WriteLine($"\nID: {profile.Id}, Name: {profile.Name}, Email: {profile.Email}");
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        await _sqsClient.DeleteMessageAsync(QueueConfig.QueueUrl, message.ReceiptHandle);
                    }
                }
                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
