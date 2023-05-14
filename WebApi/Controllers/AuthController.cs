using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Interfaces;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly JwtAuthenticationManager _jwtAuthenticationManager;

        public AuthController(IPokemonRepository pokemonRepository, JwtAuthenticationManager jwtAuthenticationManager)
        {
            _pokemonRepository = pokemonRepository;
            _jwtAuthenticationManager = jwtAuthenticationManager;
        }
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult AuthPokemon([FromBody] PokemonDto authPokemon)
        {
            if (authPokemon == null)
                return BadRequest(ModelState);

            var pokemon = _pokemonRepository.GetPokemon(authPokemon.Id);

            if (pokemon == null)
            {
                ModelState.AddModelError("", "Pokemon not found");
                return StatusCode(400, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = _jwtAuthenticationManager.Authenticate(pokemon.Name);
            if (token == null)
            {
                return Unauthorized();
            }
            return Ok(token);
        }
    }
}
