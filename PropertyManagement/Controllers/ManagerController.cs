using PropertyManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PropertyManagement.Controllers
{
    public class ManagerController : Controller
    {
        private RentalManagemetEntities1 db = new RentalManagemetEntities1();
        // GET: Manager

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel manager)
        {
            Boolean userExist = db.Managers.Any(x => x.Email == manager.Email && x.Password == manager.Password);
            Manager m = db.Managers.FirstOrDefault(x => x.Email == manager.Email && x.Password == manager.Password);
            if (userExist)
            {
                FormsAuthentication.SetAuthCookie(m.Manager_Id, false);
                Session["ManagerGmail"] = m.Email;
                Session["ManagerId"] = m.Manager_Id;


                return RedirectToAction("Index", "Manager");
            }
            ModelState.AddModelError("","UserName or Password is wrong");

            return View();

        }
        public ActionResult Logout()
        {
            Session["ManagerGmail"] = null;
            Session["ManagerId"] = null;

            FormsAuthentication.SignOut();
            return RedirectToAction("Login");

        }

        
        public ActionResult Index()
        {
            if (Session["ManagerGmail"] == null)
            {
                return RedirectToAction("Login","Manager");
            }
            else
            {
                return View();
            }
          
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

        // GET: Buildings/Create
        public ActionResult BuildingCreate()
        {
            ViewBag.Building_Id = new SelectList(db.Appartments, "Appartment_Id", "AppartmentNumber");
            ViewBag.Building_Id = new SelectList(db.Events, "Event_Id", "Building_Id");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BuildingCreate([Bind(Include = "Building_Id,Address,City,PostalCode")] Building building)
        {

            Boolean buildingIdExist = db.Buildings.Any(x => x.Building_Id == building.Building_Id);
            if (!buildingIdExist)
            {
                if (ModelState.IsValid)
                {
                    db.Buildings.Add(building);
                    db.SaveChanges();
                    TempData["Success"] = "New Building Added";
                    return RedirectToAction("Building");
                }
            }
            ModelState.AddModelError("", "Building Id Exist Enter New Id.");
            return View(building);
        }

        
        public ActionResult BuildingEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Building building = db.Buildings.Find(id);
            if (building == null)
            {
                return HttpNotFound();
            }
            ViewBag.Building_Id = new SelectList(db.Appartments, "Appartment_Id", "AppartmentNumber", building.Building_Id);
            ViewBag.Building_Id = new SelectList(db.Events, "Event_Id", "Building_Id", building.Building_Id);
            return View(building);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BuildingEdit([Bind(Include = "Building_Id,Address,City,PostalCode")] Building building)
        {
            if (ModelState.IsValid)
            {
                db.Entry(building).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Building");
            }
            ViewBag.Building_Id = new SelectList(db.Appartments, "Appartment_Id", "AppartmentNumber", building.Building_Id);
            ViewBag.Building_Id = new SelectList(db.Events, "Event_Id", "Building_Id", building.Building_Id);
            return View(building);
        }

        // GET: Buildings/Delete/5
        public ActionResult BuildingDelete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Building building = db.Buildings.Find(id);
            if (building == null)
            {
                return HttpNotFound();
            }
            return View(building);
        }

        // POST: Buildings/Delete/5
        [HttpPost, ActionName("BuildingDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult BuildingDeleteConfirmed(string id)
        {
            Building building = db.Buildings.Find(id);
            db.Buildings.Remove(building);
            TempData["Error"] = "Building Deleted";
            db.SaveChanges();
            return RedirectToAction("Building");
        }


        //--------------------Appartment---------------------------------------------//  

        
        public ActionResult Appartment()
        {
            var appartments = db.Appartments.Include(a => a.Tenant);
            return View(appartments.ToList());
        }

      
        public ActionResult AppartmentCreate()
        {
            ViewBag.Building_Id = new SelectList(db.Buildings, "Building_Id", "Address");
            ViewBag.Tenant_Id = new SelectList(db.Tenants, "Tenant_Id", "FirstName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AppartmentCreate([Bind(Include = "Appartment_Id,AppartmentNumber,Size,Building_Id,Status,Rent,Tenant_Id")] Appartment appartment)
        {
            if (ModelState.IsValid)
            {
                db.Appartments.Add(appartment);
                db.SaveChanges();
                return RedirectToAction("Appartment");
            }

            ViewBag.Building_Id = new SelectList(db.Buildings, "Building_Id", "Address", appartment.Building_Id);
            ViewBag.Tenant_Id = new SelectList(db.Tenants, "Tenant_Id", "FirstName", appartment.Tenant_Id);
            return View(appartment);
        }

        
        public ActionResult AppartmentEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appartment appartment = db.Appartments.Find(id);
            if (appartment == null)
            {
                return HttpNotFound();
            }
            ViewBag.Building_Id = new SelectList(db.Buildings, "Building_Id", "Address", appartment.Building_Id);
            ViewBag.Tenant_Id = new SelectList(db.Tenants, "Tenant_Id", "FirstName", appartment.Tenant_Id);
            return View(appartment);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AppartmentEdit([Bind(Include = "Appartment_Id,AppartmentNumber,Size,Building_Id,Status,Rent,Tenant_Id")] Appartment appartment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appartment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Appartment");
            }
            ViewBag.Building_Id = new SelectList(db.Buildings, "Building_Id", "Address", appartment.Building_Id);
            ViewBag.Tenant_Id = new SelectList(db.Tenants, "Tenant_Id", "FirstName", appartment.Tenant_Id);
            return View(appartment);
        }


        public ActionResult AppartmentDelete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appartment appartment = db.Appartments.Find(id);
            if (appartment == null)
            {
                return HttpNotFound();
            }
            return View(appartment);
        }


        [HttpPost, ActionName("AppartmentDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult AppartmentDeleteConfirmed(string id)
        {
            Appartment appartment = db.Appartments.Find(id);
            db.Appartments.Remove(appartment);
            db.SaveChanges();
            return RedirectToAction("Appartment");
        }


        //-----------------------Msg-------------------------------------------------//
   
        public ActionResult Message()
        {
            String  Userid = Session["ManagerId"].ToString();
            var messages = db.Messages.Where(m => m.Manager.Manager_Id == Userid).Include(m => m.Tenant);
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
            
            ViewBag.Tenant_Id = new SelectList(db.Tenants, "Tenant_Id", "FirstName");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MessageCreate([Bind(Include = "Message_Id,,Tenant_Id,Message1")] Message message)
        {
            if (ModelState.IsValid)
            {   
                message.Manager_Id = Session["ManagerId"].ToString();
                message.Datetime = DateTime.Now;
                db.Messages.Add(message);
                db.SaveChanges();
                return RedirectToAction("Message");
            }

            ViewBag.Manager_Id = new SelectList(db.Managers, "Manager_Id", "FirstName", message.Manager_Id);
            ViewBag.Tenant_Id = new SelectList(db.Tenants, "Tenant_Id", "FirstName", message.Tenant_Id);
            return View(message);
        }
        public ActionResult MessageReply(String ManagerId, String TenantId)
        {

            ViewBag.Manager_Id = ManagerId;
            ViewBag.Tenant_Id = TenantId;
            return View();
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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
        public ActionResult DeleteConfirmed(int id)
        {
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
            db.SaveChanges();
            return RedirectToAction("Message");
        }
        //-----------------------Events-------------------------------------------------//


        public ActionResult Event()
        {
            String Userid = Session["ManagerId"].ToString();
            var events = db.Events.Where(e => e.Manager1.Manager_Id == Userid).Include(e => e.Building1);
            return View(events.ToList());
        }

        
        public ActionResult EventDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        
        public ActionResult EventCreate()
        {
            ViewBag.Building_Id = new SelectList(db.Buildings, "Building_Id", "Address");
         
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EventCreate([Bind(Include = "Event_Id,Building_Id,Manager_Id,Message,")] Event @event)
        {
            if (ModelState.IsValid)
            {   @event.Manager_Id = Session["ManagerId"].ToString();
                @event.Date= DateTime.Now;
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Event");
            }
s
            ViewBag.Building_Id = new SelectList(db.Buildings, "Building_Id", "Address", @event.Building_Id);
            ViewBag.Manager_Id = new SelectList(db.Managers, "Manager_Id", "FirstName", @event.Manager_Id);
            return View(@event);
        }
        
        public ActionResult EventDelete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

   
        [HttpPost, ActionName("EventDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("Event");
        }
        //-----------------------Events-------------------------------------------------//
        public ActionResult Appointment()
        {
            String User = Session["ManagerId"].ToString();
            var appointments = db.Appointments.Where(a => a.Manager.Manager_Id == User).Include(a => a.Tenant);
            return View(appointments.ToList());
        }

        // GET: Appointments/Create
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
            {   appointment.Manager_Id = Session["ManagerId"].ToString();
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
                appointment.Manager_Id = Session["ManagerId"].ToString();
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