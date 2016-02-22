using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TANAPa_ePlan.Models
{
    public class AdhocTask
    {
        public AdhocTask()
        {
            CreatedAt = DateTime.Today;
            Status = "Pending";
            Read = 0;
            Parcent = 0;
        }

        [Key]
        public int AdhocTaskId { get; set; }
        public int DeptId { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public int? PerformedBy { get; set; }
        public DateTime? PerformedOn { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string Challenge { get; set; }
        public string Remark { get; set; }
        public string Attachment { get; set; }
        public string Attachment2 { get; set; }
        public Decimal Parcent { get; set; }
        public int Read { get; set; }
        public string SupervisorComment { get; set; }

        [ForeignKey("DeptId")]
        Department Department { get; set; }
        [ForeignKey("UserId")]
        public UserProfile UserProfile { get; set; }
    }
}