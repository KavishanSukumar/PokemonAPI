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
	public class PokemonController :Controller
	{
		private readonly IPokemonRepository _pokemonRepository;
		private readonly IOwnerRepository _ownerRepository;
		private readonly ICategoryRepository _categoryRepository;
		private readonly IReviewRepository _reviewRepository;
		private readonly IMapper _mapper;

		public PokemonController(IPokemonRepository pokemonRepository, IMapper mapper, IOwnerRepository ownerRepository, ICategoryRepository categoryRepository, IReviewRepository reviewRepository)
		{
			this._pokemonRepository = pokemonRepository;
			this._mapper = mapper;
			_ownerRepository = ownerRepository;
			_categoryRepository = categoryRepository;
			_reviewRepository = reviewRepository;
		}

		[HttpGet]
		[ProducesResponseType(200,Type=typeof(IEnumerable<Pokemon>))]
		public IActionResult GetPokemons()
		{
			var pokemons=_mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

            if (!ModelState.IsValid)
            {
				return BadRequest(ModelState);
            }
			return Ok(pokemons);
        }

		[HttpGet("{pokeId}")]
		[ProducesResponseType(200, Type = typeof(Pokemon))]
		[ProducesResponseType(400)]
		public IActionResult GetPokemon(int pokeId) 
		{
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }
			var pokemon= _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			return Ok(pokemon);

        }

		[HttpGet("{pokeId}/rating")]
		[ProducesResponseType(200, Type = typeof(decimal))]
		[ProducesResponseType(400)]
		public IActionResult GetPokemonRating(int pokeId)
		{
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
				return NotFound();
            }
			var rating=_pokemonRepository.GetPokemonRating(pokeId);

            if (!ModelState.IsValid)
            {
				return BadRequest();
            }

			return Ok(rating);
        }

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int catId,[FromBody] PokemonDto pokemonCreate)
		{
			if (pokemonCreate == null)
				return BadRequest(ModelState);

			var pokemon = _pokemonRepository.GetPokemons()
				.Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
				.FirstOrDefault();

			if (pokemon != null)
			{
				ModelState.AddModelError("", "Country already exists");
				return StatusCode(422, ModelState);
			}
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);
		
			if (!_pokemonRepository.CreatePokemon(ownerId,catId,pokemonMap))
			{
				ModelState.AddModelError("", "Something went wrong while saving");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully Created");

		}

		[HttpPut("{pokeId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult UpdateCategory(int pokeId, [FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDto UpdatedPokemon)
		{
			if (UpdatedPokemon == null)
				return BadRequest();

			if (pokeId != UpdatedPokemon.Id)
				return BadRequest(ModelState);

			if (!_pokemonRepository.PokemonExists(pokeId))
				return NotFound();

			if (!ModelState.IsValid)
				return BadRequest();

			var UpdatedPokemonMap = _mapper.Map<Pokemon>(UpdatedPokemon);
			if (!_pokemonRepository.UpdatePokemon(ownerId,catId, UpdatedPokemonMap))
			{
				ModelState.AddModelError("", "Something went wrong updaing category");
				return StatusCode(500, ModelState);
			}
			return NoContent();

		}

		[HttpDelete("{pokeId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult DeletePokemon(int pokeId)
		{
			if (!_pokemonRepository.PokemonExists(pokeId))
				return NotFound();

			var reviewToDelete=_reviewRepository.GetReviewsOfPokemon(pokeId);
			var pokemonToDelete = _pokemonRepository.GetPokemon(pokeId);

			if (!ModelState.IsValid)
				return BadRequest();

			if (!_reviewRepository.DeleteReviews(reviewToDelete.ToList()))
			{
				ModelState.AddModelError("", "Something went wrong deleting pokemon Reviews");
				return StatusCode(500, ModelState);
			}


			if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
			{
				ModelState.AddModelError("", "Something went wrong deleting pokemon");
				return StatusCode(500, ModelState);
			}
			return NoContent();
		}



	}
}
