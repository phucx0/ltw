using DoAn.Models.Booking;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models.Movies;

[Table("movies", Schema = "dbo")]
public partial class Movie
{
    [Column("movie_id")]
    public int MovieId { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("duration")]
    public int? Duration { get; set; }

    [Column("genre")]
    public string? Genre { get; set; }

    [Column("rating_id")]
    public int? RatingId { get; set; }

    [Column("release_date")]
    public DateTime? ReleaseDate { get; set; }

    [Column("poster_url")]
    public string? PosterUrl { get; set; }

    [Column("cover_url")]
    public string? CoverUrl { get; set; }

    [Column("trailer_url")]
    public string? TrailerUrl { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [Column("imdb_rating")]
    public decimal? ImdbRating { get; set; }
    

    public virtual AgeRating? AgeRating { get; set; }

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();

    public virtual ICollection<Actor> Actors { get; set; } = new List<Actor>();

    public virtual ICollection<Director> Directors { get; set; } = new List<Director>();
    public virtual ICollection<MovieDirector> MovieDirectors { get; set; } = new List<MovieDirector>();

    public virtual ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();

}
