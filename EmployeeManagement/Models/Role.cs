using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class Role
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
