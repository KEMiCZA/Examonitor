using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Examonitor.Models;

namespace Examonitor.Controllers
{
    public class MonitorBeurtController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: /MonitorBeurt/
        public ActionResult Index(string MonitorBeurtDepartement,string searchString)
        {
            var DepartementLijst = new List<string>();

            var DepartementQry = from d in db.MonitorBeurt
                           orderby d.Departement
                           select d.Departement;
            

            DepartementLijst.AddRange(DepartementQry.Distinct());
            ViewBag.MonitorBeurtDepartement = new SelectList(DepartementLijst);

            var MonitorBeurten = from m in db.MonitorBeurt
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                MonitorBeurten = MonitorBeurten.Where(s => s.Campus.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(MonitorBeurtDepartement))
            {
                 MonitorBeurten= MonitorBeurten.Where(x => x.Departement == MonitorBeurtDepartement);
            }
            return View(MonitorBeurten);
            
        }

        // GET: /MonitorBeurt/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MonitorBeurtModel monitorbeurtmodel = db.MonitorBeurt.Find(id);
            if (monitorbeurtmodel == null)
            {
                return HttpNotFound();
            }
            return View(monitorbeurtmodel);
        }
        public ActionResult Reserveren(int? id)
        {
            return View();

        }

        // GET: /MonitorBeurt/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /MonitorBeurt/Create
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MonitorBeurtModel monitorbeurtmodel)
        {
            if (ModelState.IsValid)
            {
                db.MonitorBeurt.Add(monitorbeurtmodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(monitorbeurtmodel);
        }

        // GET: /MonitorBeurt/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MonitorBeurtModel monitorbeurtmodel = db.MonitorBeurt.Find(id);
            if (monitorbeurtmodel == null)
            {
                return HttpNotFound();
            }
            return View(monitorbeurtmodel);
        }

        // POST: /MonitorBeurt/Edit/5
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MonitorBeurtModel monitorbeurtmodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(monitorbeurtmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(monitorbeurtmodel);
        }

        // GET: /MonitorBeurt/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MonitorBeurtModel monitorbeurtmodel = db.MonitorBeurt.Find(id);
            if (monitorbeurtmodel == null)
            {
                return HttpNotFound();
            }
            return View(monitorbeurtmodel);
        }

        // POST: /MonitorBeurt/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MonitorBeurtModel monitorbeurtmodel = db.MonitorBeurt.Find(id);
            db.MonitorBeurt.Remove(monitorbeurtmodel);
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
