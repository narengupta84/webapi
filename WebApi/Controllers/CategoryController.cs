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
    public class CategoryController : ControllerBaseClass
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Category>))]
        public async Task<IActionResult> GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            return await Task.FromResult<IActionResult>(Ok(categories));
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return await Task.FromResult<IActionResult>(NotFound());

            var categories = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId));

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            return await Task.FromResult<IActionResult>(Ok(categories));
        }

        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPokemonByCategoryId(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return await Task.FromResult<IActionResult>(NotFound());

            var pokemons = _mapper.Map<PokemonDto>(_categoryRepository.GetPokemonByCategory(categoryId));

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            return await Task.FromResult<IActionResult>(Ok(pokemons));
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] CategoryDto updateCategory)
        {
            if (updateCategory == null)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            if (categoryId != updateCategory.Id)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            if (!_categoryRepository.CategoryExists(categoryId))
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            var categoryMap = _mapper.Map<Category>(updateCategory);

            if (!_categoryRepository.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return await Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, ModelState));
            }

            return await Task.FromResult<IActionResult>(NoContent());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto createCategory)
        {
            if (createCategory == null)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            var category = _categoryRepository.GetCategories()
                .Where(c => c.Name.Trim().ToUpper() == createCategory.Name.Trim().ToUpper())
                .FirstOrDefault();

            if (category != null)
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            var categoryMap = _mapper.Map<Category>(createCategory);

            if (!_categoryRepository.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return await Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, ModelState));
            }

            return await Task.FromResult<IActionResult>(Ok("Successfully Created"));
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return await Task.FromResult<IActionResult>(NotFound());
            }

            var categoryToDelete = _categoryRepository.GetCategory(categoryId);

            if (!ModelState.IsValid)
                return await Task.FromResult<IActionResult>(BadRequest(ModelState));

            if (!_categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category");
                return await Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, ModelState));
            }

            return await Task.FromResult<IActionResult>(NoContent());
        }
    }
}
