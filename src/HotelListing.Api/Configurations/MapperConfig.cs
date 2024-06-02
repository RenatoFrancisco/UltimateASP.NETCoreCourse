using AutoMapper;
using HotelListing.Api.Data;
using HotelListing.Api.Models.Country;

namespace HotelListing.Api.Configurations;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
       CreateMap<Country, CreateCountryDto>().ReverseMap();
    }
}
