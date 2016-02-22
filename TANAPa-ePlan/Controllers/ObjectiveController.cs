using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TANAPa_ePlan.Models;

namespace TANAPa_ePlan.Controllers
{
    public class ObjectiveController : Controller
    {
        private ContextModel db = new ContextModel();

        //
        // GET: /Objective/

        public ActionResult Index()
        {
            var objectives = db.Objectives.Include(o => o.FYear);
            return View(objectives.ToList());
        }

        //
        // GET: /Objective/Details/5

        public ActionResult Details(int id = 0)
        {
            Objective objective = db.Objectives.Find(id);
            if (objective == null)
            {
                return HttpNotFound();
            }
            return View(objective);
        }

        //
        // GET: /Objective/Create

        public ActionResult Create()
        {
            var Fyear = db.FYears.Where(x => x.Status == "Active").FirstOrDefault();
            if(Fyear != null)
            {
                ViewBag.FYearId = Fyear.FYearId;
                ViewBag.FYear = Fyear.Year;
            }
            return View();
        }

        //
        // POST: /Objective/Create

        [HttpPost]
        public ActionResult Create(int FYearId, string OCode, string ODescription)
        {
            Objective objective = new Objective();
            objective.ODescription = ODescription;
            objective.FYearId = FYearId;
            objective.OCode = OCode;

            if (FYearId >0 && !(String.IsNullOrEmpty(OCode)) && !(String.IsNullOrEmpty(ODescription)))
            {
                db.Objectives.Add(objective);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var Fyear = db.FYears.Where(x => x.Status == "Active").FirstOrDefault();
            if (Fyear != null)
            {
                ViewBag.FYearId = Fyear.FYearId;
                ViewBag.FYear = Fyear.Year;
            }
            return View(objective);
        }

        //
        // GET: /Objective/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Objective objective = db.Objectives.Find(id);
            if (objective == null)
            {
                return HttpNotFound();
            }
            ViewBag.FId = new SelectList(db.FYears, "FYearId", "Year", objective.FYearId);
            return View(objective);
        }

        //
        // POST: /Objective/Edit/5

        [HttpPost]
        public ActionResult Edit(Objective objective)
        {
            if (ModelState.IsValid)
            {
                db.Entry(objective).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FId = new SelectList(db.FYears, "FYearId", "Year", objective.FYearId);
            return View(objective);
        }

        //
        // GET: /Objective/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Objective objective = db.Objectives.Find(id);
            if (objective == null)
            {
                return HttpNotFound();
            }
            return View(objective);
        }

        //
        // POST: /Objective/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Objective objective = db.Objectives.Find(id);
            db.Objectives.Remove(objective);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //====================== Add Target ===============================
        public ActionResult AddTarget()
        {
            ViewBag.DeptId = new SelectList(db.Departments, "DeptId", "DeptName");
            return View();
        }

        [HttpPost]
         public ActionResult AddTarget(int ObjectiveId, string TargetNo, string TDescription, int DeptId)
        {
            Target target = new Target();
            target.DeptId = DeptId;
            target.ObjectiveId = ObjectiveId;
            target.TargetNo = TargetNo;
            target.TDescription = TDescription;

            return View();
        }
    }
    }
