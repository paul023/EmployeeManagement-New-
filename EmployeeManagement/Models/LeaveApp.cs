using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class LeaveApp : ApprovalActivity
    {
        public int Id { get; set; }
        [Display(Name ="Employee Name")]
        public int EmployeeId { get; set; }
         
        public Employee Employee { get; set; }
        [Required]
        [Range(1, 365, ErrorMessage = "Number of days must be between 1 and 365.")]
        public int NoOfDays { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        [Display(Name = "Leave Duration")]
        public int DurationId { get; set; }

        public SystemCodeDetail Duration { get; set; }

        public int LeaveTypeId { get; set; }
        public LeaveType LeaveType { get; set; }
        public string? Attachment { get; set; }
        [Display(Name = "Notes")]
        public string Description { get; set; }

        public int StatusId { get; set; }
        public SystemCodeDetail Status { get; set; }
        public string? AttachmentPath { get; internal set; }


        [Display(Name = "Approval Notes")]
        public string? ApprovalNotes { get; set; }
    }
}
