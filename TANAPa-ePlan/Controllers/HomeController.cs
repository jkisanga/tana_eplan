using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TANAPa_ePlan.Models;

namespace TANAPa_ePlan.Controllers
{
    public class HomeController : Controller
    {
        ContextModel db = new ContextModel();
        public ActionResult MainMenu()
        {
             return PartialView("_MainMenu");
        }

        public ActionResult DeptTopMenu()
        {
            var Mydetail = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            var DeptId = Mydetail.DeptId;
            ViewBag.CountOfficerActivities = db.DailyActivities.Where(x => x.DeptId == DeptId).Where(x => x.Read == 0).Where(x => x.Status != "Approved").Count();
            ViewBag.CountAdhocTask = db.AdhocTasks.Where(x => x.DeptId == DeptId).Where(x => x.Status != "Approved").Where(x => x.Read == 0).Count();
            ViewBag.CountMyStaff = db.UserProfiles.Where(x => x.DeptId == DeptId).Count();
            DateTime today = DateTime.Today;
            var announcementexpiredate = today.AddDays(7);
            ViewBag.MyAnnouncement = db.Announcements.Where(x => x.DeptId == Mydetail.DeptId).Where(x => x.Date <= announcementexpiredate).Count();
            return PartialView("_DeptTopMenu");
        }


        public ActionResult Index()
        {
            var Mydetail = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.myStaff = db.UserProfiles.Where(x => x.DeptId > Mydetail.DeptId).Count();
            var officerPerDept = (from o in db.UserProfiles where o.DeptId == Mydetail.DeptId select new { UserId = o.UserId, Name = o.Firstname + " " + o.Surname });
            ViewBag.PerformedBy = new SelectList(officerPerDept, "UserId", "Name");
            ViewBag.UserId = Mydetail.UserId;
            ViewBag.DeptId = Mydetail.DeptId;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdhocTask(string Description, int DeptId, int UserId, int PerformedBy,  HttpPostedFileBase Attachment)
        {
            AdhocTask adhoc = new AdhocTask();
            if (!(String.IsNullOrEmpty(Description)) && DeptId > 0 && UserId >0 )
            {

                if (Attachment != null && Attachment.ContentLength > 0) 
                {

                    string targetFolder = HttpContext.Server.MapPath("~/App_Data/AdhocTask"); 
                    string fileName = Path.GetFileName(Attachment.FileName); 
                    string path = Path.Combine(targetFolder, fileName); 
                    Attachment.SaveAs(path);
                    adhoc.Attachment = path;
                }
                    adhoc.DeptId = DeptId;
                    adhoc.Description = Description;
                    adhoc.UserId = PerformedBy;
                    adhoc.PerformedBy = UserId;
                    adhoc.Parcent = 1;
                    db.AdhocTasks.Add(adhoc);
                    db.SaveChanges();
                    
                

                return RedirectToAction("Index");

            }
            return RedirectToAction("Index");
        }


        //========================= Announcement ===========================
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Annoucement(string Message, int DeptId, int PerformedBy, HttpPostedFileBase Attachment)
        {
            var announcement = new Announcement();
            if (!(String.IsNullOrEmpty(Message)) && DeptId > 0 && PerformedBy > 0)
            {

                if (Attachment != null && Attachment.ContentLength > 0)
                {

                    string targetFolder = HttpContext.Server.MapPath("~/App_Data/Annuncement");
                    string fileName = Path.GetFileName(Attachment.FileName);
                    string path = Path.Combine(targetFolder, fileName);
                    Attachment.SaveAs(path);
                    announcement.Attachment = path;
                    announcement.DeptId = DeptId;
                    announcement.Message = Message;
                    announcement.UserId = PerformedBy;
                    db.Announcements.Add(announcement);
                    db.SaveChanges();
                }
                else
                {
                    announcement.DeptId = DeptId;
                    announcement.Message = Message;
                    announcement.UserId = PerformedBy;
                    db.Announcements.Add(announcement);
                    db.SaveChanges();

                }

                return RedirectToAction("Index");

            }
            return RedirectToAction("Index");
        }



        //========================= DeptActivity ===============================
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeptActivity(string Activity, int DeptId)
        {
            DeptActivity dailyActivity = new DeptActivity();
            if (!(String.IsNullOrEmpty(Activity)) && DeptId > 0 )
            {


                dailyActivity.DeptId = DeptId;
                dailyActivity.Activity = Activity;
                db.DeptActivities.Add(dailyActivity);
                db.SaveChanges();  

            }
            return RedirectToAction("Index");
        }


        //================== List Today Dept Activity ============================
        public ActionResult TodayDeptActivity()
        {
            var MyDetail = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            var DeptId = MyDetail.DeptId;
            ViewBag.DeptId = MyDetail.DeptId;
            var todayDeptActivity = db.DailyActivities.Include(x => x.DeptActivity).Include(x => x.UserProfile).Where(x => x.DeptId == DeptId).Where(x => x.CreatedAt == DateTime.Today).ToList();

            return PartialView("_TodayDeptActivity", todayDeptActivity);
        }

        //================== List Today Adhock ============================
        public ActionResult TodayDeptAdhocTask()
        {
            var MyDetail = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            var DeptId = MyDetail.DeptId;
            var todayDeptAdhoc = db.AdhocTasks.Include(x => x.UserProfile).Where(x => x.DeptId == DeptId).Where(x => x.CreatedAt == DateTime.Today).ToList();

            return PartialView("_TodayDeptAdhocTask", todayDeptAdhoc);
        }


        //====================== Our Activities ===================================
        public ActionResult ListOurActivity()
        {
            var UserIdObj = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            var activityList = db.DeptActivities.Where(x => x.DeptId == UserIdObj.DeptId).ToList();
            return PartialView("_ListOurActivity", activityList);
        }

        //================== Remove Our Activity =================================
       [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RemoveOurActivity(int id)
        {

            var activity = db.DeptActivities.Find(id);
            db.DeptActivities.Remove(activity);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //====================== DetpActivityReview ==================
       public ActionResult DetpActivityReview(int id)
       {
           var UserIdObj = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
           ViewBag.HeadId = UserIdObj.UserId;
           var dailyActivityObj = db.DailyActivities.Include(x => x.UserProfile).Include(x => x.DeptActivity).Where(x => x.DailyActivityId == id).FirstOrDefault();
           ViewBag.Attach = dailyActivityObj.Attachment;
           ViewBag.DailyActivityId = dailyActivityObj.DailyActivityId;
           return View(dailyActivityObj);
       }


       //====================== DetpActivityReview ==================
       public ActionResult ReviewDeptAdhocTask(int id)
       {
           var UserIdObj = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
           ViewBag.HeadId = UserIdObj.UserId;
           var dailyAdhocObj = db.AdhocTasks.Include(x => x.UserProfile).Where(x => x.AdhocTaskId == id).FirstOrDefault();
           ViewBag.Attach = dailyAdhocObj.Attachment;
           ViewBag.DailyActivityId = dailyAdhocObj.AdhocTaskId;
           return View(dailyAdhocObj);
       }

        //Report eveluation for daily activities 
       [AcceptVerbs(HttpVerbs.Post)]
       public ActionResult ReportEvaluation(int DeptId, int HeadId, int DailyActivityId, string SupervisorComment, string Status, string Attachment)
       {
           
           var dailyReview = db.DailyActivities.Find(DailyActivityId);
           if (dailyReview != null && !(String.IsNullOrEmpty(Status)) )
           {
           
           dailyReview.HeadId = HeadId;
           dailyReview.SupervisorComment = SupervisorComment;
           dailyReview.Status = Status;
           dailyReview.HeadId = HeadId;
           dailyReview.Read = 1;
           db.Entry(dailyReview).State = EntityState.Modified;
           db.SaveChanges();
           return RedirectToAction("Index");
           }
           var UserIdObj = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
           ViewBag.HeadId = UserIdObj.UserId;
           var dailyActivityObj = db.AdhocTasks.Include(x => x.UserProfile).Where(x => x.AdhocTaskId == DailyActivityId).FirstOrDefault();
           return View("DetpActivityReview", new { id = DailyActivityId});
       }


       //Report eveluation for ad hoc task
       [AcceptVerbs(HttpVerbs.Post)]
       public ActionResult ReportAdhocEvaluation(int DeptId, int AdhocTaskId, string SupervisorComment, string Status, string Attachment)
       {

           var dailyReview = db.AdhocTasks.Find(AdhocTaskId);
           if (dailyReview != null && !(String.IsNullOrEmpty(Status)))
           {

               dailyReview.SupervisorComment = SupervisorComment;
               dailyReview.Status = Status;
               dailyReview.Read = 1;
               db.Entry(dailyReview).State = EntityState.Modified;
               db.SaveChanges();
               return RedirectToAction("Index");
           }
           var UserIdObj = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
           ViewBag.HeadId = UserIdObj.UserId;
           var dailyActivityObj = db.AdhocTasks.Include(x => x.UserProfile).Where(x => x.AdhocTaskId == AdhocTaskId).FirstOrDefault();
           return View("DetpActivityReview", new { id = AdhocTaskId });
       }



       //================== Adhoc Attachment Download ==================================
       public FileResult AdhocAttachment(int id)
       {
           string filename = (from f in db.AdhocTasks where f.AdhocTaskId == id select f.Attachment).FirstOrDefault();

           string strFileName = filename.ToLower();
           if (strFileName.IndexOf("_") > -1)
           {
               strFileName = strFileName.Substring(strFileName.IndexOf("_"));
           }
           string contentType = "application/pdf";

           //Response.AddHeader("Content-Disposition", "inline; filename="+filename);     
           return File(filename, contentType, strFileName);
       }

        //=================== List department employee ========================
       public ActionResult ListDeptEmployee()
       {
           var UserIdObj = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
           var deptEmployee = db.UserProfiles.Where(x => x.DeptId == UserIdObj.DeptId).ToList();
           return View(deptEmployee);
       }

        //======================== EmpProfile ==================================
       public ActionResult EmpProfile(int id)
       {
           return View();
       }


        //===============Assign activity to officer ======================
       public ActionResult AssignActivity(int id) 
       {
           var ActivityObj = db.DeptActivities.Find(id);
           var Mydetail = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

           ViewBag.DeptId = Mydetail.DeptId;
           ViewBag.HeadId = Mydetail.UserId;

           ViewBag.myStaff = db.UserProfiles.Where(x => x.DeptId > Mydetail.DeptId).Count();
           var officerPerDept = (from o in db.UserProfiles where o.DeptId == Mydetail.DeptId select new { UserId = o.UserId, Name = o.Firstname + " " + o.Surname });
           ViewBag.UserId = new SelectList(officerPerDept, "UserId", "Name");

           return View(ActivityObj);
       }

        [AcceptVerbs(HttpVerbs.Post)]
       public ActionResult AssignActivity(int DeptActivityId, int DeptId, int HeadId, int UserId) 
       {
         
               if (DeptId > 0 && DeptActivityId > 0 && UserId > 0 && HeadId > 0)
               {
                   DailyActivity dailyActivity = new DailyActivity();

                   dailyActivity.DeptActivityId = DeptActivityId;
                   dailyActivity.DeptId = DeptId;
                   dailyActivity.HeadId = HeadId;
                   dailyActivity.UserId = UserId;
                   dailyActivity.Parcent = 1;
                   db.DailyActivities.Add(dailyActivity);
                   db.SaveChanges();

                   return RedirectToAction("Index", "Home");
               }
           

            return RedirectToAction("Index", "Home");
       }
    }
}
