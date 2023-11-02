using CmsShoppingCart.Infrastructure;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CmsShoppingCart.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly CmsShoppingCartContext context;

        public CheckoutController(CmsShoppingCartContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            CartViewModel cartVM = new()
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Quantity * x.Price)
            };

            OrderViewModel vm = new()
            {
                Order = new Order(),
                CartViewModel = cartVM
            };

            return View(vm);
        }
        [HttpPost]
        public IActionResult PlaceOrder(OrderViewModel orderViewModel)
        {
            if (ModelState.IsValid)
            {
                Order order = new Order
                {
                    Name = orderViewModel.Order.Name,
                    phoneNumber = orderViewModel.Order.phoneNumber,
                    Adress = orderViewModel.Order.Adress,
                    TotalAmount = orderViewModel.Order.TotalAmount,
                    OrderDate = DateTime.Now,
                    CartItems = orderViewModel.CartViewModel.CartItems,
                    Description = orderViewModel.Order.Description
                };

                context.Orders.Add(order);
                context.SaveChanges();
                TempData["Success"] = "The product has been added!";
                return RedirectToAction("Index");
            }

            return View("Index", orderViewModel);
        }
    }
}
