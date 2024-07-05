using AutoMapper;
using HotelListing.Api.Core.Contracts;
using HotelListing.Api.Data;

namespace HotelListing.Api.Core.Repositories;

public class HotelRepository(HotelListingDbContext context, IMapper mapper) : 
                                GenericRepository<Hotel>(context, mapper), 
                                IHotelRepository
{
}
