using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.View_Models
{
    class MemberToUpdateViewModel
    {
        public string ? Name { get; set; }
        public string ? Photo { get; set; }
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        [DataType(DataType.EmailAddress)] // UI Hint
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 2 and 50")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone is Required")]
        [Phone(ErrorMessage = "Invalid Phone Format")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone must Egyption format")]
        public string  Phone { get; set; } = null!;

        public int BuildingNumber { get; set; }
        [Required(ErrorMessage = "City is Required")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "City between 2 and 30")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City must contain letters and spaces only")]

        public string City { get; set; } = null!;
        [Required(ErrorMessage = "Street is Required")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Street between 2 and 30")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Street must contain letters and spaces only")]

        public string Street { get; set; } = null!;

    }
}
