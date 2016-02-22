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
    public class TargetController : Controller
    {
        private ContextModel db = new ContextModel();

        //
        // GET: /Target/

        public ActionResult Index()
        {
            var targets = db.Targets.Include(t => t.Objective).Include(t => t.Department);
            return View(targets.ToList());
        }

        //
        // GET: /Target/Details/5

        public ActionResult Details(int id = 0)
        {
            Target target = db.Targets.Find(id);
            if (target == null)
            {
                return HttpNotFound();
            }
            return View(target);
        }

        //
        // GET: /Target/Create

        public ActionResult Create()
        {
            ViewBag.ObjectiveId = new SelectList(db.Objectives, "ObjectiveId", "ODescription");
            ViewBag.DeptId = new SelectList(db.Departments, "DeptId", "DeptName");
            return View();
        }

        //
        // POST: /Target/Create

        [HttpPost]
        public ActionResult Create(Target target)
        {
            if (ModelState.IsValid)
            {
                db.Targets.Add(target);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ObjectiveId = new SelectList(db.Objectives, "ObjectiveId", "ODescription", target.ObjectiveId);
            ViewBag.DeptId = new SelectList(db.Departments, "DeptId", "DeptName", target.DeptId);
            return View(target);
        }

        //
        // GET: /Target/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Target target = db.Targets.Find(id);
            if (target == null)
            {
                return HttpNotFound();
            }
            ViewBag.ObjectiveId = new SelectList(db.Objectives, "ObjectiveId", "ODescription", target.ObjectiveId);
            ViewBag.DeptId = new SelectList(db.Departments, "DeptId", "DeptName", target.DeptId);
            return View(target);
        }

        //
        // POST: /Target/Edit/5

        [HttpPost]
        public ActionResult Edit(Target target)
        {
            if (ModelState.IsValid)
            {
                db.Entry(target).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ObjectiveId = new SelectList(db.Objectives, "ObjectiveId", "ODescription", target.ObjectiveId);
            ViewBag.DeptId = new SelectList(db.Departments, "DeptId", "DeptName", target.DeptId);
            return View(target);
        }

        //
        // GET: /Target/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Target target = db.Targets.Find(id);
            if (target == null)
            {
                return HttpNotFound();
            }
            return View(target);
        }

        //
        // POST: /Target/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Target target = db.Targets.Find(id);
            db.Targets.Remove(target);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}