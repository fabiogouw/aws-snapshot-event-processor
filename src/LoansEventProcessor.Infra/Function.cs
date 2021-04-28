using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
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
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent input, ILambdaContext context)
        {
            ILogger logger = new LambdaLoggerWrapper();
            var processor = new SnapshotGenerationUseCase(logger, new DynamoDbLoanRepository(logger));
            foreach (var record in input.Records)
            {
                string eventType = record.Attributes.SingleOrDefault(s => s.Key == "EventType").Value;
                var @event = new Event(record.Attributes.SingleOrDefault(s => s.Key == "LoanId").Value,
                    record.MessageId,
                    eventType,
                    long.Parse(record.Attributes.SingleOrDefault(s => s.Key == "SentTimestamp").Value), 
                    record.Body);
                
                await processor.Process(@event);
            }
        }
    }
}
