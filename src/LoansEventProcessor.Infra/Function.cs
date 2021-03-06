using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS;
using Amazon.SQS.Model;
using LoansEventProcessor.Core;
using LoansEventProcessor.Core.Application;
using LoansEventProcessor.Core.Application.Ports.Input;
using LoansEventProcessor.Infra.Adapters;
using Microsoft.Extensions.Logging;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LoansEventProcessor.Infra
{
    public class Function
    {
        
        /// <summary>
        /// Processes the events that defines loans updates.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent input, ILambdaContext context)
        {
            ILogger logger = new LambdaLoggerWrapper();
            var processor = new SnapshotGenerationUseCase(logger, new DynamoDbLoanRepository(logger));
            var processedMessages = new List<DeleteMessageBatchRequestEntry>();
            var exceptions = new List<Exception>();
            foreach (var record in input.Records)
            {
                try
                {
                    string eventType = record.Attributes.SingleOrDefault(s => s.Key == "EventType").Value;
                    var @event = new Event(record.Attributes.SingleOrDefault(s => s.Key == "LoanId").Value,
                        record.MessageId,
                        eventType,
                        long.Parse(record.Attributes.SingleOrDefault(s => s.Key == "SentTimestamp").Value),
                        record.Body);

                    await processor.Process(@event);

                    var processedMessageToBeDeleted = new DeleteMessageBatchRequestEntry
                    {
                        Id = record.MessageId,
                        ReceiptHandle = record.ReceiptHandle
                    };
                    processedMessages.Add(processedMessageToBeDeleted);
                }
                catch(Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            await RemoveProcessedMessagesFromQueue(processedMessages);
            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        private async Task RemoveProcessedMessagesFromQueue(List<DeleteMessageBatchRequestEntry> processedMessages)
        {
            if (processedMessages.Count > 0)
            {
                var client = new AmazonSQSClient();
                var delRequest = new DeleteMessageBatchRequest
                {
                    Entries = processedMessages,
                    QueueUrl = Environment.GetEnvironmentVariable("LoansEventsQueue")
                };

                await client.DeleteMessageBatchAsync(delRequest);
            }
        }
    }
}
