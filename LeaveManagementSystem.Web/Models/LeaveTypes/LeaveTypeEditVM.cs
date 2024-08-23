using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Web.Models.LeaveTypes
{
    public class LeaveTypeEditVM : BaseLeaveTypeVM
    {
        [Required]
        [Length(4, 150, ErrorMessage = "You have violated the length requirements")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 90)]
        public int NumberOfDays { get; set; }
    }
}
