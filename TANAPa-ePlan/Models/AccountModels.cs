using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace TANAPa_ePlan.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }



    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    //


    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        
       

        public string EmployeeCode { get; set; }
        public string Firstname { get; set; }
        public string Othername { get; set; }
        public string Surname { get; set; }
        public string Branch { get; set; }
        public int DeptId { get; set; }
        public string Section { get; set; }
        public string Unit { get; set; }
        public string Job { get; set; }
        public string CostCenter { get; set; }

        public string GradeGroup { get; set; }
        public string Grade { get; set; }
        public string GradeLevel { get; set; }
        public Decimal Increment { get; set; }
        public string EmployementType { get; set; }
        public DateTime? DateHired { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string Religion { get; set; }




        public string Telephone { get; set; }
        public string Email { get; set; }
        public string EmergencyContact { get; set; }
        public string Bank { get; set; }
        public string Membership { get; set; }
        public string DepartmentGroup { get; set; }
        public string SectionGroup { get; set; }
        public string UnitGroup { get; set; }
        public string Team { get; set; }
        public string JobGroup { get; set; }
        public string EmploymentType { get; set; }




        public string ClassGroup { get; set; }
        public string Class { get; set; }
        public DateTime? AnniversaryDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? ReinstatementDate { get; set; }
        public DateTime? ProbationStartDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public DateTime? SuspendedFromDate { get; set; }
        public DateTime? SuspendedToDate { get; set; }
        public DateTime? EndOfContract { get; set; }
        public string PresentAddress { get; set; }

       
        
    }

    public class UserListView
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public int UserId { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
