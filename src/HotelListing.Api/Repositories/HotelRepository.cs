using HotelListing.Api.Contracts;
using HotelListing.Api.Data;

namespace HotelListing.Api.Repositories;

public class HotelRepository(HotelListingDbContext context) : 
                                GenericRepository<Hotel>(context), 
                                IHotelRepository
{
}
