using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Api.Data;
using HotelListing.Api.Contracts;

namespace HotelListing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController(IHotelRepository hotelRepository) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetHotels() => 
            Ok(await hotelRepository.GetAllAsync());
         

        [HttpGet("{id}")]
        public async Task<ActionResult<Hotel>> GetHotel(int id)
        {
            var hotel = await hotelRepository.GetAsync(id);

            if (hotel is null)
                return NotFound();

            return hotel;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, Hotel hotel)
        {
            if (id != hotel.Id)
                return BadRequest("Invalid Record Id");

            if (hotel is null)
                NotFound();

            try
            {
                await hotelRepository.UpdateAsync(hotel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!!await hotelRepository.Exists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel(Hotel hotel)
        {
            await hotelRepository.Addsync(hotel);

            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            if (!await hotelRepository.Exists(id))
                return NotFound();

            await hotelRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}
