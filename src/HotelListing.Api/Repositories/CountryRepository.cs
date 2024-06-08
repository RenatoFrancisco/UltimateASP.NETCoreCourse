using HotelListing.Api.Contracts;
using HotelListing.Api.Data;

namespace HotelListing.Api.Repositories;

public class CountryRepository(HotelListingDbContext context) : 
                                    GenericRepository<Country>(context), 
                                    ICounttryRepository 
{
    
}
