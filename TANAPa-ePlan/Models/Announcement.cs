using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TANAPa_ePlan.Models
{
    public class Announcement
    {
        public Announcement()
        {
            Date = DateTime.Now;
            Status = 0;
        }

        [Key]
        public int AnnoucementId { get; set; }
        public int DeptId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public string Attachment { get; set; }


        [ForeignKey("DeptId")]
        public Department Department { get; set; }
        [ForeignKey("UserId")]
        public UserProfile UserProfile { get; set; }
    }
}