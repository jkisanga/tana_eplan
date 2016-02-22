using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TANAPa_ePlan.Models
{
    

    //=========== Finansial Year Model =====================
    public class FYear
    {
        public FYear()
        {
            Status = "Inactive";
        }
        [Key]
        public int FYearId { get; set; }
        public string Year { get; set; }
        public string Status { get; set; }
    }

    //================= Quarter Model ======================
    public class Quarter
    {
        public Quarter() { Status = "Inactive"; }
        [Key]
        public int QId { get; set; }
        public int FYearId { get; set; }
        public string QuarterName { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Status { get; set; }

        [ForeignKey("FYearId")]
        public FYear FYear { get; set; }
    }

    //============ Month  ===================================
    public class Month
    {
        public Month() { Status = "Inactive"; }
        [Key]
        public int MonthId { get; set; }
        public int QId { get; set; }
        public DateTime MonthName { get; set; }
        public string Status { get; set; }

        [ForeignKey("QId")]
        public Quarter Quarter { get; set; }
    }


    //================ Weekl =====================================
    public class Week
    {
        public Week() { Status = "Inactive"; }
        [Key]
        public int WeekId { get; set; }
        public int MonthId { get; set; }
        public string WeekName { get; set; }
        public string Status { get; set; }

        [ForeignKey("MonthId")]
        public Month Month { get; set; }
    
    }

    //=============== Department ==============================
    public class Department
    {
        [Key]
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public string DeptCode { get; set; }
    
    }



    //================= Objective ============================
    public class Objective
    {
        public Objective() { delete = 0; }
        [Key]
        public int ObjectiveId { get; set; }
        public string ODescription { get; set; }
        public string OCode { get; set; }
        public int FYearId { get; set; }
        public int delete { get; set; }

        [ForeignKey("FYearId")]
        public FYear FYear { get; set; }

    }

    //================= Target ==============================
    public class Target
    {
        public Target() { delete = 0; }
        [Key]
        public int TargetId { get; set; }
        public int ObjectiveId { get; set; }
        public string TargetNo { get; set; }
        public string TDescription { get; set; }
        public int DeptId { get; set; }
        public int delete { get; set; }

        [ForeignKey("ObjectiveId")]
        public Objective Objective { get; set; }
        [ForeignKey("DeptId")]
        public Department Department { get; set; }
    }


    //================= Activity ===============================
    public class Activity
    {
        public Activity() { delete = 0; }
        [Key]
        public int ActivityId { get; set; }
        [Required]
        public int TargetId { get; set; }
        [Required]
        public string ActivityNo { get; set; }
        public string ADescription { get; set; }
        public Decimal Budget { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Challenge { get; set; }
        public string wayforward { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public int delete { get; set; }

        [ForeignKey("TargetId")]
        public Target Target { get; set; }

    }

    //================ Addhoc Tasks. Thise are task that belongs to Activity ==============================
    public class Task
    {
        [Key]
        public int TaskId { get; set; }
        public int ActivityId { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserProfile UserProfile { get; set; }
    }



}