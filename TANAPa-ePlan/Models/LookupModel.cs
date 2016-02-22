using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TANAPa_ePlan.Models
{
    public class DeptActivity
    {
        [Key]
        public int DeptActivityId { get; set; }
        public int DeptId { get; set; }
        public string Activity { get; set; }

        [ForeignKey("DeptId")]
        public Department Department { get; set; }
    }


}