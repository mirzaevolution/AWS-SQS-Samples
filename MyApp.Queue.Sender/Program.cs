using System;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using MyApp.Models;
using Newtonsoft.Json;

namespace MyApp.Queue.Sender
{
    class Program
    {
        private static IAmazonSQS _sqsClient;
        static Program()
        {
            _sqsClient = new AmazonSQSClient(QueueConfig.AccessKey, QueueConfig.AccessSecret);
        }
        static void Main(string[] args)
        {
            string name = string.Empty;
            string email = string.Empty;
            Console.WriteLine("Press ctrl+c to stop sending queue.");
            while (true)
            {
                Console.Write("\nEnter name: ");
                name = Console.ReadLine();
                Console.Write("Enter email: ");
                email = Console.ReadLine();
                SendMessageResponse response = _sqsClient.SendMessageAsync(new SendMessageRequest
                {
                    QueueUrl = QueueConfig.QueueUrl,
                    MessageBody = JsonConvert.SerializeObject(new Profile
                    {
                        Name = name,
                        Email = email
                    })
                }).Result;
                if(response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("Queue sent successfully");
                }
                else
                {

                    Console.WriteLine("Queue failed to send");
                }
            }
        }
    }
}
