using MongoDB.Driver;
using MongoDB.Bson;

namespace MapCompereAPI.Repositories
{
	public class DataBaseMongo
	{
		private static string _mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");

		static DataBaseMongo()
		{
			if (_mongoConnectionString == null)
			{
				Console.WriteLine("No connection string found in enviroment variables");
			}

			var setting = MongoClientSettings.FromConnectionString(_mongoConnectionString);

			setting.ServerApi = new ServerApi(ServerApiVersion.V1);
			var client = new MongoClient(setting);

			try
			{
				var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}