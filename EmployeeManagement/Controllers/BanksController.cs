using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Data;
using EmployeeManagement.Models;

namespace EmployeeManagement.Controllers
{
    public class BanksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BanksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Banks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Banks.ToListAsync());
        }

        // GET: Banks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var bank = await _context.Banks.FirstOrDefaultAsync(m => m.Id == id);
            if (bank == null)
                return NotFound();

            return View(bank);
        }

        // GET: Banks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Banks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bank bank)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bank);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Bank created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(bank);
        }

        // GET: Banks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var bank = await _context.Banks.FindAsync(id);
            if (bank == null)
                return NotFound();

            return View(bank);
        }

        // POST: Banks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Bank bank)
        {
            if (id != bank.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bank);
                    await _context.SaveChangesAsync();
                    TempData["WarningMessages"] = "Bank updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankExists(bank.Id))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(bank);
        }

        // GET: Banks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var bank = await _context.Banks.FirstOrDefaultAsync(m => m.Id == id);
            if (bank == null)
                return NotFound();

            return View(bank);
        }

        // POST: Banks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bank = await _context.Banks.FindAsync(id);
            if (bank != null)
            {
                _context.Banks.Remove(bank);
                await _context.SaveChangesAsync();
                TempData["ErrorMessage"] = "Bank deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Bank not found.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BankExists(int id)
        {
            return _context.Banks.Any(e => e.Id == id);
        }
    }
}
