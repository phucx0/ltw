using DoAn.Models.Booking;
using DoAn.Models.Movies;

namespace DoAn.ViewModels
{
    public class MovieDetailViewModel
    {
        public Movie Movie { get; set; }
        public List<Showtime> Showtimes { get; set; }
    }
}
