using AutoMapper;
using HotelListing.Api.Data;
using HotelListing.Api.Core.Models.Country;
using HotelListing.Api.Core.Models.Hotel;
using HotelListing.Api.Core.Models.Users;

namespace HotelListing.Api.Core.Configurations;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<Country, CreateCountryDto>().ReverseMap();
        CreateMap<Country, GetCountryDto>().ReverseMap();
        CreateMap<Country, CountryDto>().ReverseMap();
        CreateMap<Country, UpdateCountryDto>().ReverseMap();

        CreateMap<Hotel, HotelDto>().ReverseMap();
        CreateMap<Hotel, CreateHotelDto>().ReverseMap();

        CreateMap<ApiUserDto, ApiUser>().ReverseMap();
    }
}
