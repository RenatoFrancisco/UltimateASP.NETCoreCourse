using HotelListing.Api.Data;

namespace HotelListing.Api.Contracts;

public interface ICountryRepository : IGenericRepository<Country>
{
    Task<Country> GetDetailsAsync(int id);
}