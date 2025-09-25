using EmployeeManagement.Helpers;
using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: Banks/Create
        public IActionResult Create() => View();

        // POST: Banks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bank bank)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bank);
                await _context.SaveChangesAsync();
                AlertHelper.AddSuccessMessage(this, "Bank created successfully!");
                return RedirectToAction(nameof(Index));
            }
            return View(bank);
        }
        // GET: Cities/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null) return NotFound();

            var bank = await _context.Banks
               // .Include(b => b.Country)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bank == null) return NotFound();

            return View(bank);
        }
        // GET: Banks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bank = await _context.Banks.FindAsync(id);
            if (bank == null) return NotFound();

            return View(bank);
        }

        // POST: Banks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Bank bank)
        {
            if (id != bank.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bank);
                    await _context.SaveChangesAsync();
                    AlertHelper.AddWarningMessage(this, "Bank updated successfully!");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankExists(bank.Id)) return NotFound();
                    else throw;
                }
            }
            return View(bank);
        }

        // GET: Banks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bank = await _context.Banks.FirstOrDefaultAsync(m => m.Id == id);
            if (bank == null) return NotFound();

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
                AlertHelper.AddErrorMessage(this, "Bank deleted successfully!");
            }
            else
            {
                AlertHelper.AddErrorMessage(this, "Bank not found.");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BankExists(int id) => _context.Banks.Any(e => e.Id == id);
    }
}
