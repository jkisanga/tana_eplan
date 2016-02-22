using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TANAPa_ePlan.Models
{
    public class ContextModel : DbContext
    {
        public DbSet<FYear> FYears { get; set; }
        public DbSet<Quarter> Quarters { get; set; }
        public DbSet<Month> Months { get; set; }
        public DbSet<Week> Weeks { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Objective> Objectives { get; set; }
        public DbSet<Target> Targets { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Task> Tasks { get; set; }

        //===================================================
        public DbSet<DailyActivity> DailyActivities { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<AdhocTask> AdhocTasks { get; set; }
        public DbSet<DeptActivity> DeptActivities { get; set; }

        public DbSet<Role> Roles { get; set; }
        public DbSet<UserInRole> UserInRoles { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserProfile>()
                    .HasRequired(e => e.Department)
                    .WithMany()
                    .HasForeignKey(e => e.DeptId)
                    .WillCascadeOnDelete(false);
        modelBuilder.Entity<DailyActivity>()
               .HasRequired(e => e.Department)
               .WithMany()
               .HasForeignKey(e => e.DeptId)
               .WillCascadeOnDelete(false);
        
    }

        
    }
}