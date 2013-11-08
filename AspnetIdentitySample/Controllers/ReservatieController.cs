using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Examonitor.Models;

namespace Examonitor.Controllers
{
    [Authorize]
    public class ReservatieController : Controller
    {
        private ExamenMonitorDbContext db = new ExamenMonitorDbContext();

        // GET: /Reservatie/
        public ActionResult Index()
        {
            var reservatie = db.Reservatie.Include(r => r.Toezichtbeurt);
            return View(reservatie.ToList());
        }
        [Authorize]
        public ActionResult ReservatieToevoegen(int id)
        {
            var Monitorbeurt = db.MonitorBeurt.Single(monitor => monitor.MonitorBeurtId == id);
            ReservatieModel res = new ReservatieModel();
            res.ToezichtbeurtId = id;
            res.UserId = User.Identity.GetUserId();
            res.AangepastOp = System.DateTime.Now;
            res.AangemaaktOp = System.DateTime.Now;
            
            if (ModelState.IsValid)
            {
                db.Reservatie.Add(res);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ToezichtbeurtId = new SelectList(db.MonitorBeurt, "MonitorBeurtId", "Start", res.ToezichtbeurtId);
            return View(res);
            //return RedirectToAction("index");

        }

        // GET: /Reservatie/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReservatieModel reservatiemodel = db.Reservatie.Find(id);
            if (reservatiemodel == null)
            {
                return HttpNotFound();
            }
            return View(reservatiemodel);
        }

        // GET: /Reservatie/Create
        public ActionResult Create()
        {
            ViewBag.ToezichtbeurtId = new SelectList(db.MonitorBeurt, "MonitorBeurtId", "Start");
            return View();
        }

        // POST: /Reservatie/Create
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReservatieModel reservatiemodel)
        {
            if (ModelState.IsValid)
            {
                db.Reservatie.Add(reservatiemodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ToezichtbeurtId = new SelectList(db.MonitorBeurt, "MonitorBeurtId", "Start", reservatiemodel.ToezichtbeurtId);
            return View(reservatiemodel);
        }

        // GET: /Reservatie/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReservatieModel reservatiemodel = db.Reservatie.Find(id);
            if (reservatiemodel == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToezichtbeurtId = new SelectList(db.MonitorBeurt, "MonitorBeurtId", "Start", reservatiemodel.ToezichtbeurtId);
            return View(reservatiemodel);
        }

        // POST: /Reservatie/Edit/5
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReservatieModel reservatiemodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservatiemodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ToezichtbeurtId = new SelectList(db.MonitorBeurt, "MonitorBeurtId", "Start", reservatiemodel.ToezichtbeurtId);
            return View(reservatiemodel);
        }

        // GET: /Reservatie/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReservatieModel reservatiemodel = db.Reservatie.Find(id);
            if (reservatiemodel == null)
            {
                return HttpNotFound();
            }
            return View(reservatiemodel);
        }

        // POST: /Reservatie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReservatieModel reservatiemodel = db.Reservatie.Find(id);
            db.Reservatie.Remove(reservatiemodel);
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
