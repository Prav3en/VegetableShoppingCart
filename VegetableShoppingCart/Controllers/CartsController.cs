using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VegetableShoppingCart.Models;

namespace VegetableShoppingCart.Controllers
{
    public class CartsController : Controller
    {
        private VegetableEntities db = new VegetableEntities();
        // GET: Carts
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CartView()
        {
            List<Cart> cart = Session["Cart"] as List<Cart>;
            if (cart == null)
            {
                cart = new List<Cart>();
                Session["Cart"] = cart;
            }
            return View(cart);
        }

        [HttpPost]
        public ActionResult AddToCart(int productId, string productName, decimal price, int quantity = 1)
        {

            List<Cart> cart = Session["Cart"] as List<Cart> ?? new List<Cart>();

            Cart existingItem = cart.FirstOrDefault(item => item.ProductID == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new Cart { ProductID = productId, ProductName = productName, Price = price, Quantity = quantity });
            }

            Session["Cart"] = cart;

            return RedirectToAction("CartView");

        }

        public ActionResult RemoveFromCart(int productId)
        {
            List<Cart> cart = Session["Cart"] as List<Cart>;

            if (cart != null)
            {
                cart.RemoveAll(item => item.ProductID == productId);
                Session["Cart"] = cart;
            }
            return RedirectToAction("CartView");
        }

        public ActionResult ClearCart()
        {
            /*Session.Clear();*/
            Session.Remove("Cart");

            return Redirect("~/Products/UserDisplay");
        }

        //Payment Success Page for User
        public ActionResult PaymentSuccess()
        {
            List<Cart> cart = Session["Cart"] as List<Cart>;
            if (cart == null)
            {
                cart = new List<Cart>();
                Session["Cart"] = cart;
            }
            return View(cart);
            //return View();
        }


        private int GetCurrentUserId()
        {
            if (Session["UserId"] != null)
            {
                return (int)Session["UserId"];
            }
            else
            {
                return -1;
            }
            //return 1;
        }

        //To Clear the Cart
        private void ClearCart(int userId)
        {
            // Retrieve cart items for the current user from the database
            var cartItems = db.Carts.Where(c => c.UserID == userId).ToList();

            // Remove cart items from the database
            db.Carts.RemoveRange(cartItems);
            db.SaveChanges();
        }

        public ActionResult Checkout()
        {
            // Retrieve cart items from session
            List<Cart> cartItems = Session["Cart"] as List<Cart>;

            if (cartItems == null || cartItems.Count == 0)
            {
                // Redirect to an empty cart page or display a message
                return RedirectToAction("EmptyCart");
            }

            // Calculating total amount
            decimal totalAmount = cartItems.Sum(item => item.Price * item.Quantity);

            // Create a new order
            var order = new Order
            {
                UserId = GetCurrentUserId(),
                TotalAmount = totalAmount,
                OrderDate = DateTime.Now,
                OrderStatus = "Paid" // Set the initial order status
            };

            // Save the order to the database
            db.Orders.Add(order);
            db.SaveChanges();

            // Optionally, you can clear the cart session after creating the order
            Session["Cart"] = null;

            // Redirect to the order confirmation page with the order ID
            return RedirectToAction("PaymentSuccess", new { orderId = order.OrderID }); ;
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