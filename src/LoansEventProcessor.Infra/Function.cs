using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using LoansEventProcessor.Core;
using LoansEventProcessor.Core.Application;
using LoansEventProcessor.Core.Application.Ports.In;

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
        public void FunctionHandler(SQSEvent input, ILambdaContext context)
        {
            LambdaLogger.Log($"Calling function name: {context.FunctionName}\n");
            var processor = new SnapshotGenerationUseCase(null, null);
            foreach (var record in input.Records)
            {
                var eventTime = DateTimeOffset.FromUnixTimeSeconds(
                    long.Parse(record.Attributes.SingleOrDefault(s => s.Key == "SentTimestamp").Value)
                    );
                var @event = new Event(record.MessageId,
                    record.Attributes.SingleOrDefault(s => s.Key == "EntityId").Value,
                    record.Attributes.SingleOrDefault(s => s.Key == "EventType").Value,
                    eventTime, 
                    record.Body);
                string eventType = record.Attributes.SingleOrDefault(s => s.Key == "EventType").Value;
                if (!string.IsNullOrEmpty(eventType))
                {
                    processor.Process(@event);
                }
            }
        }
    }
}
