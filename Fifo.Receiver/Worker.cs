using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
namespace Fifo.Receiver
{
    public class Worker : BackgroundService
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger<Worker> _logger;
        private readonly string _sqsUrl = "https://sqs.ap-southeast-1.amazonaws.com/073943586533/CoreQueue.fifo";


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
                try
                {
                    ReceiveMessageResponse messageResponse =
                    await _sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
                    {
                        QueueUrl = _sqsUrl,
                        MaxNumberOfMessages = 10,
                        WaitTimeSeconds = 2,
                        MessageAttributeNames =
                        {
                            "All"
                        },
                        AttributeNames =
                        {
                            "All"
                        }
                    });
                    if (messageResponse.HttpStatusCode == System.Net.HttpStatusCode.OK &&
                       messageResponse.Messages.Count > 0)
                    {
                        foreach (Message message in messageResponse.Messages)
                        {
                            string groupId = "";
                            string deduplicateId = "";

                            //actually when we set AttributeNames => All, we can get group id and deduplicate id
                            //from there. Here we demonstrate how to use MessageAttributes
                            if(message.MessageAttributes.Count>0 && 
                               message.MessageAttributes.ContainsKey("GroupId") &&
                               message.MessageAttributes.ContainsKey("DeduplicateId"))
                            {
                                groupId = message.MessageAttributes["GroupId"].StringValue;
                                deduplicateId = message.MessageAttributes["DeduplicateId"].StringValue;
                            }
                            _logger.LogInformation($"GroupId: {groupId ?? "-"}, DeduplicateId: {deduplicateId ?? "-"}, Message: {message.Body}");
                            await _sqsClient.DeleteMessageAsync(new DeleteMessageRequest
                            {
                                QueueUrl = _sqsUrl,
                                ReceiptHandle = message.ReceiptHandle
                            });
                        }
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError($"[Error]: {ex.Message}");
                }
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}
