using Asp.Versioning;
using AutoMapper;
using HotelListing.Api.Core.Contracts;
using HotelListing.Api.Core.Exceptions;
using HotelListing.Api.Core.Models;
using HotelListing.Api.Core.Models.Country;
using HotelListing.Api.Data;
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
        (await countryRepository.GetAllAsync<GetCountryDto>()).ToList();

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

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountry)
    {
        var country = await countryRepository.Addsync<CreateCountryDto, Country>(createCountry);
        return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
    {
        if (id != updateCountryDto.Id)
            return BadRequest("Invalid Record Id");

        await countryRepository.UpdateAsync(id, updateCountryDto);

        return NoContent();
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
