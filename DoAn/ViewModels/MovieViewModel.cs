using DoAn.Models.Movies;
using System.Collections.Generic;

namespace DoAn.ViewModels
{
    public class MovieViewModel
    {
        public int? MovieId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string Genre { get; set; }
        public int? RatingId { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string PosterUrl { get; set; }
        public string CoverUrl { get; set; }
        public string TrailerUrl { get; set; }
        public string Status { get; set; }
        public decimal? ImdbRating { get; set; }
        public List<string> SelectedGenres { get; set; } = new();
        public List<string> AllGenres { get; set; } = new() { "Action", "Comedy", "Crime", "Drama", "Fantasy", "Horror", "Sci-Fi" };


        // Actors & Directors
        public List<int> SelectedActorIds { get; set; } = new();
        public List<int> SelectedDirectorIds { get; set; } = new();

        // For dropdown list
        public List<Actor> AllActors { get; set; }
        public List<Director> AllDirectors { get; set; }
        public List<AgeRating> AllRatings { get; set; }
    }
}
