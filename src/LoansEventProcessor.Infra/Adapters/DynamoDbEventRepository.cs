using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using LoansEventProcessor.Core.Application.Ports.Input;
using LoansEventProcessor.Core.Application.Ports.Output;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoansEventProcessor.Infra.Adapters
{
    public class DynamoDbEventRepository : IEventRepository
    {
        private static readonly AmazonDynamoDBClient _client = new AmazonDynamoDBClient();
        private static readonly string _tableName = "Events";
        private readonly ILogger _logger;

        public DynamoDbEventRepository(ILogger logger)
        {
            _logger = logger;
        }
        public async Task<Event> GetEvent(string eventId)
        {
            Table table = Table.LoadTable(_client, _tableName);
            Document document = await table.GetItemAsync(eventId);
            
            return FromDocument(document);
        }

        public async Task<List<Event>> GetEventsSinceVersion(string entityId, int version)
        {
            Table table = Table.LoadTable(_client, _tableName);

            QueryFilter filter = new QueryFilter("EntityId", QueryOperator.Equal, entityId);
            filter.AddCondition("Version", QueryOperator.GreaterThan, version);
            var config = new QueryOperationConfig()
            {
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

        public async Task<bool> SaveEvent(Event @event)
        {
            Table table = Table.LoadTable(_client, _tableName);

            var document = new Document();
            document["EntityId"] = @event.EntityId;
            document["Id"] = @event.Id;
            document["EventType"] = @event.EventType;
            document["EventTime"] = @event.EventTime.ToUnixTimeMilliseconds();
            document["Version"] = @event.Version;
            document["Content"] = @event.Content;

            var expr = new Expression();
            expr.ExpressionStatement = "attribute_not_exists(Id)";

            PutItemOperationConfig config = new PutItemOperationConfig()
            {
                ConditionalExpression = expr,
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
