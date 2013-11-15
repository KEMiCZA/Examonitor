﻿using System;
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
        [Authorize]
        // GET: /Reservatie/
        public async Task<ActionResult> Index()
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
            var reservatie = from m in db.Reservatie
                                 select m;
            reservatie = reservatie.Where(x => x.UserName == currentUser.UserName);
            
            return View(reservatie.ToList());
        }
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
                if(Monitorbeurt.Gereserveerd<Monitorbeurt.Capaciteit)
                {
                    Monitorbeurt.Gereserveerd += 1;
                    db.Entry(Monitorbeurt).State = EntityState.Modified;
                }                
                db.Reservatie.Add(res);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ToezichtbeurtId = new SelectList(db.MonitorBeurt, "MonitorBeurtId", "Start", res.ToezichtbeurtId);            
            return RedirectToAction("index");           
            
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ReservatieId,ToezichtbeurtId,UserName,AangemaaktOp,AangepastOp")] ReservatieModel reservatiemodel)
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
