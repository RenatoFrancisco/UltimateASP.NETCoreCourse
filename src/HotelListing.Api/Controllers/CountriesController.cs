using AutoMapper;
using HotelListing.Api.Data;
using HotelListing.Api.Models.Country;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController(HotelListingDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries() => 
        mapper.Map<List<GetCountryDto>>(await context.Countries.ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<CountryDto>> GetCountry(int id)
    {
        var country = await context
            .Countries
            .Include(c => c.Hotels)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (country is null) 
            return NotFound();

        return mapper.Map<CountryDto>(country);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
    {
        if (id != updateCountryDto.Id)
            return BadRequest("Invalid Record Id");

        var country = await GetById(id);
        if (country is null)
            return NotFound();

        mapper.Map(updateCountryDto, country);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CountryExists(id)) 
                return NotFound();
            else 
                throw;
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountry)
    {
        var country = mapper.Map<Country>(createCountry);

        context.Countries.Add(country);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var country = await GetById(id);
        if (country is null)
            return NotFound();

        context.Countries.Remove(country);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool CountryExists(int id) => context.Countries.Any(e => e.Id == id);

    private async Task<Country> GetById(int id) => await context.Countries.FindAsync(id);
}
