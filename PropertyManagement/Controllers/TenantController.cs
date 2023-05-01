using PropertyManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PropertyManagement.Controllers
{
    [Authorize]
    public class TenantController : Controller
    {  
       RentalManagemetEntities1 db = new RentalManagemetEntities1();
    
        public ActionResult Index()
        {
            
            return View(db.Buildings.ToList());
        }

        //--------------------Building---------------------------------------------//  

        public ActionResult Building()
        {
            var buildings = db.Buildings.Include(b => b.Event);
            return View(buildings.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Building(String search, String searchBy)
        {
            if (search == "")
            {
                return View(db.Buildings.ToList());
            }
            else
            {
                switch (searchBy)
                {
                    case "Address":
                        return View(db.Buildings.Where(x => x.Address.Contains(search)).ToList());

                    case "City":
                        return View(db.Buildings.Where(x => x.City.Contains(search)).ToList());

                    case "PostalCode":
                        return View(db.Buildings.Where(x => x.PostalCode.Contains(search)).ToList());
                }
            }
            return View(db.Buildings.ToList());
        }

        //--------------------Appartment---------------------------------------------//  


        //--------------------Appartment---------------------------------------------//  

        // GET: Appartments
        public ActionResult Appartment()
        {
            var appartments = db.Appartments.Include(a => a.Tenant);
            return View(appartments.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Appartment(String search, String searchBy)
        {
            if (search == "")
            {
                return View(db.Appartments.ToList());
            }
            else
            {
                switch (searchBy)
                {
                    case "Address":
                        return View(db.Appartments.Where(x => x.Building.Address.Contains(search)).ToList());

                    case "Size":
                        return View(db.Appartments.Where(x => x.Size.Contains(search)).ToList());

                    case "Rent":
                        int rent = Int32.Parse(search);
                        return View(db.Appartments.Where(x => x.Rent <= rent).ToList());
                }
            }
            return View(db.Appartments.ToList()); 
        }


        //-----------------------Msg-------------------------------------------------//

        public ActionResult Message()
        {
            String Userid = Session["TenantId"].ToString();
            var messages = db.Messages.Where(m => m.Tenant.Tenant_Id == Userid).Include(m => m.Manager);
            return View(messages.ToList());
        }

    
        public ActionResult MessageDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        public ActionResult MessageCreate()
        {
            ViewBag.Manager_Id = new SelectList(db.Managers, "Manager_Id", "FirstName");
        
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MessageCreate([Bind(Include = "Message_Id,Manager_Id,,Message1")] Message message)
        {
            if (ModelState.IsValid)
            {
                message.Tenant_Id = Session["TenantId"].ToString();
                message.Datetime = DateTime.Now;
                db.Messages.Add(message);
                db.SaveChanges();
                return RedirectToAction("Message");
            }

            ViewBag.Manager_Id = new SelectList(db.Managers, "Manager_Id", "FirstName", message.Manager_Id);
            ViewBag.Tenant_Id = new SelectList(db.Tenants, "Tenant_Id", "FirstName", message.Tenant_Id);
            return View();
        }


        public ActionResult MessageReply(String ManagerId,String TenantId)
        {

            ViewBag.Manager_Id = ManagerId;
            ViewBag.Tenant_Id = TenantId;
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MessageReply([Bind(Include = "Message_Id,Manager_Id,Tenant_Id,Message1")] Message message)
        {
            if (ModelState.IsValid)
            {
                message.Datetime = DateTime.Now;
                db.Messages.Add(message);
                db.SaveChanges();
                return RedirectToAction("Message");
            }
            ViewBag.Manager_Id = new SelectList(db.Managers, "Manager_Id", "FirstName", message.Manager_Id);
            ViewBag.Tenant_Id = new SelectList(db.Tenants, "Tenant_Id", "FirstName", message.Tenant_Id);
            return View(message);
        }

   
        public ActionResult MessageDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

       
        [HttpPost, ActionName("MessageDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult MessageDeleteConfirmed(int id)
        {
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
            db.SaveChanges();
            return RedirectToAction("Message");
        }

        //-----------------------Appointment-------------------------------------------------//

        // GET: Appointments
        public ActionResult Appointment()
        {   String User = Session["TenantId"].ToString();
            var appointments = db.Appointments.Where(a => a.Tenant.Tenant_Id == User ).Include(a => a.Manager);
            return View(appointments.ToList());
        }

    
        public ActionResult AppointmentCreate()
        {
            ViewBag.Manager_Id = new SelectList(db.Managers, "Manager_Id", "FirstName");
            ViewBag.Tenant_Id = new SelectList(db.Tenants, "Tenant_Id", "FirstName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AppointmentCreate([Bind(Include = "Appointment_Id,Manager_Id,Tenant_Id,Date,Conformation")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {   appointment.Tenant_Id = Session["TenantId"].ToString();
                appointment.Conformation = false;
                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("Appointment");
            }

            ViewBag.Manager_Id = new SelectList(db.Managers, "Manager_Id", "FirstName", appointment.Manager_Id);
            ViewBag.Tenant_Id = new SelectList(db.Tenants, "Tenant_Id", "FirstName", appointment.Tenant_Id);
            return View(appointment);
        }

        public ActionResult AppointmentEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            ViewBag.Manager_Id = new SelectList(db.Managers, "Manager_Id", "FirstName", appointment.Manager_Id);
            ViewBag.Tenant_Id = new SelectList(db.Tenants, "Tenant_Id", "FirstName", appointment.Tenant_Id);
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AppointmentEdit([Bind(Include = "Appointment_Id,Manager_Id,Tenant_Id,Date,Conformation")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.Tenant_Id = Session["TenantId"].ToString();
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Appointment");
            }
            ViewBag.Manager_Id = new SelectList(db.Managers, "Manager_Id", "FirstName", appointment.Manager_Id);
            ViewBag.Tenant_Id = new SelectList(db.Tenants, "Tenant_Id", "FirstName", appointment.Tenant_Id);
            return View(appointment);
        }

        public ActionResult AppointmentDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        [HttpPost, ActionName("AppointmentDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult AppointmentDeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
            db.SaveChanges();
            return RedirectToAction("Appointment");
        }
    }
}