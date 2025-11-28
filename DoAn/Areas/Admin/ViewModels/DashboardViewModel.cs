namespace DoAn.Areas.Admin.ViewModels
{
    public class DashboardViewModel
    {
        // Thống kê tổng quan
        public int TotalMovies { get; set; }
        public int TotalBranches { get; set; }
        public int TotalRooms { get; set; }
        public int TotalUsers { get; set; }

        // Thống kê hôm nay
        public int TodayShowtimes { get; set; }
        public int TodayTicketsSold { get; set; }
        public decimal TodayRevenue { get; set; }

        // Thống kê tháng này
        public decimal MonthlyRevenue { get; set; }
        public int MonthlyTicketsSold { get; set; }
    }
}
