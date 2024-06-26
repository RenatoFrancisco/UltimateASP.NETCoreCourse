using AutoMapper;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;

namespace HotelListing.Api.Repositories;

public class HotelRepository(HotelListingDbContext context, IMapper mapper) : 
                                GenericRepository<Hotel>(context, mapper), 
                                IHotelRepository
{
}
