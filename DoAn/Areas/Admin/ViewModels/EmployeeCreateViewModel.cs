using System.ComponentModel.DataAnnotations;

namespace DoAn.Areas.Admin.ViewModels
{
    public class EmployeeCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        public DateTime Birthday { get; set; } // Đổi thành nullable
    }
}
