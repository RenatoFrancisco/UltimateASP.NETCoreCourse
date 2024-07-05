using AutoMapper;
using HotelListing.Api.Core.Contracts;
using HotelListing.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Core.Repositories;

public class CountryRepository(HotelListingDbContext context, IMapper mapper) :
                                    GenericRepository<Country>(context, mapper),
                                    ICountryRepository
{
    public async Task<Country> GetDetailsAsync(int id) => 
        await DbSet.Include(c => c.Hotels).SingleOrDefaultAsync(c => c.Id == id);
}
