using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
	public class CategoryRepository : ICategoryRepository
	{
		private DataContext _context;
        public CategoryRepository(DataContext context)
        {
             this._context = context;
        }
        public bool CategoryExists(int id)
		{
			return _context.Categories.Any(c=>c.Id == id);
		}

		public ICollection<Category> GetCategories()
		{
			return _context.Categories.OrderBy(c=>c.Id).ToList();
		}

		public Category GetCategory(int id)
		{
			return _context.Categories.Where(e => e.Id == id).FirstOrDefault();
		}

		public ICollection<Pokemon> GetPokemonByCategory(int categoryId)
		{
			return _context.PokemonCategories.Where(e=>e.CategoryId== categoryId).Select(c=>c.Pokemon).ToList();
		}
	}
}
