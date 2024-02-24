using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
	public class ReviewerRepository : IReviewerRepository
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public ReviewerRepository(DataContext context,IMapper mapper)
        {
			this._context = context;
			this._mapper = mapper;
		}
        public Reviewer GetReviewer(int id)
		{
			return _context.Reviewers.Where(r => r.Id == id).Include(e=>e.Reviews).FirstOrDefault();
		} 

		public ICollection<Reviewer> GetReviewers()
		{
			return _context.Reviewers.ToList();
		}

		public ICollection<Review> GetReviewsByReviewer(int id)
		{
			return _context.Reviews.Where(r=>r.Reviewer.Id==id).ToList();
		}

		public bool ReviewerExists(int id)
		{
			return _context.Reviewers.Any(r => r.Id == id);
		}
	}
}
