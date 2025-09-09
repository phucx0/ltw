using System;
using System.Collections.Generic;

namespace DoAn.Models.Movies;

public partial class Actor
{
    public int ActorId { get; set; }

    public string? ActorName { get; set; }

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
