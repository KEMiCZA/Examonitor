using Examonitor.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Examonitor.Controllers
{
    public class AdminMessageController : Controller
    {
        public MyDbContext db { get; private set; }

        public AdminMessageController()
        {
            db = new MyDbContext();
        }

        //
        // GET: /AdminMessage/
        public ActionResult Index()
        {
            return View();
        }

        public String GetMessage()
        {
            var message = from m in db.AdminMessage
                          select m;
            message = message.OrderBy(s => s.Id);
            var messageObject = message.Skip(0).FirstOrDefault();
            if (messageObject.Active)
                return messageObject.Message;
            else
                return "";
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            AdminMessageModel adminMessageModel = db.AdminMessage.Find(id);
            if (adminMessageModel == null)
            {
                return HttpNotFound();
            }
            return View(adminMessageModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(AdminMessageModel adminMessageModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adminMessageModel).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index", "MonitorBeurt");
        }

    }
}