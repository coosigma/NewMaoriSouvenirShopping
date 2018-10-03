using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MaoriSouvenirShopping.Data;
using MaoriSouvenirShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
        //public ActionResult AddToCart(int id)
        public async Task<IActionResult> AddToCart(int? id, byte[] rowVersion)
        {
            // Retrieve the album from the database
            var addedSouvenir = await _context.Souvenirs
                .SingleOrDefaultAsync(souvenir => souvenir.SouvenirID == id);
            if (addedSouvenir == null)
            {
                Souvenir deletedSouvenir = new Souvenir();
                await TryUpdateModelAsync(deletedSouvenir);
                ModelState.AddModelError("cannotAddtoCart",
                    "Unable to add to cart. The souvenir was deleted by administrator.");
                ViewData["ConcurrencyErrorMessage"] = "Unable to add to cart. The souvenir was deleted by administrator.";
                return RedirectToAction("Index", "MemberSouvenirs");
            }
            _context.Entry(addedSouvenir).Property("RowVersion").OriginalValue = rowVersion;
            if (await TryUpdateModelAsync<Souvenir>(
                addedSouvenir, "",
                s => s.SouvenirName, s => s.Price, s => s.PhotoPath,
                s => s.PhotoPath, s => s.Description, s => s.CategoryID,
                s => s.SupplierID))
            {
                try
                {
                    // Go back to the main store page for more shopping                   
                    await _context.SaveChangesAsync();
                    // Add it to the shopping cart
                    var cart = ShoppingCart.GetCart(this.HttpContext);
                    cart.AddToCart(addedSouvenir, _context);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Souvenir)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("cannotAddtoCart",
                            "Unable to add to cart. The souvenir was deleted by administrator.");
                        ViewData["ConcurrencyErrorMessage"] = "Unable to add to cart. The souvenir was deleted by administrator.";
                    }
                    else
                    {
                        var databaseValues = (Souvenir)databaseEntry.ToObject();
                        ModelState.AddModelError("cannotAddtoCart", "The souvenir you attempted to add to cart "
                       + "was modified by administrator after you got the original value. The "
                       + "order operation was canceled and please check it again.");
                        ViewData["ConcurrencyErrorMessage"] = "The souvenir you attempted to add to cart "
                       + "was modified by administrator after you got the original value. The "
                       + "order operation was canceled and please check it again.";
                        addedSouvenir.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }
            return RedirectToAction("Index", "MemberSouvenirs");
        }

        public ActionResult RemoveFromCart(int id)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            int itemCount = cart.RemoveFromCart(id, _context);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public ActionResult EmptyCart()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            cart.EmptyCart(_context);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}