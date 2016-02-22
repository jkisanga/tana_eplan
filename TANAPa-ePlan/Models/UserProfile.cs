using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TANAPa_ePlan.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
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

        public int Status { get; set; }
        public string DeactivateReason { get; set; }
        public int isLogin { get; set; }

        [ForeignKey("DeptId")]
        public Department Department { get; set; }
   
    }


    [Table("webpages_Membership")]
    public class webpages_Membership
    {
        [Key]
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public string ConfirmationToken { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime LastPasswordFailureDate { get; set; }
        public int PasswordFailuresSinceLastSuccess { get; set; }
        public string Password { get; set; }
        public DateTime? PasswordChangeDate { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordVerificationToken { get; set; }
        public DateTime PasswordVerificationTokenExpirationDate { get; set; }
    }
}