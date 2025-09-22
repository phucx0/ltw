namespace DoAn.Models.Movies
{
    public class MovieActor
    {
        public int MovieId { get; set; }
        public int ActorId { get; set; }

        public Movie Movie { get; set; }
        public Actor Actor { get; set; }

        public virtual ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();

    }
}
