using Asp.Versioning;
using AutoMapper;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.Exceptions;
using HotelListing.Api.Models;
using HotelListing.Api.Models.Country;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace HotelListing.Api.Controllers;

[Route("api/v{version:apiVersion}/countries")]
[ApiController]
[ApiVersion("1.0", Deprecated = true)]
public class CountriesController(ICountryRepository countryRepository, IMapper mapper) : ControllerBase
{
    [HttpGet("GetAll")]
    [EnableQuery]
    public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries() =>
        mapper.Map<List<GetCountryDto>>(await countryRepository.GetAllAsync());

    [HttpGet]
    public async Task<ActionResult<PagedResult<GetCountryDto>>> GetCountries([FromQuery] QueryParameters query) =>
        await countryRepository.GetAllAsync<GetCountryDto>(query);

    [HttpGet("{id}")]
    public async Task<ActionResult<CountryDto>> GetCountry(int id)
    {
        var country = await countryRepository.GetDetailsAsync(id) 
            ?? throw new NotFoundException(nameof(GetCountry), id);

        return mapper.Map<CountryDto>(country);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
    {
        if (id != updateCountryDto.Id)
            return BadRequest("Invalid Record Id");

        var country = await countryRepository.GetAsync(id) 
            ?? throw new NotFoundException(nameof(PutCountry), id);

        mapper.Map(updateCountryDto, country);

        await countryRepository.UpdateAsync(country);

        return NoContent();
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountry)
    {
        var country = mapper.Map<Country>(createCountry);

        await countryRepository.Addsync(country);

        return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        if (!await countryRepository.Exists(id))
            throw new NotFoundException(nameof(DeleteCountry), id);

        await countryRepository.DeleteAsync(id);

        return NoContent();
    }
}
