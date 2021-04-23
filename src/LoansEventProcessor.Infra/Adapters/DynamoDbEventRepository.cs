using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using LoansEventProcessor.Core.Application.Ports.Input;
using LoansEventProcessor.Core.Application.Ports.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoansEventProcessor.Infra.Adapters
{
    public class DynamoDbEventRepository : IEventRepository
    {
        private static readonly AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        private static readonly string tableName = "Events";
        public async Task<Event> GetEvent(string eventId)
        {
            Table table = Table.LoadTable(client, tableName);
            GetItemOperationConfig config = new GetItemOperationConfig
            {
                AttributesToGet = new List<string> { "EntityId", "Id", "EventType", "EventTime", "Version", "Content" }
            };
            Document document = await table.GetItemAsync(eventId, config);
            return FromDocument(document);
        }

        public async Task<List<Event>> GetEventsSinceVersion(string entityId, int version)
        {
            Table table = Table.LoadTable(client, tableName);

            QueryFilter filter = new QueryFilter("EntityId", QueryOperator.Equal, entityId);
            filter.AddCondition("Version", QueryOperator.GreaterThan, version);
            var config = new QueryOperationConfig()
            {
                AttributesToGet = new List<string> { "EntityId", "Id", "EventType", "EventTime", "Version", "Content" },
                Filter = filter
            };

            Search search = table.Query(config);

            var events = new List<Event>();
            do
            {
                foreach (var document in await search.GetNextSetAsync())
                {
                    var @event = FromDocument(document);
                    events.Add(@event);
                }
            } while (!search.IsDone);
            return events;
        }

        public async Task SaveEvent(Event @event)
        {
            Table table = Table.LoadTable(client, tableName);

            var document = new Document();
            document["EntityId"] = @event.EntityId;
            document["Id"] = @event.Id;
            document["EventType"] = @event.EventType;
            document["EventTime"] = @event.EventTime.ToUnixTimeSeconds();
            document["Version"] = @event.Version;
            document["Content"] = @event.Content;

            var expr = new Expression();
            expr.ExpressionStatement = "attribute_not_exists(Id)";

            PutItemOperationConfig config = new PutItemOperationConfig()
            {
                ConditionalExpression = expr,
            };
            await table.PutItemAsync(document, config);
        }

        private Event FromDocument(Document document)
        {
            return new Event(document["EntityId"].AsString(),
                        document["Id"].AsString(),
                        document["EventType"].AsString(),
                        document["EventTime"].AsLong(),
                        document["Content"].AsString());
        }
    }
}
