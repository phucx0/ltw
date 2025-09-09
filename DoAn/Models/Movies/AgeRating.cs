using System;
using System.Collections.Generic;

namespace DoAn.Models.Movies;

public partial class AgeRating
{
    public int RatingId { get; set; }

    public string? RatingCode { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
