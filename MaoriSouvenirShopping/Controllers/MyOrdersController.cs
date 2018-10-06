using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MaoriSouvenirShopping.Data;
using MaoriSouvenirShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MaoriSouvenirShopping.Controllers
{
    [Authorize(Roles = "Member")]
    public class MyOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyOrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MyOrders
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            string Id = user.Id;
            if (Id == null)
            {
                return NotFound();
            }
            var orders = _context.Orders
                .Include(o => o.User)
                 .AsNoTracking();
            orders = orders.Where(o => o.User.Id == Id);
            return View(orders);
        }

        // GET: MyOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(o => o.Souvenir)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}
