using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SQ1.Receiver.DeadLetter
{
    public class Worker : BackgroundService
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger<Worker> _logger;
        private readonly string _sqsUrl = "https://sqs.ap-southeast-1.amazonaws.com/073943586533/CoreQueue-DeadLetter";


        public Worker(IAmazonSQS sqsClient, ILogger<Worker> logger)
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
                    ReceiveMessageResponse receiveMessageResponse = await _sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
                    {
                        QueueUrl = _sqsUrl,
                        WaitTimeSeconds = 10
                    });
                    if (receiveMessageResponse.HttpStatusCode == System.Net.HttpStatusCode.OK &&
                       receiveMessageResponse.Messages.Count > 0)
                    {
                        foreach (var message in receiveMessageResponse.Messages)
                        {
                            _logger.LogInformation($"Id: {message.MessageId}, Body: {message.Body}");
                            await _sqsClient.DeleteMessageAsync(new DeleteMessageRequest
                            {
                                QueueUrl = _sqsUrl,
                                ReceiptHandle = message.ReceiptHandle
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[SystemError]: {ex.Message}");
                }
                finally
                {

                    await Task.Delay(3000, stoppingToken);
                }
            }
        }
    }
}
