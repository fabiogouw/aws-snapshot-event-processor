using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
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
        private static readonly string _tableName = "LoanSnapshot";
        private readonly ILogger _logger;

        public DynamoDbLoanRepository(ILogger logger)
        {
            _logger = logger;
        }
        public async Task<Snapshot<Loan>> GetLoan(string loanId)
        {
            Table table = Table.LoadTable(_client, _tableName);
            Document document = await table.GetItemAsync(loanId);

            if(document != null)
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
            Table table = Table.LoadTable(_client, _tableName);

            var document = new Document();
            document["Id"] = loan.Item.Id;
            document["Version"] = loan.NewEventVersion;
            document["Content"] = JsonSerializer.Serialize(loan.Item);

            var expr = new Expression();
            expr.ExpressionStatement = "Version = :version OR attribute_not_exists(Id)";
            expr.ExpressionAttributeValues[":version"] = loan.LastEventVersion;

            PutItemOperationConfig config = new PutItemOperationConfig()
            {
                ConditionalExpression = expr
            };

            try
            {
                await table.PutItemAsync(document, config);
                return true;
            }
            catch (ConditionalCheckFailedException ex)
            {
                _logger.LogError(ex, "Error while saving new event.");
                return false;
            }
        }
    }
}
