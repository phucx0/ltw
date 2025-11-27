using DoAn.Models.Data;
using DoAn.Models.Movies;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Services
{
    public class MovieService
    {
        private readonly ModelContext _context;
        public MovieService(ModelContext context)
        {
            _context = context;
        }
        public Task<Movie?> GetMovieById(int id)
        {
            return _context.Movies
                .Include(m => m.AgeRating)
                .Include(m => m.MovieDirectors)
                .Include(m => m.MovieActors)
                .Include(m => m.Showtimes)
                    .ThenInclude(s => s.Room)
                        .ThenInclude(r => r.Branch)
                .FirstOrDefaultAsync(m => m.MovieId == id);
        }
    }
}
