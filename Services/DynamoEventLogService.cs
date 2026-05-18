using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class DynamoEventLogService : IEventLogService
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private const string TableName = "CloudGamesEventLogs";

        public DynamoEventLogService(IAmazonDynamoDB dynamoDb) 
        {
            _dynamoDb = dynamoDb;
        }

        public async Task RegistrarAsync(EventLog eventLog)
        {
            var table = Table.LoadTable(_dynamoDb, TableName);

            var document = new Document
            {
                ["EventId"] = eventLog.EventId,
                ["EventType"] = eventLog.EventType,
                ["Source"] = eventLog.Source,
                ["Payload"] = eventLog.Payload,
                ["CreatedAt"] = eventLog.CreatedAt.ToString("O")
            };

            await table.PutItemAsync(document);
        }
    }
}
