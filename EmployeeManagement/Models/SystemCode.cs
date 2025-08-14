using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class SystemCode : UserActivity
    {
        [Key]
        public int Id { get; set; }
        public string code { get; set; }
        public string Description { get; set; }
    }
}
 