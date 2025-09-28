using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        public string Address { get; set; }

        public string UserName { get; set; }

        [DisplayName("National ID")]
        public string? NationalId { get; set; }
        public string? FullName => $"{FirstName} {MiddleName} {LastName}";
        [DisplayName("User Role")]
        public string? RoleId { get; set; }
    }
}
