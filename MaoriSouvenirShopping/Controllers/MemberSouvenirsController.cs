using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MaoriSouvenirShopping.Data;
using MaoriSouvenirShopping.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;


namespace MaoriSouvenirShopping.Controllers
{
    [AllowAnonymous]
    [Authorize(Roles = "Member")]
    public class MemberSouvenirsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemberSouvenirsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MemberSouvenirs
        public async Task<IActionResult> Index(
             string sortOrder,
             string currentFilter,
             string currentCategory,
             decimal? currentLowerPrice,
             decimal? currentUpperPrice,
             string searchString,
             string error,
             string rv,
             string category,
             decimal? lower_price,
             decimal? upper_price,
             int? page)
        {
            if (error != null)
            {
                if (error.Equals("deleted"))
                {
                    ModelState.AddModelError("cannotAddtoCart",
                    "Unable to add to cart. The souvenir was deleted by administrator.");
                }
                else if(error.Equals("updated"))
                {
                    ModelState.AddModelError("cannotAddtoCart", "The souvenir you attempted to add to cart "
                       + "was modified by administrator after you got the original value. The "
                       + "order operation(includes items in your cart) was canceled and please check it again.");
                }
            }
            ViewData["CurrentSort"] = sortOrder;
            ViewData["lowerPrice"] = lower_price;
            ViewData["upperPrice"] = upper_price;
            ViewData["CurrentCategory"] = category;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            if (searchString != null || category != null || lower_price != null || upper_price != null)
            {
                page = 1;
            }
            else
            {
                if (searchString == null)
                    searchString = currentFilter;
                if (category == null)
                    category = currentCategory;
                if (lower_price == null && upper_price == null)
                {
                    lower_price = currentLowerPrice;
                    upper_price = currentUpperPrice;
                }


            }
            ViewData["CurrentFilter"] = searchString;

            var souvenirs = _context.Souvenirs
                .Include(s => s.Category)
                .Include(s => s.Supplier)
                .AsNoTracking();

            if (category != null && category != "AllCategories")
            {
                souvenirs = souvenirs.Where(s => s.Category.CategoryName == category);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                souvenirs = souvenirs.Where(s => s.SouvenirName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    souvenirs = souvenirs.OrderByDescending(s => s.SouvenirName);
                    break;
                case "Price":
                    souvenirs = souvenirs.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    souvenirs = souvenirs.OrderByDescending(s => s.Price);
                    break;
                default:
                    souvenirs = souvenirs.OrderBy(s => s.SouvenirName);
                    break;
            }
            if (lower_price != null || upper_price != null)
            {
                if (lower_price != null && upper_price != null)
                {
                    souvenirs = souvenirs.Where(s => s.Price >= lower_price && s.Price <= upper_price);
                }
                else if (lower_price == null)
                {
                    souvenirs = souvenirs.Where(s => s.Price <= upper_price);
                }
                else
                {
                    souvenirs = souvenirs.Where(s => s.Price >= lower_price);
                }
            }

            int pageSize = 5;
            return View(await PaginatedList<Souvenir>.CreateAsync(souvenirs.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: MemberSouvenirs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var souvenir = await _context.Souvenirs
                .Include(s => s.Category)
                .Include(s => s.Supplier)
                //.Include(s => s.OrderItem)
                //.ThenInclude(e => e.Order)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.SouvenirID == id);
            if (souvenir == null)
            {
                return NotFound();
            }

            return View(souvenir);
        }

        //// GET: MemberSouvenirs/Create
        //public IActionResult Create()
        //{
        //    ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID");
        //    ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierID");
        //    return View();
        //}

        //// POST: MemberSouvenirs/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("SouvenirID,SouvenirName,Price,PhotoPath,Description,CategoryID,SupplierID")] Souvenir souvenir)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(souvenir);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", souvenir.CategoryID);
        //    ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierID", souvenir.SupplierID);
        //    return View(souvenir);
        //}

        //// GET: MemberSouvenirs/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var souvenir = await _context.Souvenirs.SingleOrDefaultAsync(m => m.SouvenirID == id);
        //    if (souvenir == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", souvenir.CategoryID);
        //    ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierID", souvenir.SupplierID);
        //    return View(souvenir);
        //}

        //// POST: MemberSouvenirs/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("SouvenirID,SouvenirName,Price,PhotoPath,Description,CategoryID,SupplierID")] Souvenir souvenir)
        //{
        //    if (id != souvenir.SouvenirID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(souvenir);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!SouvenirExists(souvenir.SouvenirID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", souvenir.CategoryID);
        //    ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierID", souvenir.SupplierID);
        //    return View(souvenir);
        //}

        //// GET: MemberSouvenirs/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var souvenir = await _context.Souvenirs
        //        .Include(s => s.Category)
        //        .Include(s => s.Supplier)
        //        .SingleOrDefaultAsync(m => m.SouvenirID == id);
        //    if (souvenir == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(souvenir);
        //}

        //// POST: MemberSouvenirs/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var souvenir = await _context.Souvenirs.SingleOrDefaultAsync(m => m.SouvenirID == id);
        //    _context.Souvenirs.Remove(souvenir);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool SouvenirExists(int id)
        {
            return _context.Souvenirs.Any(e => e.SouvenirID == id);
        }
    }
}
