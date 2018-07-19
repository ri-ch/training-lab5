using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace StateMachines
{
    public class SqsConsumer
    {
        public async Task Start(AWSOptions awsOptions, string queueName, CancellationToken cancel)
        {
            using (var client = awsOptions.CreateServiceClient<IAmazonSQS>())
            {
                var getQueueResult = await client.GetQueueUrlAsync(queueName, cancel);

                while (cancel.IsCancellationRequested == false)
                {
                    Console.WriteLine("Waiting for messages....");

                    ReceiveMessageRequest rxMessage = new ReceiveMessageRequest();
                    rxMessage.QueueUrl = getQueueResult.QueueUrl;
                    rxMessage.WaitTimeSeconds = 20;
                    rxMessage.MaxNumberOfMessages = 10;

                    ReceiveMessageResponse rxMessageResponse = await client.ReceiveMessageAsync(rxMessage);

                    if (rxMessageResponse.Messages.Any() == false)
                        return;

                    Console.WriteLine($"Got {rxMessageResponse.Messages.Count} messages");

                    var deletions = rxMessageResponse.Messages.Select(m => new DeleteMessageBatchRequestEntry(m.MessageId, m.ReceiptHandle));

                    Console.WriteLine("Deleting messages....");

                    await client.DeleteMessageBatchAsync(getQueueResult.QueueUrl, deletions.ToList());
                }
            }
        }
    }
}