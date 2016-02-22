using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TANAPa_ePlan.Models
{



    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class UserInRole
    {
        [Key, Column(Order = 1)]
        public int UserId { get; set; }
        [Key, Column(Order = 2)]
        public int RoleId { get; set; }

        [ForeignKey("UserId")]
        public UserProfile UserProfile { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }
    }
}