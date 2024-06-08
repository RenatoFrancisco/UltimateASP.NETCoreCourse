using AutoMapper;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.Models.Country;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController(ICountryRepository countryRepository, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries() => 
        Ok(mapper.Map<List<GetCountryDto>>(await countryRepository.GetAllAsync()));

    [HttpGet("{id}")]
    public async Task<ActionResult<CountryDto>> GetCountry(int id)
    {
        var country = await countryRepository.GetDetailsAsync(id);

        if (country is null) 
            return NotFound();

        return mapper.Map<CountryDto>(country);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
    {
        if (id != updateCountryDto.Id)
            return BadRequest("Invalid Record Id");

        var country = await countryRepository.GetAsync(id);
        if (country is null)
            return NotFound();

        mapper.Map(updateCountryDto, country);

        try
        {
            await countryRepository.UpdateAsync(country);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await countryRepository.Exists(id)) 
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

        await countryRepository.Addsync(country);

        return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        if (!await countryRepository.Exists(id))
            return NotFound();

        await countryRepository.DeleteAsync(id);

        return NoContent();
    }
}
