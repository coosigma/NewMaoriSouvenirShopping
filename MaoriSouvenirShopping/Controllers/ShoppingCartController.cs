using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MaoriSouvenirShopping.Data;
using MaoriSouvenirShopping.Models;
using Microsoft.AspNetCore.Authorization;


namespace MaoriSouvenirShopping.Controllers
{
    [AllowAnonymous]
    [Authorize(Roles = "Member")]
    public class ShoppingCartController : Controller
    {
        ApplicationDbContext _context;

        public ShoppingCartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            // Return the view
            return View(cart);
        }

        //
        // GET: /Store/AddToCart/5
        public ActionResult AddToCart(int id)
        {
            // Retrieve the album from the database
            var addedSouvenir = _context.Souvenirs
                .Single(souvenir => souvenir.SouvenirID == id);
            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext);
            cart.AddToCart(addedSouvenir, _context);
            // Go back to the main store page for more shopping
            return RedirectToAction("Index", "MemberSouvenirs");
        }

        public ActionResult RemoveFromCart(int id)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            int itemCount = cart.RemoveFromCart(id, _context);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}