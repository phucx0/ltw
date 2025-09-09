using DoAn.Models.Booking;
using System;
using System.Collections.Generic;

namespace DoAn.Models.Movies;

public partial class Movie
{
    public int MovieId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public short? Duration { get; set; }

    public string? Genre { get; set; }

    public int? RatingId { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public string? PosterUrl { get; set; }

    public string? TrailerUrl { get; set; }

    public string? Status { get; set; }

    public decimal? ImdbRating { get; set; }

    public virtual AgeRating? Rating { get; set; }

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();

    public virtual ICollection<Actor> Actors { get; set; } = new List<Actor>();

    public virtual ICollection<Director> Directors { get; set; } = new List<Director>();
}
