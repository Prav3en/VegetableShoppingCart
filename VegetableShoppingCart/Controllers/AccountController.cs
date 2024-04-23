using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VegetableShoppingCart.Models;

namespace VegetableShoppingCart.Controllers
{
    public class AccountController : Controller
    {
        VegetableEntities db = new VegetableEntities();
        // GET: Account
        public ActionResult Index()
        {

            if (Session["IsLoggedIn"] == null || (bool)Session["IsLoggedIn"] == false)

            {

                return HttpNotFound();

            }

            return View(db.UserRegisters.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRegister userRegister = db.UserRegisters.Find(id);
            if (userRegister == null)
            {
                return HttpNotFound();
            }
            return View(userRegister);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRegister userRegister = db.UserRegisters.Find(id);
            if (userRegister == null)
            {
                return HttpNotFound();
            }
            return View(userRegister);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,UserName,Email,Password")] UserRegister userRegister)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userRegister).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userRegister);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRegister userRegister = db.UserRegisters.Find(id);
            if (userRegister == null)
            {
                return HttpNotFound();
            }
            return View(userRegister);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserRegister userRegister = db.UserRegisters.Find(id);
            db.UserRegisters.Remove(userRegister);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(UserRegister ur)
        {
            if (ModelState.IsValid)
            {
                if (db.UserRegisters.Any(x => x.Email == ur.Email))
                {
                    ViewBag.Message = "Email Already Registered";
                }
                else
                {
                    db.UserRegisters.Add(ur);
                    db.SaveChanges();
                    Response.Write("<script>alert('Registration Successful')</script>");
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(MyLogin l)
        {


            var user = db.UserRegisters.SingleOrDefault(m => m.Email == l.Email && m.Password == l.Password);

            var admin = db.AdminRegisters.SingleOrDefault(m => m.Email == l.Email && m.Password == l.Password);

            if (user != null)

            {

                Session["IsLoggedIn"] = true;

                Session["IsUser"] = true;

                Session["UserName"] = user.UserName;

                Session["UserId"] = user.UserId;
                

                return RedirectToAction("UserDisplay", "Products");

            }

            else if (admin != null)

            {

                Session["IsLoggedIn"] = true;

                Session["IsAdmin"] = true;

                Session["UserName"] = admin.AdminName;

               
                return RedirectToAction("Index", "Products");

            }

            else

            {

                ViewBag.ErrorMessage = "Invalid Credentials";

                return View();

            }
        }

        public ActionResult Logout()
        {
            Session["IsLoggedIn"] = false;
            Session["IsUser"] = false;
            Session["IsAdmin"] = false;

            return RedirectToAction("Index", "Home");
        }
    }
}