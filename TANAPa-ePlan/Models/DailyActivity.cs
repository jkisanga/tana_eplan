using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TANAPa_ePlan.Models
{
    public class DailyActivity
    {
        public DailyActivity()
        {
            CreatedAt = DateTime.Today;
            Status = "Pending";
            Read = 0;
            Parcent = 0;
        }

        [Key]
        public int DailyActivityId { get; set; }
        public int DeptActivityId { get; set; }
        public int DeptId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Remark { get; set; }
        public string Challenge { get; set; }
        public string Reason { get; set; }
        public Decimal Parcent { get; set; }
        public string Status { get; set; }
        public int Read { get; set; }
        public string Attachment { get; set; }
        public string SupervisorComment { get; set; }
        public int? HeadId { get; set; }
  


        [ForeignKey("DeptActivityId")]
        public DeptActivity DeptActivity { get; set; }
        [ForeignKey("UserId")]
        public UserProfile UserProfile { get; set; }
        [ForeignKey("DeptId")]
        public Department Department { get; set; }
    }
}