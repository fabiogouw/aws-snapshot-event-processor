using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using LoansEventProcessor.Core.Application.Ports.Output;
using LoansEventProcessor.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoansEventProcessor.Infra.Adapters
{
    public class DynamoDbLoanRepository : ILoanRepository
    {
        private static readonly AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        private static readonly string tableName = "LoanSnapshot";

        public async Task<Snapshot<Loan>> GetLoan(string loanId)
        {
            Table table = Table.LoadTable(client, tableName);

            GetItemOperationConfig config = new GetItemOperationConfig
            {
                AttributesToGet = new List<string> { "Id", "Version", "Content" }
            };
            Document document = await table.GetItemAsync(loanId, config);

            var loan = JsonSerializer.Deserialize<Loan>(document["Content"].AsString());
            return new Snapshot<Loan>(loan, document["Version"].AsInt());
        }

        public async Task SaveLoan(Snapshot<Loan> loan)
        {
            Table table = Table.LoadTable(client, tableName);

            var document = new Document();
            document["Id"] = loan.Item.Id;
            document["Version"] = loan.NewEventVersion;
            document["Content"] = Document.FromJson(JsonSerializer.Serialize(loan.Item));

            var expr = new Expression();
            expr.ExpressionStatement = "Version = :version";
            expr.ExpressionAttributeValues[":version"] = loan.LastEventVersion;

            PutItemOperationConfig config = new PutItemOperationConfig()
            {
                ConditionalExpression = expr
            };

            await table.PutItemAsync(document, config);
        }
    }
}
