using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using WebApi.Dto;
using WebApi.Generic;
using WebApi.Interfaces;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Controllers
{
    public class OwnerController : ControllerBaseClass
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Owner>))]
        public  async Task<IActionResult> GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            return await Task.FromResult<IActionResult>(Ok(owners));
        }


        [HttpGet("{ownerId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Owner))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public  async Task<IActionResult> GetOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
                return NotFound();

            var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            return await Task.FromResult<IActionResult>(Ok(owner));
        }

        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Owner))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public  async Task<IActionResult> GetPokemonByOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
                return NotFound();

            var pokemons = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonByOwner(ownerId));

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            return await Task.FromResult<IActionResult>(Ok(pokemons));
        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public  async Task<IActionResult> UpdateOwner(int ownerId, [FromBody] OwnerDto updateOwner)
        {
            if (updateOwner == null)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            if (ownerId != updateOwner.Id)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            if (!_ownerRepository.OwnerExists(ownerId))
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            var ownerMap = _mapper.Map<Owner>(updateOwner);

            if (!_ownerRepository.UpdateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return await Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, ModelState));
            }

            return await Task.FromResult<IActionResult>(NoContent());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public  async Task<IActionResult> CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto createOwner)
        {
            if (createOwner == null)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            var owner = _ownerRepository.GetOwners()
                .Where(c => c.FirstName.Trim().ToUpper() == createOwner.FirstName.Trim().ToUpper())
                .Where(c => c.LastName.Trim().ToUpper() == createOwner.LastName.Trim().ToUpper())
                .FirstOrDefault();

            if (owner != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            var ownerMap = _mapper.Map<Owner>(createOwner);
            ownerMap.Country = _countryRepository.GetCountry(countryId);

            if (!_ownerRepository.CreateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return await Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, ModelState));
            }

            return await Task.FromResult<IActionResult>(Ok("Successfully Created"));
        }

        [HttpDelete("{ownerId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public  async Task<IActionResult> DeleteOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var ownerToDelete = _ownerRepository.GetOwner(ownerId);

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            if (!_ownerRepository.DeleteOwner(ownerToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting owner");
                return await Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, ModelState));
            }

            return await Task.FromResult<IActionResult>(NoContent());
        }
    }
}
