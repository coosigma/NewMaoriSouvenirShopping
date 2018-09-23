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
using Microsoft.Extensions.DependencyInjection;

namespace MaoriSouvenirShopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminApplicationUsersController(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }
        public async Task<IEnumerable<ApplicationUser>> ReturnAllMembers()
        {
            var users = await _userManager.GetUsersInRoleAsync("Member");
            return users;
        }

        // GET: AdminApplicationUsers
        public IActionResult Index()
        {
            IEnumerable<ApplicationUser> members = ReturnAllMembers().Result;
            return View(members);
        }

    }
}
