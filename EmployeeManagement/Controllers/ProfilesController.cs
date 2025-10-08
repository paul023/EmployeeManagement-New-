using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

      
        public async Task<IActionResult> Index()
        {
            var task = new ProfileViewModel();
            var roles = await _context.Roles.OrderBy(x=>x.Name).ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
           
            var systemtask = await _context.SystemProfiles
                .Include("Children.Children.Children")
                .OrderBy(r => r.Order)
     
                .ToListAsync();
            
            ViewBag.Tasks = new SelectList(systemtask, "Id", "Name");
            return View(task);
         }

        public async Task<ActionResult> AssignRights(ProfileViewModel vm)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roles = new RoleProfile
            {
                TaskId = vm.TaskId,
                RoleId = vm.RoleId,
            };
            _context.RoleProfiles.Add(roles);
            await _context.SaveChangesAsync(userId);
            return RedirectToAction("Index");
        }

        // GET: RoleProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleProfile = await _context.RoleProfiles
                .Include(r => r.Role)
                .Include(r => r.Task)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roleProfile == null)
            {
                return NotFound();
            }

            return View(roleProfile);
        }

        // GET: RoleProfiles/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id");
            ViewData["TaskId"] = new SelectList(_context.SystemProfiles, "Id", "Id");
            return View();
        }

        // POST: RoleProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TaskId,RoleId")] RoleProfile roleProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roleProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", roleProfile.RoleId);
            ViewData["TaskId"] = new SelectList(_context.SystemProfiles, "Id", "Id", roleProfile.TaskId);
            return View(roleProfile);
        }

        // GET: RoleProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleProfile = await _context.RoleProfiles.FindAsync(id);
            if (roleProfile == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", roleProfile.RoleId);
            ViewData["TaskId"] = new SelectList(_context.SystemProfiles, "Id", "Id", roleProfile.TaskId);
            return View(roleProfile);
        }

        // POST: RoleProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TaskId,RoleId")] RoleProfile roleProfile)
        {
            if (id != roleProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roleProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleProfileExists(roleProfile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", roleProfile.RoleId);
            ViewData["TaskId"] = new SelectList(_context.SystemProfiles, "Id", "Id", roleProfile.TaskId);
            return View(roleProfile);
        }

        // GET: RoleProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleProfile = await _context.RoleProfiles
                .Include(r => r.Role)
                .Include(r => r.Task)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roleProfile == null)
            {
                return NotFound();
            }

            return View(roleProfile);
        }

        // POST: RoleProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roleProfile = await _context.RoleProfiles.FindAsync(id);
            if (roleProfile != null)
            {
                _context.RoleProfiles.Remove(roleProfile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleProfileExists(int id)
        {
            return _context.RoleProfiles.Any(e => e.Id == id);
        }
    }
}
