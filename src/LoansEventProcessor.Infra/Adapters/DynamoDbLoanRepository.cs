using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using LoansEventProcessor.Core.Application.Ports.Input;
using LoansEventProcessor.Core.Application.Ports.Output;
using LoansEventProcessor.Core.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoansEventProcessor.Infra.Adapters
{
    public class DynamoDbLoanRepository : ILoanRepository
    {
        private static readonly AmazonDynamoDBClient _client = new AmazonDynamoDBClient();
        private readonly ILogger _logger;

        public DynamoDbLoanRepository(ILogger logger)
        {
            _logger = logger;
        }
        public async Task<Snapshot<Loan>> GetLoan(string loanId)
        {
            Table table = Table.LoadTable(_client, "LoanSnapshot");
            Document document = await table.GetItemAsync(loanId);

            if (document != null)
            {
                var loan = JsonSerializer.Deserialize<Loan>(document["Content"].AsString());
                return new Snapshot<Loan>(loan, document["Id"].AsString(), document["Version"].AsInt());
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> SaveLoan(Snapshot<Loan> loan)
        {
            var response = await _client.TransactWriteItemsAsync(new TransactWriteItemsRequest
            {
                TransactItems = new List<TransactWriteItem>
                {
                    new TransactWriteItem() { Put = CreateLoanSaveRequest(loan) },
                    new TransactWriteItem() { Put = CreateLoanEventSaveRequest(loan.Event) }
                },
                ClientRequestToken = loan.Event.Id
            });
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        private Put CreateLoanSaveRequest(Snapshot<Loan> loan)
        {
            var put = new Put()
            {
                TableName = "LoanSnapshot"
            };

            put.ExpressionAttributeValues[":version"] = new AttributeValue { N = loan.LastEventVersion.ToString() };
            put.ConditionExpression = "Version = :version OR attribute_not_exists(Id)";
            put.Item = new Dictionary<string, AttributeValue>()
            {
                ["Id"] = new AttributeValue { S = loan.Item.Id },
                ["Version"] = new AttributeValue { N = loan.NewEventVersion.ToString() },
                ["UpdatedAt"] = new AttributeValue { N = loan.UpdatedAt.ToUnixTimeMilliseconds().ToString() },
                ["Content"] = new AttributeValue { S = JsonSerializer.Serialize(loan.Item) }
            };

            return put;
        }

        private Put CreateLoanEventSaveRequest(Event @event)
        {
            var put = new Put()
            {
                TableName = "Events"
            };

            put.ConditionExpression = "attribute_not_exists(Id)";
            put.Item = new Dictionary<string, AttributeValue>()
            {
                ["EntityId"] = new AttributeValue { S = @event.EntityId },
                ["Id"] = new AttributeValue { S = @event.Id },
                ["EventType"] = new AttributeValue { S = @event.EventType },
                ["EventTime"] = new AttributeValue { N = @event.EventTime.ToUnixTimeMilliseconds().ToString() },
                ["ProcessesAt"] = new AttributeValue { N = @event.ProcessesAt.ToUnixTimeMilliseconds().ToString() },
                ["Content"] = new AttributeValue { S = @event.Content }
            };

            return put;
        }
    }
}
