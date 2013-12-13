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
    public class ReservatieController : Controller
    {
        
        private MyDbContext db;
        private UserManager<MyUser> manager;
        public ReservatieController()
        {
            db = new MyDbContext();
            manager = new UserManager<MyUser>(new UserStore<MyUser>(db));
        }
        
        // GET: /Reservatie/
        [Authorize]
        public async Task<ActionResult> Index(string sortOrder)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
            var reservatie = from m in db.Reservatie
                                 select m;
            ViewBag.DatumSortParm = String.IsNullOrEmpty(sortOrder) ? "Datum_desc" : "";
            ViewBag.ExamenNaamSortParm = sortOrder == "ExamenNaam" ? "ExamenNaam_desc" : "ExamenNaam";
            ViewBag.CampusSortParm = sortOrder == "Campus" ? "Campus_desc" : "Campus";

            switch (sortOrder)
            {
                case "Datum_desc":
                    reservatie = reservatie.OrderByDescending(s => s.Toezichtbeurt.BeginDatum);
                    break;
                case "Campus":
                    reservatie = reservatie.OrderBy(s => s.Toezichtbeurt.Campus.Name);
                    break;
                case "Campus_desc":
                    reservatie = reservatie.OrderByDescending(s => s.Toezichtbeurt.Campus.Name);
                    break;
                case "ExamenNaam":
                    reservatie = reservatie.OrderBy(s => s.Toezichtbeurt.ExamenNaam);
                    break;
                case "ExamenNaam_desc":
                    reservatie = reservatie.OrderByDescending(s => s.Toezichtbeurt.ExamenNaam);
                    break;
                default:                    
                    if (!User.IsInRole("Admin"))
                    {
                        reservatie = reservatie.OrderBy(s => s.Toezichtbeurt.BeginDatum);
                    }
                    else
                    {
                        reservatie = reservatie.OrderBy(s => s.Toezichtbeurt.ExamenNaam);
                    }
                    
                    break;
            }
            if (!User.IsInRole("Admin"))
            {
            reservatie = reservatie.Where(x => x.UserName == currentUser.UserName);
            }
            foreach (var re in reservatie)
            {
                re.Toezichtbeurt.Duurtijd = re.Toezichtbeurt.EindDatum.Subtract(re.Toezichtbeurt.BeginDatum).ToString();
            }
            
            return View(reservatie.ToList());
        }
        // GET: /Reservatie/All
        //[Authorize(Roles = "Admin")]
        //public async Task<ActionResult> All()
        //{
        //    return View(await db.Reservatie.ToListAsync());
        //}
        [Authorize]
        public async Task<ActionResult> ReservatieToevoegen(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var currentUser =  await manager.FindByIdAsync(User.Identity.GetUserId()); 
            var Monitorbeurt = db.MonitorBeurt.Single(monitor => monitor.MonitorBeurtId == id);
            
            foreach(ReservatieModel rest in db.Reservatie)
            {
               if(rest.ToezichtbeurtId == (int)id && rest.UserName == currentUser.UserName) 
               {
                   return RedirectToAction("index");
               }
            }            
            
            ReservatieModel res = new ReservatieModel();
            res.ToezichtbeurtId = (int)id;
            
            res.UserName =  currentUser.UserName; 
            res.AangepastOp = System.DateTime.Now;
            res.AangemaaktOp = System.DateTime.Now;

            if (ModelState.IsValid)
            {
                if(Monitorbeurt.Gereserveerd < Monitorbeurt.Capaciteit)
                {
                    Monitorbeurt.Gereserveerd += 1;
                    db.Entry(Monitorbeurt).State = EntityState.Modified;
                    db.Reservatie.Add(res);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ViewBag.ToezichtbeurtId = new SelectList(db.MonitorBeurt, "MonitorBeurtId", "Start", res.ToezichtbeurtId);
            
            return RedirectToAction("index");           
            
        }

        // GET: /Reservatie/Details/5
        [Authorize]
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
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.ToezichtbeurtId = new SelectList(db.MonitorBeurt, "MonitorBeurtId", "Start");
            return View();
        }

        // POST: /Reservatie/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include="ReservatieId,ToezichtbeurtId,UserName,AangemaaktOp,AangepastOp")] ReservatieModel reservatiemodel)
        {
            if (ModelState.IsValid)
            {
                db.Reservatie.Add(reservatiemodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ExamenNaam = new SelectList(db.MonitorBeurt, "ExamenNaam", "Examen", reservatiemodel.Toezichtbeurt.ExamenNaam);
            return View(reservatiemodel);
        }

        // GET: /Reservatie/Edit/5
        [Authorize(Roles = "Admin")]
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include="ReservatieId,ToezichtbeurtId,UserName,AangemaaktOp,AangepastOp")] ReservatieModel reservatiemodel)
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
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReservatieModel reservatiemodel = db.Reservatie.Find(id);
            if (reservatiemodel == null)
            {
                return HttpNotFound();
            }
            else if(!reservatiemodel.UserName.Equals(currentUser.UserName) && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(reservatiemodel);
        }

        // POST: /Reservatie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            ReservatieModel reservatiemodel = db.Reservatie.Find(id);
            
            var Monitorbeurt = db.MonitorBeurt.Single(monitor => monitor.MonitorBeurtId == reservatiemodel.ToezichtbeurtId);
            
            if (Monitorbeurt.Gereserveerd > 0)
            {
               Monitorbeurt.Gereserveerd -= 1;
               db.Entry(Monitorbeurt).State = EntityState.Modified;
                    
            }
            
            db.Reservatie.Remove(reservatiemodel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
