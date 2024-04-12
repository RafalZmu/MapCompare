using MongoDB.Driver;
using MongoDB.Bson;
using System.ComponentModel;

namespace MapCompereAPI.Repositories
{
	public class DataBaseMongo: IDocumentDatabase
	{
		private static string _mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION");
		private static MongoClient? _client;

		public DataBaseMongo()
		{
			__init__();
		}
        public void __init__()
        {
			if (_mongoConnectionString == null)
			{
				Console.WriteLine("No connection string found in enviroment variables");
			}

			var setting = MongoClientSettings.FromConnectionString(_mongoConnectionString);

			setting.ServerApi = new ServerApi(ServerApiVersion.V1);
			_client = new MongoClient(setting);

			try
			{
				var result = _client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
				Console.WriteLine(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error connecting to database");
				Console.WriteLine(ex);
			}
        }

        public IMongoCollection<BsonDocument> GetCollection(string collectionName)
        {
			if(_client == null)
			{
				return null;
			}
			return _client.GetDatabase("MapCompere").GetCollection<BsonDocument>(collectionName);
        }

    }
}