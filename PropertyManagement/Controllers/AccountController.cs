using PropertyManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PropertyManagement.Controllers
{
    public class AccountController : Controller
    {
        RentalManagemetEntities1 db = new RentalManagemetEntities1();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel Credentials)
        {
            Boolean userExist = db.Tenants.Any(x => x.Email == Credentials.Email && x.Password == Credentials.Password);
            Tenant t = db.Tenants.FirstOrDefault(x => x.Email == Credentials.Email && x.Password == Credentials.Password);
            if (userExist)
            {
                Session["TenantId"] = t.Tenant_Id;
                FormsAuthentication.SetAuthCookie(t.Tenant_Id, false);
                return RedirectToAction("Index", "Tenant");
            }
            ModelState.AddModelError("", "UserName or Password is wrong");

            return View();

        }
        [HttpPost]
        public ActionResult Signup(Tenant tenant)
        {
            Boolean userIdExist = db.Tenants.Any(x => x.Tenant_Id == tenant.Tenant_Id);
            if (!userIdExist)
            {
                db.Tenants.Add(tenant);
                db.SaveChanges();
                TempData["Success"] = "Register successfully";
                return RedirectToAction("Login");
            }
            ModelState.AddModelError("", "UserId Exist enter new ID");

            return View();
        }
        public ActionResult Logout()
        {

            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        
        }
     




    }
}