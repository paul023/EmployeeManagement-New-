using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class ProfileViewModel
    {

        public ICollection<SystemProfile> Profiles { get; set; }
        [DisplayName("Role")]
        public string RoleId { get; set; }

       
        [DisplayName("System Task")]
        public int TaskId { get; set; }


    }

   
}
