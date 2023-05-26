using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Dto;
using WebApi.Generic;
using WebApi.Interfaces;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Controllers
{
    public class CountryController : ControllerBaseClass
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CountryController> _logger;
        public CountryController(ICountryRepository countryRepository, IMapper mapper, ILogger<CountryController> logger)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Country>))]
        public async Task<IActionResult> GetCountries()
        {
            var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            return await Task.FromResult<IActionResult>(Ok(countries));
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Country))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return await Task.FromResult<IActionResult>(NotFound());

            var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            return await Task.FromResult<IActionResult>(Ok(country));
        }

        [HttpGet("owners/{ownerId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Country))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCountryOfAnOwner(int ownerId)
        {
            var country = _mapper.Map<CountryDto>(_countryRepository.GetCountryByOwner(ownerId));

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            return await Task.FromResult<IActionResult>(Ok(country));
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCountry(int countryId, [FromBody] CountryDto updateCountry)
        {
            //_logger.LogInformation("UpdateCountry => ", UpdateCountry);

            if (updateCountry == null)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            if (countryId != updateCountry.Id)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            if (!_countryRepository.CountryExists(countryId))
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            var countryMap = _mapper.Map<Country>(updateCountry);

            if (!_countryRepository.UpdateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return await Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, ModelState));
            }

            return await Task.FromResult<IActionResult>(NoContent());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCountry([FromBody] CountryDto createCountry)
        {
            if (createCountry == null)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            var country = _countryRepository.GetCountries()
                .Where(c => c.Name.Trim().ToUpper() == createCountry.Name.Trim().ToUpper())
                .FirstOrDefault();

            if (country != null)
            {
                ModelState.AddModelError("", "Country already exists");
                return await Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState));
            }

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            var countryMap = _mapper.Map<Country>(createCountry);

            if (!_countryRepository.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return await Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, ModelState));
            }

            return await Task.FromResult<IActionResult>(Ok("Successfully Created"));
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
            {
                return await Task.FromResult<IActionResult>(NotFound());
            }

            var countryToDelete = _countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            if (!_countryRepository.DeleteCountry(countryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category");
                return await Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, ModelState));
            }

            return await Task.FromResult<IActionResult>(NoContent());
        }
    }
}
