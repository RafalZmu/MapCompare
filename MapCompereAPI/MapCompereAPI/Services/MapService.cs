
using Amazon.Runtime.Internal.Transform;
using AutoMapper;
using MapCompereAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver;

public class MapService : IMapService
{
    private readonly IDocumentDatabase _database;
    private readonly IMongoCollection<BsonDocument> _mapCollection;
    private readonly ILogger<MapService> _logger;
    private readonly IMapper _mapper;
    public MapService(IDocumentDatabase database, ILogger<MapService> logger, IMapper mapper)
    {
        _database = database;
        _mapCollection = _database.GetCollection("Maps");
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<Map> GetBaseMap()
    {
        if(_mapCollection == null)
        {
            _logger.LogError("No collection found");
            return null;
        }

        var filter = Builders<BsonDocument>.Filter.Eq("Name", "BaseMap");
        var mapBSON = _mapCollection.FindAsync(filter).Result.FirstOrDefaultAsync().Result;

        return _mapper.Map<Map>(mapBSON);

    }

    public async Task<Map> GetMap(string mapName)
    {
        if(_mapCollection == null)
        {
            _logger.LogError("No collection found");
            return null;
        }
        var filter = Builders<BsonDocument>.Filter.Eq("Name", mapName);
        var mapBSON = _mapCollection.FindAsync(filter).Result.FirstOrDefaultAsync().Result;
        Map map = _mapper.Map<Map>(mapBSON);


        return map;

    }

    public async Task<int> PostMap(Map map)
    {
        if(_mapCollection == null)
        {
            _logger.LogError("No collection found");
            return 0;
        }
        await _mapCollection.InsertOneAsync(map.ToBsonDocument());
        return 1;

    }
    public async Task<int> DeleteMap(string mapName)
    {
        if(mapName == null)
        {
            return 1;
        }
        try
        {
            var resoult = await _mapCollection.FindOneAndDeleteAsync(Builders<BsonDocument>.Filter.Eq("Name", mapName));
            if(resoult == null)
            {
                return 0;
            }
            return 1;
        }
        catch (Exception)
        {
            return 0;
        }
    }
}