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

        public MonitorBeurtController()
        {
            db = new MyDbContext();
            UserManager = new UserManager<MyUser>(new UserStore<MyUser>(db));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }

        // GET: /MonitorBeurt/
        public ActionResult Index(string MonitorBeurtDepartement,string searchString)
        {
            var CampusLijst = new List<string>();

            var CampusQry = from d in db.Campus
                                 select d.Name;

            CampusLijst.AddRange(CampusQry.Distinct());
            ViewBag.MonitorBeurtDepartement = new SelectList(CampusLijst);

            var MonitorBeurten = from m in db.MonitorBeurt
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                MonitorBeurten = MonitorBeurten.Where(s => s.Campus.Name.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(MonitorBeurtDepartement))
            {
                 MonitorBeurten= MonitorBeurten.Where(x => x.Campus.Name == MonitorBeurtDepartement);
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            MonitorBeurtModel monitorbeurtmodel = db.MonitorBeurt.Find(id);

            var reservatie = from m in db.Reservatie
                             select m;
            reservatie = reservatie.Where(x => x.ToezichtbeurtId == monitorbeurtmodel.MonitorBeurtId);

            foreach (var res in reservatie)
            {
                db.Reservatie.Remove(res);
            }

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
