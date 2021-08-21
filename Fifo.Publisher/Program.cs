using System;
using Amazon.SQS.Model;
using Amazon.SQS;
using Amazon;

namespace Fifo.Publisher
{
    class Program
    {
        private static IAmazonSQS _sqsClient;
        private static readonly string _sqsUrl = "https://sqs.ap-southeast-1.amazonaws.com/073943586533/CoreQueue.fifo";

        static Program()
        {
            _sqsClient = new AmazonSQSClient(RegionEndpoint.APSoutheast1);
        }
        static void SendMessage(string groupdId, string deduplicateId, string message)
        {
            try
            {
                var request = new SendMessageRequest
                {
                    QueueUrl = _sqsUrl,
                    MessageGroupId = groupdId,
                    MessageDeduplicationId = deduplicateId,
                    MessageBody = message
                };
                request.MessageAttributes = new System.Collections.Generic.Dictionary<string, MessageAttributeValue>();
                request.MessageAttributes.Add("GroupId", new MessageAttributeValue() { StringValue = groupdId, DataType = "String" });
                request.MessageAttributes.Add("DeduplicateId", new MessageAttributeValue() { StringValue = deduplicateId, DataType = "String" });

                var response = _sqsClient.SendMessageAsync(request).Result;
                Console.WriteLine($"StatusCode: {response.HttpStatusCode}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Error]: {ex.Message}");
            }
        }

        static void Main(string[] args)
        {

            Console.WriteLine("** SQS Fifo Publisher **");
            Console.WriteLine("Command: <group_id>;<deduplicate_id>;<message_body>");
            Console.WriteLine("To exit type 'exit'\n");

            string payload = string.Empty;

            do
            {
                Console.Write("> ");
                payload = Console.ReadLine().Trim();
                if(!payload.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    string groupdId = payload.Split(";")[0];
                    string deduplicateId = payload.Split(";")[1];
                    string message = payload.Split(";")[2];
                    SendMessage(groupdId, deduplicateId, message);
                }

            } while (!payload.Equals("exit", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
