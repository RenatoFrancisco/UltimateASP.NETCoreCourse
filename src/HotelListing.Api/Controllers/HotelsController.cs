using AutoMapper;
using HotelListing.Api.Core.Contracts;
using HotelListing.Api.Core.Exceptions;
using HotelListing.Api.Core.Models;
using HotelListing.Api.Core.Models.Hotel;
using HotelListing.Api.Data;
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
            (await hotelRepository.GetAllAsync<HotelDto>()).ToList();

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

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDto createHotelDto)
        {
            var hotel = await hotelRepository.Addsync<CreateHotelDto, Hotel>(createHotelDto);
            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutHotel(int id, HotelDto hotelDto)
        {
            if (id != hotelDto.Id)
                return BadRequest("Invalid Record Id");

            await hotelRepository.UpdateAsync(id, hotelDto);

            return NoContent();
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
