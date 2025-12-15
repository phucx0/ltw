using System.ComponentModel.DataAnnotations;

namespace DoAn.Areas.Admin.ViewModels
{
    public class EmployeeEditViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên không quá 100 ký tự")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        //[Required(ErrorMessage = "Vui lòng chọn giới tính")]
        //public string Gender { get; set; }

        //[StringLength(200, ErrorMessage = "Địa chỉ không quá 200 ký tự")]
        //public string Address { get; set; }

        //[Required(ErrorMessage = "Vui lòng chọn chi nhánh")]
        //public int BranchId { get; set; }

        public bool IsActive { get; set; }
    }
}
