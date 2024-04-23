using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VegetableShoppingCart.Models;

namespace VegetableShoppingCart.Controllers
{
    public class ProductsController : Controller
    {
        private VegetableEntities db = new VegetableEntities();

        // GET: Products
        public ActionResult Index()
        {

            if (Session["IsLoggedIn"] == null || (bool)Session["IsLoggedIn"] == false)

            {

                return HttpNotFound();

            }

            return View(db.Products.ToList());
        }

        //Displaying Products for the User
        public ActionResult UserDisplay()
        {

            if (Session["IsLoggedIn"] == null || (bool)Session["IsLoggedIn"] == false)

            {

                return HttpNotFound();

            }

            return View(db.Products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        //Displaying Product Details for the User
        public ActionResult DetailDisplay(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,Name,Description,Price,ImageURL,File,Category,StockQuantity")] Product product)
        {

            if (ModelState.IsValid)
            {
                string filename = Path.GetFileName(product.File.FileName);
                string _filename = DateTime.Now.ToString("hhmmssfff") + filename;
                string path = Path.Combine(Server.MapPath("~/Images/"), _filename);
                product.ImageURL = "~/Images/" + _filename;

                db.Products.Add(product);
                if (db.SaveChanges() > 0)
                {
                    product.File.SaveAs(path);
                }
                //db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            Session["imgPath"] = product.ImageURL;
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,Name,Description,Price,ImageURL,File,Category,StockQuantity")] Product product)
        {


            if (ModelState.IsValid)
            {
                if (product.File != null)
                {
                    string filename = Path.GetFileName(product.File.FileName);
                    string _filename = DateTime.Now.ToString("hhmmssfff") + filename;
                    string path = Path.Combine(Server.MapPath("~/Images/"), _filename);
                    product.ImageURL = "~/Images/" + _filename;

                    db.Entry(product).State = EntityState.Modified;
                    //retrive the path of the old image from the session
                    string oldImgPath = Request.MapPath(Session["imgPath"].ToString());
                    if (db.SaveChanges() > 0)
                    {
                        product.File.SaveAs(path);
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    product.ImageURL = Session["imgPath"].ToString();

                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            string currentImg = Request.MapPath(product.ImageURL);
            db.Products.Remove(product);
            if (db.SaveChanges() > 0)
            {
                if (System.IO.File.Exists(currentImg))
                {
                    System.IO.File.Delete(currentImg);
                }
            }

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
