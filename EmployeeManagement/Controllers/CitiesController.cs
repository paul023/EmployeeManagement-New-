using EmployeeManagement.Data;
using EmployeeManagement.Helpers;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class CitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cities
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Cities.Include(c => c.Country);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Cities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // GET: Cities/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(City city)
        {

            if (await _context.Cities.AnyAsync(c => c.Name == city.Name))
            {
                ModelState.AddModelError("Name", "A city with this name already exists.");
                ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", city.CountryId);
                return View(city);
            }
            city.CreateById = "Paul";
                city.CreatedOn = DateTime.Now;
                _context.Add(city);
                await _context.SaveChangesAsync();

            // Set TempData message
            AlertHelper.AddSuccessMessage(this, "City created successfully!");
            return RedirectToAction(nameof(Index));
            
        }

        // GET: Cities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", city.CountryId);
            return View(city);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,City city)
        {
            if (id != city.Id)
            {
                return NotFound();
            }
            if (await _context.Cities.AnyAsync(c => c.Name == city.Name && c.Id != city.Id))
            {
                ModelState.AddModelError("Name", "A city with this name already exists.");
                ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", city.CountryId);
                return View(city);
            }

            try
                {
                    city.ModifiedById = "Paul";
                    city.ModifiedOn = DateTime.Now;
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                AlertHelper.AddWarningMessage(this, "City updated successfully!");
                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CityExists(city.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", city.CountryId);
            return View(city);
        }

     

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city != null)
            {
                _context.Cities.Remove(city);
            }

            await _context.SaveChangesAsync();
            AlertHelper.AddErrorMessage(this, "City deleted successfully!");
            return RedirectToAction(nameof(Index));
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}
