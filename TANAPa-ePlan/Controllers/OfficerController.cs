using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TANAPa_ePlan.Models;

namespace TANAPa_ePlan.Controllers
{
    //[Access(Roles = "officer")]
    public class OfficerController : Controller
    {
        public ContextModel db = new ContextModel();
        //
        // GET: /Officer/

        public ActionResult TopMenu()
        {
            var Mydetail = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.CountMyDuties = db.DailyActivities.Where(x => x.Status != "Approved").Where(x => x.UserId == Mydetail.UserId).Count();

            ViewBag.CountMyAdhocTask = db.AdhocTasks.Where(x => x.Status != "Approved").Where(x => x.PerformedBy == Mydetail.UserId).Count();
           DateTime today = DateTime.Today;
            var announcementexpiredate = today.AddDays(7);
            ViewBag.MyAnnouncement = db.Announcements.Where(x => x.DeptId == Mydetail.DeptId).Where(x => x.Date <= announcementexpiredate).Count();
            return PartialView("_TopMenu");
        }


        public ActionResult Index()
        {
            var UserIdObj = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            var Mydetail = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.myStaff = db.UserProfiles.Where(x => x.DeptId == UserIdObj.DeptId).Count();
            var officerPerDept = (from o in db.UserProfiles where o.DeptId == UserIdObj.DeptId select new { UserId = o.UserId, Name = o.Firstname + " " + o.Surname });
           
            
            ViewBag.UserId = UserIdObj.UserId;
            ViewBag.PerformedBy = UserIdObj.UserId;
            ViewBag.MyName = UserIdObj.Firstname + " " + UserIdObj.Surname;
            ViewBag.DeptId = UserIdObj.DeptId;

            return View();
        }


        public ActionResult OurActivity()
        {
            var UserIdObj = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            var activityList = db.DeptActivities.Where(x => x.DeptId == UserIdObj.DeptId).ToList();
            return PartialView("_OurActivity", activityList);
        }


        [HttpPost]
        public ActionResult DailyActivity(int[] DeptActivityId, int PerformedBy, int DeptId)
        {
            DailyActivity dailyActivity = new DailyActivity();
            if (DeptActivityId != null)
            {

                foreach (var deptactivityid in DeptActivityId)
                {
                    dailyActivity.DeptActivityId = deptactivityid;
                    dailyActivity.UserId = PerformedBy;
                    dailyActivity.DeptId = DeptId;
                    db.DailyActivities.Add(dailyActivity);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Could not add Activity make sure Activities are selected");
            }
            return RedirectToAction("Index");

        }


        //===================== Adhoc Task =========================================================
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdhocTask(string Description, int DeptId, int UserId, int? PerformedBy, HttpPostedFileBase Attachment)
        {
            AdhocTask adhoc = new AdhocTask();
            if (!(String.IsNullOrEmpty(Description)) && DeptId > 0 && UserId > 0)
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
                    adhoc.UserId = UserId;
                    adhoc.PerformedBy = PerformedBy;
                    db.AdhocTasks.Add(adhoc);
                    db.SaveChanges();

                return RedirectToAction("Index");

            }
            return RedirectToAction("Index");
        }

        //================== My Duties ===============================
        public ActionResult MyDuties()
        {
            var UserIdObj = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            var UserId = UserIdObj.UserId;
           var myDuties = db.DailyActivities.Include(x => x.DeptActivity).Where(x => x.UserId == UserId).Where(x => x.CreatedAt == DateTime.Today).ToList();
           ViewBag.myDuties = db.DailyActivities.Include(x => x.DeptActivity).Where(x => x.UserId == UserId).Where(x => x.CreatedAt == DateTime.Today).Where(x => x.SupervisorComment == null).Count();
            
            return PartialView("_MyDuties", myDuties);
        }

        public ActionResult ReportMyDuty(int id)
        {
            var duty = db.DailyActivities.Include(x => x.DeptActivity).Where(x => x.DailyActivityId == id).FirstOrDefault();
            if(duty != null)
            {

                return View(duty);
            }
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReportMyDuty(int DailyActivityId, string Challenge,  HttpPostedFileBase Attachment )
        {
            var myReport = db.DailyActivities.Find(DailyActivityId);
            if(myReport != null)
            {
                if (Attachment != null && Attachment.ContentLength > 0)
                {

                    string targetFolder = HttpContext.Server.MapPath("~/App_Data/DutyReport");
                    string fileName = Path.GetFileName(Attachment.FileName);
                    string path = Path.Combine(targetFolder, fileName);
                    Attachment.SaveAs(path);
                    myReport.Attachment = path;
                }

                myReport.Challenge = Challenge;
                myReport.Status = "Waiting Approval";
                db.Entry(myReport).State = EntityState.Modified;
                db.SaveChanges();           
            }

            return RedirectToAction("Index");
        }

        //========================= End MyDuty =======================================


        //================== My Adhc Task ===============================
        public ActionResult MyAdhocTask()
        {
            var UserIdObj = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            var UserId = UserIdObj.UserId;
            var myDuties = db.AdhocTasks.Where(x => x.UserId == UserId).Where(x => x.CreatedAt == DateTime.Today).ToList();
            ViewBag.myAdhock = db.AdhocTasks.Where(x => x.UserId == UserId).Where(x => x.CreatedAt == DateTime.Today).Count();

            return PartialView("_MyAdhocTask", myDuties);
        }

        public ActionResult ReportMyAdhocTask(int id)
        {
            var duty = db.AdhocTasks.Include(x => x.UserProfile).Where(x => x.AdhocTaskId == id).FirstOrDefault();
            if (duty != null)
            {

                return View(duty);
            }
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReportMyAdhocTask(int AdhocTaskId, string Remark, string Challenge, HttpPostedFileBase Attachment)
        {
            var myReport = db.AdhocTasks.Find(AdhocTaskId);
            if (myReport != null)
            {
                if (Attachment != null && Attachment.ContentLength > 0)
                {

                    string targetFolder = HttpContext.Server.MapPath("~/App_Data/AdhocTask");
                    string fileName = Path.GetFileName(Attachment.FileName);
                    string path = Path.Combine(targetFolder, fileName);
                    Attachment.SaveAs(path);
                    myReport.Attachment2 = path;
                }

                myReport.Challenge = Challenge;
                myReport.Remark = Remark;
                myReport.Status = "Waiting Approval";
                db.Entry(myReport).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //========================= End MyDuty =======================================


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


        //============= Remove Activity In Day ==============================
    
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RemoveActivityInDay(int id)
        {
            var dailyActivityObj = db.DailyActivities.Find(id);
            db.DailyActivities.Remove(dailyActivityObj);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
