using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Examonitor.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace Examonitor.Controllers
{
    public class MonitorBeurtController : Controller
    {
        public UserManager<MyUser> UserManager { get; private set; }
        public RoleManager<IdentityRole> RoleManager { get; private set; }
        public MyDbContext db { get; private set; }
        public string MonitorBeurtCampuss { get; set; }

        public MonitorBeurtController()
        {
            db = new MyDbContext();
            UserManager = new UserManager<MyUser>(new UserStore<MyUser>(db));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }
        [Authorize]
        // GET: /MonitorBeurt/
        public ActionResult Index(string MonitorBeurtCampus, string sortOrder)
        {
            
            var CampusLijst = new List<string>();

            var CampusQry = from d in db.Campus
                                 select d.Name;

            CampusLijst.AddRange(CampusQry.Distinct());
           
            ViewBag.MonitorBeurtCampus = new SelectList(CampusLijst);
            

            var MonitorBeurten = from m in db.MonitorBeurt
                         select m;
            ViewBag.DatumSortParm = String.IsNullOrEmpty(sortOrder) ? "Datum_desc" : "";
            ViewBag.CampusSortParm = sortOrder == "Campus" ? "Campus_desc" : "Campus";
            
            switch (sortOrder)
            {
                case "Datum_desc":
                    MonitorBeurten = MonitorBeurten.OrderByDescending(s => s.Datum);
                    break;
                case "Campus":
                    MonitorBeurten = MonitorBeurten.OrderBy(s => s.Campus.Name);
                    break;
                case "Campus_desc":
                    MonitorBeurten = MonitorBeurten.OrderByDescending(s => s.Campus.Name);
                    break;
                default:
                    MonitorBeurten = MonitorBeurten.OrderBy(s => s.Datum);
                    break;
            }


            if (!string.IsNullOrEmpty(MonitorBeurtCampus))
            {
                MonitorBeurtCampuss = MonitorBeurtCampus;
            }
            if (!string.IsNullOrEmpty(MonitorBeurtCampuss))
            {
                MonitorBeurten = MonitorBeurten.Where(x => x.Campus.Name == MonitorBeurtCampuss);
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create()
        {
            ViewBag.CampusId = new SelectList(await db.Campus.ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: /MonitorBeurt/Create
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MonitorBeurtModel monitorbeurtmodel, int CampusId)
        {
            if (ModelState.IsValid)
            {
                var CampusList = db.Campus.ToList().Where(campus => campus.Id == CampusId);
                monitorbeurtmodel.Campus = CampusList.First();
                db.MonitorBeurt.Add(monitorbeurtmodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(monitorbeurtmodel);
        }

        // GET: /MonitorBeurt/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            ViewBag.CampusId = new SelectList(await db.Campus.ToListAsync(), "Id", "Name");

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
        public ActionResult Edit(MonitorBeurtModel monitorbeurtmodel, int CampusId)
        {
            if (ModelState.IsValid)
            {
                var CampusList = db.Campus.ToList().Where(campus => campus.Id == CampusId);
                monitorbeurtmodel.Campus = CampusList.First();
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
