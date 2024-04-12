
using MongoDB.Bson;
using MongoDB.Driver;

public class MapService : IMapService
{
    private readonly IDocumentDatabase _database;
    private readonly IMongoCollection<BsonDocument> _mapCollection;
    public MapService(IDocumentDatabase database)
    {
        _database = database;
        _mapCollection = _database.GetCollection("Maps");
    }
    public MapDTO GetMap()
    {
        if(_mapCollection == null)
        {
            throw new Exception("No connection to database");
        }
        var filter = Builders<BsonDocument>.Filter.Eq("MapName", "BaseMap");
        var map = _mapCollection.Find(filter).FirstOrDefault();
        MapDTO mapDTO = new MapDTO(name: map["MapName"].AsString, description: null, svgImage: map["MapSVG"].AsString, countries: null);
        return mapDTO;

    }

    public void PostMap(MapDTO map)
    {
        throw new NotImplementedException();
    }
}