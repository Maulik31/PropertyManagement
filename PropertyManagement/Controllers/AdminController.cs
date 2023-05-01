using PropertyManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PropertyManagement.Controllers
{
    public class AdminController : Controller
    {
        private RentalManagemetEntities1 db = new RentalManagemetEntities1();

        public ActionResult Login()
        { 
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel admin)
        {
            Boolean userExist = db.Admins.Any(x => x.Email == admin.Email && x.Password == admin.Password);
            Admin a = db.Admins.FirstOrDefault(x => x.Email == admin.Email && x.Password == admin.Password);
            if (userExist)
            {
                FormsAuthentication.SetAuthCookie(a.AdminId.ToString(), false);
                Session["AdminEmail"] = a.Email;

                return RedirectToAction("Index", "Admin");
            }
            ModelState.AddModelError("", "UserName or Password is wrong");
            return View();

        }
        public ActionResult Logout()
        {
            Session["AdminEmail"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");

        }

        // GET: Admin
        public ActionResult Index()
        {
            if (Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View(db.Buildings.ToList());
            }
        }




        //--------------------Manager---------------------------------------------//  

     public ActionResult Manager() {
            if (Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                return View(db.Managers.ToList());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manager(String search,String searchBy)
        {
            if (Session["AdminEmail"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                if (search == "")
                {
                    return View(db.Managers.ToList());
                }
                else
                {
                    switch (searchBy)
                    {
                        case "FirstName":
                            return View(db.Managers.Where(x => x.FirstName.Contains(search)).ToList());

                        case "LastName":
                            return View(db.Managers.Where(x => x.LastName.Contains(search)).ToList());

                        case "City":
                            return View(db.Managers.Where(x => x.City.Contains(search)).ToList());
                    }
                }
                return View(db.Managers.ToList());
            }
        }

        public ActionResult ManagerCreate()
        {
            ViewBag.Manager_Id = new SelectList(db.Appointments, "AppointmentI_d", "Tenant_Id");
            ViewBag.Building_Id = new SelectList(db.Buildings, "Building_Id", "Address");
            ViewBag.Manager_Id = new SelectList(db.Events, "Event_Id", "Building_Id");
            return View();
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManagerCreate([Bind(Include = "Manager_Id,FirstName,LastName,Email,Password,MoNumber,City,Building_Id")] Manager manager)
        {
            Boolean mangerIdExist = db.Managers.Any(x => x.Manager_Id == manager.Manager_Id);
            if (!mangerIdExist)
            {
                if (ModelState.IsValid)
                {
                    db.Managers.Add(manager);
                    db.SaveChanges();
                    TempData["Success"] = "New Manager Added";
                    return RedirectToAction("Manager");
                }
            }
            else
            {
                ModelState.AddModelError("", "Manager ID Exist enter new");
            }
           

            ViewBag.Manager_Id = new SelectList(db.Appointments, "AppointmentI_d", "Tenant_Id", manager.Manager_Id);
       
            ViewBag.Manager_Id = new SelectList(db.Events, "Event_Id", "Building_Id", manager.Manager_Id);
            return View(manager);
        }


        public ActionResult ManagerEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager manager = db.Managers.Find(id);
            if (manager == null)
            {
                return HttpNotFound();
            }
            ViewBag.Manager_Id = new SelectList(db.Appointments, "AppointmentI_d", "Tenant_Id", manager.Manager_Id);

            ViewBag.Manager_Id = new SelectList(db.Events, "Event_Id", "Building_Id", manager.Manager_Id);
            return View(manager);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManagerEdit([Bind(Include = "Manager_Id,FirstName,LastName,Email,Password,MoNumber,City,Building_Id")] Manager manager)
        {
            if (ModelState.IsValid)
            {
                db.Entry(manager).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Manager");
            }
            ViewBag.Manager_Id = new SelectList(db.Appointments, "AppointmentI_d", "Tenant_Id", manager.Manager_Id);
            ViewBag.Manager_Id = new SelectList(db.Events, "Event_Id", "Building_Id", manager.Manager_Id);
            return View(manager);
        }

        public ActionResult ManagerDelete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager manager = db.Managers.Find(id);
            if (manager == null)
            {
                return HttpNotFound();
            }
            return View(manager);
        }

     
        [HttpPost, ActionName("ManagerDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Manager manager = db.Managers.Find(id);
            db.Managers.Remove(manager);
            db.SaveChanges();
            TempData["Error"] = "Manager Deleted";
            return RedirectToAction("Manager");
        }

        //--------------------Tenant---------------------------------------------//  

       
        public ActionResult Tenant()
        {
            var tenants = db.Tenants.Include(t => t.Appointments);
            return View(tenants.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Tenant(String search, String searchBy)
        {
            if (search == "")
            {
                return View(db.Tenants.ToList());
            }
            else
            {
                switch (searchBy)
                {
                    case "FirstName":
                        return View(db.Tenants.Where(x => x.FirstName.Contains(search)).ToList());

                    case "LastName":
                        return View(db.Tenants.Where(x => x.LastName.Contains(search)).ToList());

                    case "City":
                        return View(db.Tenants.Where(x => x.City.Contains(search)).ToList());
                }
            }
            return View(db.Tenants.ToList());
        }

     
        public ActionResult TenantEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tenant tenant = db.Tenants.Find(id);
            if (tenant == null)
            {
                return HttpNotFound();
            }
            ViewBag.Tenant_Id = new SelectList(db.Appointments, "AppointmentI_d", "Tenant_Id", tenant.Tenant_Id);
            return View(tenant);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TenantEdit([Bind(Include = "Tenant_Id,FirstName,LastName,Email,Password,MoNumber,Address,City,Province")] Tenant tenant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tenant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Tenant");
            }
            ViewBag.Tenant_Id = new SelectList(db.Appointments, "AppointmentI_d", "Tenant_Id", tenant.Tenant_Id);
            return View(tenant);
        }


        public ActionResult TenantDelete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tenant tenant = db.Tenants.Find(id);
            if (tenant == null)
            {
                return HttpNotFound();
            }
            return View(tenant);
        }


        [HttpPost, ActionName("TenantDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult TenantDeleteConfirmed(string id)
        {
            Tenant tenant = db.Tenants.Find(id);
            db.Tenants.Remove(tenant);
            db.SaveChanges();
            TempData["Error"] = "Tenant Deleted";
            return RedirectToAction("Tenant");
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
          
            Boolean buildingIdExist = db.Buildings.Any(x=>x.Building_Id == building.Building_Id);
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


        //--------------------Events---------------------------------------------//  

        
        public ActionResult Event()
        {
            var events = db.Events.Include(e => e.Building1).Include(e => e.Manager1);
            return View(events.ToList());
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
        public ActionResult EventDeleteConfirmed(string id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("Event");
        }
    }
}
