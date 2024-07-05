using HotelListing.Api.Data;

namespace HotelListing.Api.Core.Contracts;

public interface ICountryRepository : IGenericRepository<Country>
{
    Task<Country> GetDetailsAsync(int id);
}