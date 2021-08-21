using System;
using Amazon.SQS.Model;
using Amazon.SQS;
using Amazon;

namespace SQ1.Sender
{
    class Program
    {
        private static IAmazonSQS _sqsClient;
        private static readonly string _sqsUrl = "https://sqs.ap-southeast-1.amazonaws.com/073943586533/CoreQueue";
        static Program()
        {
            _sqsClient = new AmazonSQSClient(region: RegionEndpoint.APSoutheast1);

        }
        static void SendMessage(string message)
        {
            try
            {
                var sendResult =  _sqsClient.SendMessageAsync(_sqsUrl, message).Result;
                Console.WriteLine($"{sendResult.MessageId} - `{message}`: {sendResult.HttpStatusCode.ToString()}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Error]: {ex.Message}");
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("** Queue Sender **");
            Console.WriteLine("Type 'exit' to end the program\n");
            string commandMessage = string.Empty;
            do
            {
                Console.Write("> ");
                commandMessage = Console.ReadLine().Trim();
                if(!commandMessage.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                    SendMessage(commandMessage);

            } while (!commandMessage.Equals("exit", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
