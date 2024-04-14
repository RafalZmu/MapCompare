
using System.Reflection.Metadata;
using MongoDB.Bson;
using MongoDB.Driver;

public interface IDocumentDatabase
{
    public void __init__();
    
    //Get collection from database
    public IMongoCollection<BsonDocument> GetCollection(string collectionName);
}