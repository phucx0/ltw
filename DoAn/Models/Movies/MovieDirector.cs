namespace DoAn.Models.Movies
{
    public class MovieDirector
    {
        public int MovieId { get; set; }
        public int DirectorId { get; set; }

        public Movie Movie { get; set; }
        public Director Director { get; set; }
        //public virtual ICollection<MovieDirector> MovieDirectors { get; set; } = new List<MovieDirector>();

    }
}
