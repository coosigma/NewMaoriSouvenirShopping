﻿using System;
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
using System.Data.SqlClient;

namespace MaoriSouvenirShopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SouvenirsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;

        public SouvenirsController(ApplicationDbContext context,IHostingEnvironment hEnv)
        {
            _context = context;
            _hostingEnv = hEnv;
        }

        // GET: Souvenirs
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string currentCategory,
            decimal? currentLowerPrice,
            decimal? currentUpperPrice,
            string searchString,
            string category,
            decimal? lower_price,
            decimal? upper_price,
            int? page)
        {
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
                if(lower_price == null && upper_price == null)
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
            if(lower_price != null || upper_price != null)
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

        // GET: Souvenirs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var souvenir = await _context.Souvenirs
                .Include(s => s.Category)
                .Include(s => s.Supplier)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.SouvenirID == id);
            if (souvenir == null)
            {
                return NotFound();
            }

            return View(souvenir);
        }

        // GET: Souvenirs/Create
        public IActionResult Create()
        {
            loadViewData();
            return View();
        }

        // POST: Souvenirs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SouvenirName,Price,PhotoPath,Description,CategoryID,SupplierID")] Souvenir souvenir, IList<IFormFile> _files)
        {
            var relativeName = "";
            var fileName = "";

            if (_files.Count < 1)
            {
                relativeName = "/images/Souvenir.svg";
            }
            else
            {
                foreach (var file in _files)
                {
                    fileName = ContentDispositionHeaderValue
                                      .Parse(file.ContentDisposition)
                                      .FileName
                                      .Trim('"');
                    //Path for localhost
                    relativeName = "/images/SouvenirImages/" + DateTime.Now.ToString("ddMMyyyy-HHmmssffffff") + fileName;

                    using (FileStream fs = System.IO.File.Create(_hostingEnv.WebRootPath + relativeName))
                    {
                        await file.CopyToAsync(fs);
                        fs.Flush();
                    }
                }
            }
            souvenir.PhotoPath = relativeName;
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(souvenir);
                    await _context.SaveChangesAsync();
                    loadViewData();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            loadViewData();
            return View(souvenir);
        }
        public void loadViewData()
        {
            var categories = _context.Categories.AsNoTracking();
            var category = new Category();
            try
            {
                category = categories.Where(c => c.CategoryName == "MaoriGift").Single();
                ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", category.CategoryID);
            }
            catch (InvalidOperationException)
            {
                ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", 0);
            }
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "FullName");
        }

        // GET: Souvenirs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var souvenir = await _context.Souvenirs
                .SingleOrDefaultAsync(m => m.SouvenirID == id);
            if (souvenir == null)
            {
                return NotFound();
            }
            loadViewData();
            return View(souvenir);
        }

        // POST: Souvenirs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, byte[] rowVersion, IList<IFormFile> _files)
        {
            if (id == null)
            {
                return NotFound();
            }
            var souvenirToUpdate = await _context.Souvenirs.SingleOrDefaultAsync(s => s.SouvenirID == id);

            if (souvenirToUpdate == null)
            {
                Souvenir deletedSouvenir = new Souvenir();
                await TryUpdateModelAsync(deletedSouvenir);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The Souvenir was deleted by another Administrator.");
                loadViewData();
                return View(deletedSouvenir);
            }

            _context.Entry(souvenirToUpdate).Property("RowVersion").OriginalValue = rowVersion;
            var relativeName = "";
            var fileName = "";

            if (_files.Count < 1)
            {
                relativeName = souvenirToUpdate.PhotoPath;
            }
            else
            {
                foreach (var file in _files)
                {
                    fileName = ContentDispositionHeaderValue
                                      .Parse(file.ContentDisposition)
                                      .FileName
                                      .Trim('"');
                    //Path for localhost
                    relativeName = "/images/SouvenirImages/" + DateTime.Now.ToString("ddMMyyyy-HHmmssffffff") + fileName;

                    using (FileStream fs = System.IO.File.Create(_hostingEnv.WebRootPath + relativeName))
                    {
                        await file.CopyToAsync(fs);
                        fs.Flush();
                    }
                }
            }            
            souvenirToUpdate.PhotoPath = relativeName;

            if (await TryUpdateModelAsync<Souvenir>(
                souvenirToUpdate,
                "",
                s => s.SouvenirName, s => s.Price, s => s.PhotoPath,
                s => s.PhotoPath, s => s.Description, s => s.CategoryID,
                s => s.SupplierID))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        await _context.SaveChangesAsync();
                        loadViewData();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Souvenir)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The souvenir was deleted by another administrator.");
                    }
                    else
                    {
                        var databaseValues = (Souvenir)databaseEntry.ToObject();

                        if (databaseValues.SouvenirName != clientValues.SouvenirName)
                        {
                            ModelState.AddModelError("Souvenir Name", $"Current value: {databaseValues.SouvenirName}");
                        }
                        if (databaseValues.Price != clientValues.Price)
                        {
                            ModelState.AddModelError("Price", $"Current value: {databaseValues.Price:c}");
                        }
                        if (databaseValues.PhotoPath != clientValues.PhotoPath)
                        {
                            ModelState.AddModelError("Photo", $"Current value: {databaseValues.PhotoPath}");
                        }
                        if (databaseValues.Description != clientValues.Description)
                        {
                            ModelState.AddModelError("Description", $"Current value: {databaseValues.Description}");
                        }
                        if (databaseValues.CategoryID != clientValues.CategoryID)
                        {
                            Category databaseCategory = await _context.Categories.SingleOrDefaultAsync(c => c.CategoryID == databaseValues.CategoryID);
                            ModelState.AddModelError("Category", $"Current value: {databaseCategory?.CategoryName}");
                        }
                        if (databaseValues.SupplierID != clientValues.SupplierID)
                        {
                            Supplier databaseSupplier = await _context.Suppliers.SingleOrDefaultAsync(s => s.SupplierID == databaseValues.SupplierID);
                            ModelState.AddModelError("Supplier", $"Current value: {databaseSupplier?.FullName}");
                        }
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another administrator after you got the original value. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to edit this record, click "
                                + "the Save button again. Otherwise click the Back to List hyperlink.");
                        souvenirToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists " +
                        "see your system administrator.");
                }
            }
            loadViewData();
            return View(souvenirToUpdate);
        }

        // GET: Souvenirs/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var souvenir = await _context.Souvenirs
                .Include(s => s.Category)
                .Include(s => s.Supplier)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.SouvenirID == id);

            if (souvenir == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another administrator after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }
            return View(souvenir);
        }

        // POST: Souvenirs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Souvenir souvenir)
        {
            try
            {
                if (await _context.Souvenirs.AnyAsync(m => m.SouvenirID == souvenir.SouvenirID))
                {
                    _context.Souvenirs.Remove(souvenir);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = souvenir.SouvenirID });
            }
            catch (DbUpdateException)
            {
                TempData["UserUsed"] = "The Souvenir being deleted has been ordered in previous orders.Delete those orders before trying again.";
                return RedirectToAction("Delete");
            }
        }

        private bool SouvenirExists(int id)
        {
            return _context.Souvenirs.Any(e => e.SouvenirID == id);
        }
    }
}
