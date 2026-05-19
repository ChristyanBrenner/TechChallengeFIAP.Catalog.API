using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class DynamoEventLogService : IEventLogService
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly string _tableName;

        public DynamoEventLogService(IAmazonDynamoDB dynamoDb, IConfiguration configuration)
        {
            _dynamoDb = dynamoDb;
            _tableName = configuration["DynamoDB:TableName"] ?? "CloudGamesEventLogs";
        }

        public async Task RegistrarAsync(EventLog eventLog)
        {
            if (eventLog.EventId == Guid.Empty)
                eventLog.EventId = Guid.NewGuid();

            if (eventLog.CreatedAt == default)
                eventLog.CreatedAt = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(eventLog.Source))
                eventLog.Source = "CatalogAPI";

            var table = Table.LoadTable(_dynamoDb, _tableName);

            var document = new Document
            {
                ["EventId"] = eventLog.EventId.ToString(),
                ["EventType"] = eventLog.EventType ?? "",
                ["Source"] = eventLog.Source ?? "",
                ["Payload"] = eventLog.Payload ?? "",
                ["CreatedAt"] = eventLog.CreatedAt.ToString("O")
            };

            await table.PutItemAsync(document);
        }
    }
}