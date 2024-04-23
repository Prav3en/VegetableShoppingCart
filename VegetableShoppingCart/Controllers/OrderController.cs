using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VegetableShoppingCart.Models;

namespace VegetableShoppingCart.Controllers
{
    public class OrderController : Controller
    {
        VegetableEntities db = new VegetableEntities();
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrdersList()
        {
            if (Session["IsLoggedIn"] == null || (bool)Session["IsLoggedIn"] == false)

            {

                return HttpNotFound();

            }
            return View(db.Orders.OrderByDescending(o => o.OrderID).ToList());
        }
    }
}