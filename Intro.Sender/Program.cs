using System;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Intro.Models;
using Newtonsoft.Json;

namespace Intro.Sender
{
    class Program
    {
        private static IAmazonSQS _sqsClient;
        private static readonly string _queueUrl = "https://sqs.ap-southeast-1.amazonaws.com/073943586533/basic-queue.fifo";
        static Program()
        {
            _sqsClient = new AmazonSQSClient("AKIARCN3BS3SQ7Q2VIXB", "S/kcyks78OYtHefw4TsPjjocmST5UhuPvF5rXuJJ", RegionEndpoint.APSoutheast1);
            
        }
        static async void ListQueues()
        {
            ListQueuesResponse response = await _sqsClient.ListQueuesAsync("");
            foreach(var queueUrl in response.QueueUrls)
            {
                Console.WriteLine(queueUrl);
            }
        }
        static async void SendQueue(MessagePayload payload)
        {
            string json = JsonConvert.SerializeObject(payload);
            SendMessageResponse response = await _sqsClient.SendMessageAsync(new SendMessageRequest
            {
                QueueUrl = _queueUrl,
                MessageBody = json,
                //MessageGroupId = Guid.NewGuid().ToString() //this is fifo session id
                MessageGroupId = "MyGroup"

            });
            Console.WriteLine($"MessageId: {response.MessageId}");
            Console.WriteLine($"StatusCode: {(int)response.HttpStatusCode}");
            
        }
        static void Main(string[] args)
        {
            //ListQueues();
            SendQueue(new MessagePayload
            {
                Message = "Hello World"
            });
            Console.ReadLine();
        }
    }
}
