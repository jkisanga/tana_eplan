using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TANAPa_ePlan.Models;

namespace TANAPa_ePlan.Controllers
{
    [Access(Roles = "admin")]
    public class AdminController : Controller
    {
        private ContextModel db = new ContextModel();

        //
        // GET: /Setup/

        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult ListUser()
        {
     
            return View(db.UserProfiles.Include(x => x.Department).Where(x => x.UserName!= "Admin").ToList());
        }

        public ActionResult DeptList()
        {
            return PartialView("_DeptList", db.Departments.ToList());
        }

        //
        // GET: /Setup/Details/5

        public ActionResult Details(int id = 0)
        {
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // GET: /Setup/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Setup/Create

        [HttpPost]
        public ActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            return View(department);
        }

        //======================== Start Quarter ========================================

        //List Quarters
        public ActionResult ListQuarter()
        {
            return PartialView("_ListQuarter", db.Quarters.Include(x => x.FYear).ToList());
        }



        //Create Quarter
        public ActionResult AddQuarter()
        {

            var FYear = db.FYears.Where(x => x.Status == "Active").FirstOrDefault();
            if (FYear != null)
            {
                ViewBag.FYear = FYear.Year;
                ViewBag.FYearId = FYear.FYearId;
            }
            return View();
        }

        //post Quarter
        [HttpPost]
        public ActionResult AddQuarter(int FYearId, string QuarterName, DateTime Start, DateTime End)
        {
            Quarter quarter = new Quarter();
            quarter.FYearId = FYearId;
            quarter.QuarterName = QuarterName;
            quarter.Start = Start;
            quarter.End = End;
            if (FYearId > 0 && QuarterName != null && Start != null && End != null)
            {
                db.Quarters.Add(quarter);
                db.SaveChanges();
                return RedirectToAction("AddQuarter");
            }

            return View(quarter);
        }

        //Edit Quarter
        public ActionResult EditQuarter(int id = 0)
        {
            Quarter quarter = db.Quarters.Find(id);
            var FYear = quarter.FYearId;

            if (quarter == null)
            {
                return HttpNotFound();
            }

            return View(quarter);
        }


        //Post Edit Quarter
        [HttpPost]
        public ActionResult EditQuarter(Quarter quarter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quarter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AddQuarter");
            }

            return View(quarter);
        }
        //=========================== End of Quarter ==============================


        //========================== Month =======================================

        //List Quarters
        public ActionResult ListMonth()
        {
            return PartialView("_ListMonth", db.Months.Include(x => x.Quarter).ToList());
        }



        //Create Quarter
        public ActionResult AddMonth()
        {

            var FYear = db.FYears.Where(x => x.Status == "Active").FirstOrDefault();
            if (FYear != null)
            {
                var quarter = db.Quarters.Where(x => x.FYearId == FYear.FYearId).Where(x => x.Status == "Active").FirstOrDefault();
                if (quarter != null)
                {
                    ViewBag.QId = quarter.QId;
                    ViewBag.QuarterName = quarter.QuarterName;
                }

            }
            return View();
        }

        //post Quarter
        [HttpPost]
        public ActionResult AddMonth(int QId, DateTime MonthName)
        {
            Month month = new Month();
            month.MonthName = MonthName;
            month.QId = QId;

            if (QId > 0 && MonthName != null)
            {
                db.Months.Add(month);
                db.SaveChanges();
                return RedirectToAction("AddMonth");
            }

            return View(month);
        }

        //Edit Quarter
        public ActionResult EditMonth(int id = 0)
        {
            Month month = db.Months.Find(id);
            var Month = month.QId;

            if (month == null)
            {
                return HttpNotFound();
            }

            return View(month);
        }


        //Post Edit Quarter
        [HttpPost]
        public ActionResult EditMonth(Month month)
        {
            if (ModelState.IsValid)
            {
                db.Entry(month).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AddMonth");
            }

            return View(month);
        }



        //===================== End of Month =======================================

        //========================== Week =======================================

        //List Quarters
        public ActionResult ListWeek()
        {
            return PartialView("_ListWeek", db.Weeks.Include(x => x.Month).ToList());
        }



        //Create Quarter
        public ActionResult AddWeek()
        {

            var FYear = db.FYears.Where(x => x.Status == "Active").FirstOrDefault();
            if (FYear != null)
            {
                var quarter = db.Quarters.Where(x => x.FYearId == FYear.FYearId).Where(x => x.Status == "Active").FirstOrDefault();
                if (quarter != null)
                {
                    var month = db.Months.Where(x => x.QId == quarter.QId).Where(x => x.Status == "Active").FirstOrDefault();
                    ViewBag.MonthId = month.MonthId;
                    ViewBag.MonthName = month.MonthName.ToString("MMM/yyyy");
                }

            }
            return View();
        }

        //post Quarter
        [HttpPost]
        public ActionResult AddWeek(int MonthId, string WeekName)
        {
            Week week = new Week();
            week.MonthId = MonthId;
            week.WeekName = WeekName;

            if (MonthId > 0 && !(String.IsNullOrEmpty(WeekName)))
            {
                db.Weeks.Add(week);
                db.SaveChanges();
                return RedirectToAction("AddWeek");
            }

            return View(week);
        }

        //Edit Quarter
        public ActionResult EditWeek(int id = 0)
        {
            Month month = db.Months.Find(id);
            var Month = month.QId;

            if (month == null)
            {
                return HttpNotFound();
            }

            return View(month);
        }


        //Post Edit Quarter
        [HttpPost]
        public ActionResult EditWeek(Month month)
        {
            if (ModelState.IsValid)
            {
                db.Entry(month).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AddMonth");
            }

            return View(month);
        }



        //===================== End of Week =======================================







        //List Financial Years
        public ActionResult ListFYear()
        {
            return PartialView("_ListFYear", db.FYears.ToList());
        }



        //Create Quarter
        public ActionResult AddFYear()
        {
            return View();
        }

        //post Quarter
        [HttpPost]
        public ActionResult AddFYear(FYear fyear)
        {
            if (ModelState.IsValid)
            {
                db.FYears.Add(fyear);
                db.SaveChanges();
                return RedirectToAction("AddFYear");
            }

            return View(fyear);
        }

        //Edit Quarter
        public ActionResult EditFYear(int id = 0)
        {
            FYear fyear = db.FYears.Find(id);
            if (fyear == null)
            {
                return HttpNotFound();
            }

            return View(fyear);
        }


        //Post Edit Quarter
        [HttpPost]
        public ActionResult EditFYear(FYear fyear)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fyear).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AddFYear");
            }

            return View(fyear);
        }


        //
        // GET: /Setup/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }









        //
        // POST: /Setup/Edit/5

        [HttpPost]
        public ActionResult Edit(Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(department);
        }

        //
        // GET: /Setup/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // POST: /Setup/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        //Add role to user
        [Access(Roles = "admin")]
        public ActionResult RoleAddToUser(int id)
        {
            var UserObj = db.UserProfiles.Find(id);
            ViewBag.UserName = UserObj.UserName;
            string[] UserExctRole = Roles.GetRolesForUser(UserObj.UserName);
            ViewBag.OldRoleName = UserExctRole[0];

            
            return View();
        }

        [Access(Roles = "admin")]
        [HttpPost]
        public ActionResult RoleAddToUser(string RoleName, string UserName, string OldRoleName)
        {
           
            if (Roles.IsUserInRole(UserName, RoleName))
            {
                ViewBag.ResultMessage = "This user already has the role specified !";
                return RedirectToAction("ListUser", "Admin");
                
            }
            else
            {
                Roles.RemoveUserFromRole(UserName, OldRoleName);
                Roles.AddUserToRole(UserName, RoleName);
            }
            return RedirectToAction("Index", "Admin");
        }

        //================ User Profile =========================
        public ActionResult UserProfile(int id)
        {

            var data = db.UserProfiles.Include(x => x.Department).Where(x => x.UserId == id).FirstOrDefault();
            ViewBag.Status = data.Status;
            return View(data);
        }

         //================ User Deactivate =========================
        public ActionResult Deactivate(int id)
        {

            var data = db.UserProfiles.Include(x => x.Department).Where(x => x.UserId == id).FirstOrDefault();
            
            return View(data);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Deactivate(int UserId, string DeactvateReason)
        {
            var data1 = db.UserProfiles.Find(UserId);
            if (String.IsNullOrEmpty(DeactvateReason))
            {
                var data = db.UserProfiles.Include(x => x.Department).Where(x => x.UserId == UserId).FirstOrDefault();
                ViewBag.Message = "You must add Reason";
               
                return View(data);

            }
            else
            {
                data1.Status = 1;
                data1.DeactivateReason = DeactvateReason;
                db.Entry(data1).State = EntityState.Modified;
                db.SaveChanges();
            
            }
            return RedirectToAction("ListUser", "Admin");
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Activate(int UserId)
        {
            var data1 = db.UserProfiles.Find(UserId);
 
                data1.Status = 0;
                data1.DeactivateReason = "";
                db.Entry(data1).State = EntityState.Modified;
                db.SaveChanges();

            
            return RedirectToAction("ListUser", "Admin");
        }

    }
}