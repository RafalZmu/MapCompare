
using MapCompereAPI.Models;

public interface IMapService
{
    public Task<Map> GetBaseMap();
    public Task<Map> GetMap(string mapName);
    public Task<int> PostMap(Map map);
    public Task<int> DeleteMap(string mapName);
}