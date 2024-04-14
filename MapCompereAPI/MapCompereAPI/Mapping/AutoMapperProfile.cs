using AutoMapper;
using MapCompereAPI.Models;
using MongoDB.Bson;

namespace MapCompereAPI.Mapping
{
    public class AutoMapperProfile :Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BsonDocument, Map>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Contains("Name") ? src["Name"].AsString : null))
                .ForMember(dest => dest.SVGImage, opt => opt.MapFrom(src => src.Contains("SVGImage") ? src["SVGImage"].AsString : null))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Contains("Creator") ? src["Creator"].AsString : null))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Contains("Description") ? src["Description"].AsString : null))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.Contains("CreationDate") ? src["CreationDate"].ToUniversalTime() : DateTime.MinValue))
                .ForMember(dest => dest.Countries, opt => opt.Ignore());


        }
    }
}
