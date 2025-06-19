using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace Transport.Infrastructure.MongoDB
{
    public class MongoDBHelper
    {
        private readonly IMongoDatabase _database;

        public MongoDBHelper(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDB:ConnectionString"];
            var databaseName = configuration["MongoDB:DatabaseName"];

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoDatabase GetDatabase() => _database;
    }
}
