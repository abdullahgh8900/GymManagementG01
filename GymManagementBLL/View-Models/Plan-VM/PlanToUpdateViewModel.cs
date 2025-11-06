using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.View_Models.Plan_VM
{
    public class PlanToUpdateViewModel
    {
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description is Required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Description Between 5 and 50")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Duration days is Required")]
        [Range(1, 365, ErrorMessage = "Duration days between 1 and 365")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(250, 10000, ErrorMessage = "Price must be between 250 and 10000 EGP")]
        public decimal Price { get; set; }
    }
}
