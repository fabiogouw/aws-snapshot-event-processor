using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using LoansEventProcessor.Infra;
using Amazon.Lambda.SQSEvents;

namespace LoansEventProcessor.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task TestToUpperFunction()
        {

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var msg = new SQSEvent()
            {
                Records = new List<SQSEvent.SQSMessage>() 
                {
                    new SQSEvent.SQSMessage()
                    {
                        MessageId = "19dd0b57-b21e-4ac1-bd88-01bbb068cb73",
                        Body = "dummy",
                        Attributes = new Dictionary<string, string>()
                        {
                            ["LoanId"] = "2",
                            ["EventType"] = "dummy",
                            ["SentTimestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
                        }
                    }
                }
            };
            await function.FunctionHandler(msg, context);
        }
    }
}
