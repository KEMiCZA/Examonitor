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
            if (!User.IsInRole("Admin"))
            {
                reservatie = reservatie.Where(x => x.UserName == currentUser.UserName);
            }

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
            List<MonitorBeurtModel> monitorbeurten = new List<MonitorBeurtModel>();
            Campus campus = null;
            IQueryable<Campus> campusQuery = null;
            if (ModelState.IsValid)
            {
                if (file.ContentLength > 0)
                {
                    StreamReader reader = new StreamReader(file.InputStream);
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        string[] input = line.Split(';');
                        if(input.Length == 6)
                        {
                            String examenNaam = input[0];
                            DateTime beginDatum = DateTime.Parse(input[1]);
                            DateTime eindDatum = DateTime.Parse(input[2]);
                            int capaciteit = Convert.ToInt32(input[3]);
                            bool digitaal = input[4].Equals("y");
                            String campusNaam = input[5];
                            campusQuery = db.Campus.Where(n => n.Name.Equals(campusNaam));
                            if (campusQuery.Count().Equals(0)) // Campus does not exist, create it :)
                            {
                                campus = new Campus { Name = input[5] };
                                db.Campus.Add(campus);
                            }
                            else {
                                foreach(var x in campusQuery)
                                    campus = x;
                            }

                            monitorbeurten.Add(new MonitorBeurtModel
                            {
                                ExamenNaam = examenNaam,
                                BeginDatum = beginDatum,
                                EindDatum = eindDatum,
                                Capaciteit = capaciteit,
                                Digitaal = digitaal,
                                Campus = campus,
                                Gereserveerd = 0,
                            });
                        }
                        line = reader.ReadLine();
                    }
                    reader.Close();
                    foreach (MonitorBeurtModel monitorbeurt in monitorbeurten)
                    {
                        if (db.MonitorBeurt.Where(n => n.ExamenNaam.Equals(monitorbeurt.ExamenNaam)).Count().Equals(0))
                        {
                            db.MonitorBeurt.Add(monitorbeurt);
                        }
                    }
                    try
                    {
                        db.SaveChanges();
                    }
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
                    
                }
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
