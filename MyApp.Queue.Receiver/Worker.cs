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
                ReceiveMessageResponse response = await _sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    WaitTimeSeconds = 3,
                    QueueUrl = QueueConfig.QueueUrl,
                    MaxNumberOfMessages = 10
                });
                if(response.HttpStatusCode == System.Net.HttpStatusCode.OK && 
                    response.Messages.Count>0)
                {
                    foreach(var message in response.Messages)
                    {
                        Profile profile = JsonConvert.DeserializeObject<Profile>(message.Body);
                        if (profile != null)
                        {
                            Console.WriteLine($"\nID: {profile.Id}, Name: {profile.Name}, Email: {profile.Email}");
                        }
                        await _sqsClient.DeleteMessageAsync(QueueConfig.QueueUrl, message.ReceiptHandle);
                    }
                }
                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
