using System;
using System.Collections.Generic;

namespace DoAn.Models.Movies;

public partial class Director
{
    public int DirectorId { get; set; }

    public string? DirectorName { get; set; }

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
    public virtual ICollection<MovieDirector> MovieDirectors { get; set; } = new List<MovieDirector>();
}
