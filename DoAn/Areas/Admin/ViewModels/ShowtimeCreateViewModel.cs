using DoAn.Models.Booking;
using DoAn.Models.Cinema;
using DoAn.Models.Movies;

namespace DoAn.Areas.Admin.ViewModels
{
    public class ShowtimeCreateViewModel
    {
        public int MovieId { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int BranchId { get; set; }

        // Thông tin chính để tạo suất chiếu
        public Showtime Showtime { get; set; }

        // Dùng để hiển thị dropdown
        public List<Movie> Movies { get; set; }
        public List<Branch> Branches { get; set; }
        public List<Room> Rooms { get; set; }

    }

}
