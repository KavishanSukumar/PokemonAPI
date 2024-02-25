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
	public class ReviewController :Controller
	{
		private readonly IReviewRepository _reviewRepository;
		private readonly IMapper _mapper;
		private readonly IPokemonRepository _pokemonRepository;
		private readonly IReviewerRepository _reviewerRepository;

		public ReviewController(IReviewRepository reviewRepository, IMapper mapper, IPokemonRepository pokemonRepository, IReviewerRepository reviewerRepository)
		{
			this._reviewRepository = reviewRepository;
			this._mapper = mapper;
			_pokemonRepository = pokemonRepository;
			_reviewerRepository = reviewerRepository;
		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
		public IActionResult GetReviews()
		{
			var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(reviews);
		}

		[HttpGet("{reviewId}")]
		[ProducesResponseType(200, Type = typeof(Review))]
		[ProducesResponseType(400)]
		public IActionResult GetCountry(int reviewId)
		{
			if (!_reviewRepository.ReviewExists(reviewId))
			{
				return NotFound();
			}
			var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			return Ok(review);

		}
		[HttpGet("pokemon/{pokeId}")]
		[ProducesResponseType(200, Type = typeof(Review))]
		[ProducesResponseType(400)]
		public IActionResult GetReviewsForAPokemon(int pokeId)
		{
			var reviews=_mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfPokemon(pokeId));
			
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			return Ok(reviews);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreateReview([FromQuery]int reviewerId, [FromQuery]int pokeId, [FromBody] ReviewDto reviewCreate)
		{
			if (reviewCreate == null)
				return BadRequest(ModelState);

			var review = _reviewRepository.GetReviews()
				.Where(c => c.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper())
				.FirstOrDefault();

			if (review != null)
			{
				ModelState.AddModelError("", "Review already exists");
				return StatusCode(422, ModelState);
			}
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var reviewMap = _mapper.Map<Review>(reviewCreate);
			reviewMap.Pokemon=_pokemonRepository.GetPokemon(pokeId);
			reviewMap.Reviewer=_reviewerRepository.GetReviewer(reviewerId);

			if (!_reviewRepository.CreateReview(reviewMap))
			{
				ModelState.AddModelError("", "Something went wrong while saving");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully Created");

		}


		[HttpPut("{reviewId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto Updatedreview)
		{
			if (Updatedreview == null)
				return BadRequest();

			if (reviewId != Updatedreview.Id)
				return BadRequest(ModelState);

			if (!_reviewRepository.ReviewExists(reviewId))
				return NotFound();

			if (!ModelState.IsValid)
				return BadRequest();

			var UpdatedreviewMap = _mapper.Map<Review>(Updatedreview);
			if (!_reviewRepository.UpdateReview(UpdatedreviewMap))
			{
				ModelState.AddModelError("", "Something went wrong updaing Reviw");
				return StatusCode(500, ModelState);
			}
			return NoContent();

		}

		[HttpDelete("{reviewId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult DeleteReview(int reviewId)
		{
			if (!_reviewRepository.ReviewExists(reviewId))
				return NotFound();

			var reviewToDelete = _reviewRepository.GetReview(reviewId);

			if (!ModelState.IsValid)
				return BadRequest();

			if (!_reviewRepository.DeleteReview(reviewToDelete))
			{
				ModelState.AddModelError("", "Something went wrong deleting review");
				return StatusCode(500, ModelState);
			}
			return NoContent();
		}

	}


}
