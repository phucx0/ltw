using DoAn.Models.Movies;

namespace DoAn.ViewModels
{
    public class MoviesPageViewModel
    {
        public List<Movie> TredingMovies { get; set; } = new List<Movie>();
        public List<Movie> NowShowingMovies { get; set; } = new List<Movie>();
    }
}
