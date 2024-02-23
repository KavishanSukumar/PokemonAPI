using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoryController :Controller
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly IMapper _mapper;

		public CategoryController(ICategoryRepository categoryRepository,IMapper mapper)
        {
			this._categoryRepository = categoryRepository;
			this._mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
		public IActionResult GetCategories()
		{
			var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(categories);
		}

		[HttpGet("{catgoryId}")]
		[ProducesResponseType(200, Type = typeof(Pokemon))]
		[ProducesResponseType(400)]
		public IActionResult GetPokemons(int catgoryId)
		{
			if (!_categoryRepository.CategoryExists(catgoryId))
			{
				return NotFound();
			}
			var category = _mapper.Map<PokemonDto>(_categoryRepository.GetCategory(catgoryId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			return Ok(category);

		}


	}
}
