using AutoMapper;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.Exceptions;
using HotelListing.Api.Models;
using HotelListing.Api.Models.Hotel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace HotelListing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController(IHotelRepository hotelRepository, IMapper mapper) : ControllerBase
    {

        [HttpGet("GetAll")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels() =>
            mapper.Map<List<HotelDto>>(await hotelRepository.GetAllAsync());

        [HttpGet]
        public async Task<ActionResult<PagedResult<HotelDto>>> GetHotels([FromQuery] QueryParameters query) =>
            await hotelRepository.GetAllAsync<HotelDto>(query);


        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            var hotel = await hotelRepository.GetAsync(id) 
                ?? throw new NotFoundException(nameof(GetHotel), id);

            return mapper.Map<HotelDto>(hotel);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutHotel(int id, HotelDto hotelDto)
        {
            if (id != hotelDto.Id)
                return BadRequest("Invalid Record Id");

            var hotel = await hotelRepository.GetAsync(id)
                ?? throw new NotFoundException(nameof(PutHotel), id);

            mapper.Map(hotelDto, hotel);

            await hotelRepository.UpdateAsync(hotel);

            return NoContent();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDto createHotelDto)
        {
            var hotel = mapper.Map<Hotel>(createHotelDto);
            await hotelRepository.Addsync(hotel);

            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            if (!await hotelRepository.Exists(id))
                throw new NotFoundException(nameof(DeleteHotel), id);

            await hotelRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}
