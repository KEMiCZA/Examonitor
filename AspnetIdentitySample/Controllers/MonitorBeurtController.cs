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
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Data.Entity.Validation;
using System.Diagnostics;
using Newtonsoft.Json;
using Daishi.JsonParser;
using Examonitor.Utility;

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
        public async Task<ActionResult> Index(string MonitorBeurtCampus, string sortOrder, string currentFilter)
        {
   
            if (MonitorBeurtCampus == null)
                MonitorBeurtCampus = currentFilter;
           ViewBag.CurrentFilter = MonitorBeurtCampus;
            
            
            var currentUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var reservatie = from m in db.Reservatie
                             select m;
            //if (!User.IsInRole("Admin"))
         //   {
                reservatie = reservatie.Where(x => x.UserName == currentUser.UserName);
         //   }

            var reservatiesUser = reservatie.ToList();


            var MonitorBeurten = from m in db.MonitorBeurt
                         select m;
            ViewBag.DatumSortParm = String.IsNullOrEmpty(sortOrder) ? "Datum_desc" : "";
            ViewBag.ExamenNaamSortParm = sortOrder == "ExamenNaam" ? "ExamenNaam_desc" : "ExamenNaam";
            ViewBag.CampusSortParm = sortOrder == "Campus" ? "Campus_desc" : "Campus";
            
            switch (sortOrder)
            {
                case "Datum_desc":
                    MonitorBeurten = MonitorBeurten.OrderByDescending(s => s.BeginDatum);
                    break;
                case "Campus":
                    MonitorBeurten = MonitorBeurten.OrderBy(s => s.Campus.Name);
                    break;
                case "Campus_desc":
                    MonitorBeurten = MonitorBeurten.OrderByDescending(s => s.Campus.Name);
                    break;
                case "ExamenNaam":
                    MonitorBeurten = MonitorBeurten.OrderBy(s => s.ExamenNaam);
                    break;
                case "ExamenNaam_desc":
                    MonitorBeurten = MonitorBeurten.OrderByDescending(s => s.ExamenNaam);
                    break;
                default:
                    MonitorBeurten = MonitorBeurten.OrderBy(s => s.BeginDatum);
                    break;
            }
            
            if (!string.IsNullOrEmpty(MonitorBeurtCampus))
            {
                MonitorBeurten = MonitorBeurten.Where(x => x.Campus.Name.ToUpper().Contains(MonitorBeurtCampus.ToUpper()));
               
            }
            foreach (var mb in MonitorBeurten)
            {
                mb.ReservedByCurrentUser = false;
                mb.Available = true;
                if (mb.Capaciteit == mb.Gereserveerd)
                    mb.Available = false;
                mb.Duurtijd = mb.EindDatum.Subtract(mb.BeginDatum).ToString();
                foreach (var rt in reservatiesUser)
                {
                    if (mb.MonitorBeurtId == rt.Toezichtbeurt.MonitorBeurtId)
                    { 
                        mb.ReservedByCurrentUser = true;
                        mb.CurrentRegistratieID = rt.ReservatieId;
                    }
                }
            }
            return View(MonitorBeurten);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Import(HttpPostedFileBase file)
        {
            var campusLijst = new List<Campus>();
            IQueryable<Campus> campusQuery = null;
            if(ModelState.IsValid && file != null)
            {
                var monitorbeurtenParsed = new MonitorbeurtParser(file.InputStream, @"monitorbeurt");
                monitorbeurtenParsed.Parse();
                foreach(var monitorbeurt in monitorbeurtenParsed.Result)
                {
                    bool campusAlreadyExists = false; // We asume the campus does not exist yet

                    campusQuery = db.Campus.Where(n => n.Name.Equals(monitorbeurt.Campus.Name));
                    foreach(var campus in campusQuery) //First check if the campus exists in the database
                    {
                        if (campus.Name == monitorbeurt.Campus.Name) {
                            monitorbeurt.Campus = campus;
                            campusAlreadyExists = true; 
                        }
                    }

                    if(!campusAlreadyExists)
                    {
                        foreach(var campus in campusLijst) // if the campus is not in the db check if it exists in our current uploaded list
                        {
                            if (campus.Name == monitorbeurt.Campus.Name) {
                                monitorbeurt.Campus = campus;
                                campusAlreadyExists = true; 
                            }
                        }
                    }

                    if (db.MonitorBeurt.Where(n => n.ExamenNaam.Equals(monitorbeurt.ExamenNaam)).Count().Equals(0))
                        db.MonitorBeurt.Add(monitorbeurt);

                    if(!campusAlreadyExists) // if the campus does not exist in the database nor the uploaded list then add it to our current list
                        campusLijst.Add(monitorbeurt.Campus);
                }
            }
            file.InputStream.Close();

            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            return RedirectToAction("Index");
        }

        // GET: /MonitorBeurt/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var reservatie = from m in db.Reservatie
                             where m.ToezichtbeurtId == id
                                 select m;
            ReservatieModel res = new ReservatieModel();
            MonitorBeurtModel monitorbeurtmodel = db.MonitorBeurt.Find(id);
            if (monitorbeurtmodel == null)
            {
                return HttpNotFound();
            }
            var tuple = new Tuple<MonitorBeurtModel,ReservatieModel,IEnumerable<ReservatieModel>>(monitorbeurtmodel, res,reservatie);
            return View(tuple);
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
                monitorbeurtmodel.Duurtijd = "50";
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

        // GET: /MonitorBeurt/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteAll()
        {
            return View();
        }

        // POST: /MonitorBeurt/Delete/5
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteAllConfirmed(FormCollection formCollection)
        {
            var confirmation = formCollection["confirmation"];

            if (!string.IsNullOrEmpty(confirmation) && confirmation.Equals("DELETEALL"))
            {
                var reservatie = from m in db.Reservatie
                                 select m;

                foreach (var res in reservatie)
                {
                    db.Reservatie.Remove(res);
                }

                var monitorbeurt = from m in db.MonitorBeurt
                                   select m;

                foreach (var mon in monitorbeurt)
                {
                    db.MonitorBeurt.Remove(mon);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Invalid confirmation code.");
            }

            return View("DeleteAll");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
