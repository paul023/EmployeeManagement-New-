using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class LeaveAppsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LeaveAppsController> _logger;

        public LeaveAppsController(ApplicationDbContext context, ILogger<LeaveAppsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: LeaveApps
        public async Task<IActionResult> Index()
        {
            var awaitingstatus = _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.code == "Approval Leave Status" && y.Code == "Awaiting Approval").FirstOrDefault();

            var applicationDbContext = _context.LeaveApps
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .Where(l => l.StatusId == awaitingstatus!.Id);
            return View(await applicationDbContext.ToListAsync());
        }
        
        public async Task<IActionResult> ApprovedLeave()
        {
            var approvedstatus = _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.code == "Approval Leave Status" && y.Code == "Approved").FirstOrDefault();

            var applicationDbContext = _context.LeaveApps
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
            .Where(l => l.StatusId == approvedstatus!.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> RejectedLeave()
        {
            var rejectstatus = _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.code == "Approval Leave Status" && y.Code == "Rejected").FirstOrDefault();

            var applicationDbContext = _context.LeaveApps
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
            .Where(l => l.StatusId == rejectstatus!.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpGet]
        //[HttpGet]
        public IActionResult Approve(int id)
        {
            var leaveApp = _context.LeaveApps
                .Include(l => l.Employee)  // Include Employee details
                .Include(l => l.Duration)  // Include Duration details
                .Include(l => l.LeaveType) // Include LeaveType details
                .Include(l => l.Status)    // Include Status details
                .FirstOrDefault(l => l.Id == id);

            if (leaveApp == null)
            {
                return NotFound(); // Prevents invalid ID issues
            }

            return View(leaveApp);
        }
        [HttpPost]
        public async Task<IActionResult> Approve(LeaveApp leave)
        {
            var approvedstatus = _context.SystemCodeDetails.Include(x=>x.SystemCode).Where(y=>y.SystemCode.code== "Approval Leave Status" && y.Code=="Approved").FirstOrDefault();

            var Approved = await _context.LeaveApps
                .Include(l => l.Employee)
                .Include(l => l.Duration)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(l => l.Id == leave.Id);
            if(Approved == null)
            {
                return NotFound();
            }
            Approved.ApprovedOn= DateTime.Now;
            Approved.ApprovedId = "Paul";
            Approved.StatusId = approvedstatus!.Id;
            Approved.ApprovalNotes = leave.ApprovalNotes;
            _context.Update(Approved);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        //[HttpGet]
        public IActionResult Reject(int id)
        {
            var leaveApp = _context.LeaveApps
                .Include(l => l.Employee)  // Include Employee details
                .Include(l => l.Duration)  // Include Duration details
                .Include(l => l.LeaveType) // Include LeaveType details
                .Include(l => l.Status)    // Include Status details
                .FirstOrDefault(l => l.Id == id);

            if (leaveApp == null)
            {
                return NotFound(); // Prevents invalid ID issues
            }
             
            return View(leaveApp);
        }
        [HttpPost]
        public async Task<IActionResult>Reject(LeaveApp leave)
        {
            var Reject = _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.code == "Approval Leave Status" && y.Code == "Rejected").FirstOrDefault();

            var LeaveApp = await _context.LeaveApps
                .Include(l => l.Employee)
                .Include(l => l.Duration)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(l => l.Id == leave.Id);
            if (Reject == null)
            {
                return NotFound();
            }
            LeaveApp.ApprovedOn = DateTime.Now;
            LeaveApp.ApprovedId = "Paul";
            LeaveApp.StatusId = Reject.Id;
            LeaveApp.ApprovalNotes = leave.ApprovalNotes;

            _context.Update(LeaveApp);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // GET: LeaveApps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var leaveApp = await _context.LeaveApps
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == id);

            return leaveApp == null ? NotFound() : View(leaveApp);
        }

        // GET: LeaveApps/Create
        public async Task<IActionResult> Create()
        {
            await LoadViewData();
            return View();
        }

        // POST: LeaveApps/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveApp leaveApp)
        {
            // Retrieve pending status as before.
            var pendingStatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.Code == "Awaiting Approval" && y.SystemCode.code == "Approval Leave Status")
                .FirstOrDefaultAsync();

            if (pendingStatus == null)
            {
                ModelState.AddModelError("", "Pending status not found.");
                await LoadViewData();
                return View(leaveApp);
            }

            // If no attachment provided, assign a default value (empty string).
            leaveApp.AttachmentPath = string.IsNullOrEmpty(leaveApp.AttachmentPath)
                ? string.Empty
                : leaveApp.AttachmentPath;

            leaveApp.CreatedOn = DateTime.Now;
            leaveApp.CreateById = User.Identity?.Name ?? "Unknown";
            leaveApp.StatusId = pendingStatus.Id;

            _context.Add(leaveApp);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: LeaveApps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var leaveApp = await _context.LeaveApps.FindAsync(id);
            if (leaveApp == null) return NotFound();

            await LoadViewData(leaveApp);
            return View(leaveApp);
        }

        // POST: LeaveApps/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeaveApp leaveApp)
        {
            if (id != leaveApp.Id) return NotFound();

            var pendingStatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.Code == "Pending" && y.SystemCode.code == "Approval Leave Status")
                .FirstOrDefaultAsync();

            if (pendingStatus == null)
            {
                ModelState.AddModelError("", "Pending status not found.");
                await LoadViewData(leaveApp);
                return View(leaveApp);
            }

            try
            {
                leaveApp.CreatedOn = DateTime.Now;
                leaveApp.CreateById = User.Identity?.Name ?? "Unknown";
                leaveApp.StatusId = pendingStatus.Id;

                _context.Update(leaveApp);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error when updating leave application.");
                if (!LeaveAppExists(leaveApp.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: LeaveApps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var leaveApp = await _context.LeaveApps
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == id);

            return leaveApp == null ? NotFound() : View(leaveApp);
        }

        // POST: LeaveApps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveApp = await _context.LeaveApps.FindAsync(id);
            if (leaveApp != null)
            {
                _context.LeaveApps.Remove(leaveApp);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveAppExists(int id)
        {
            return _context.LeaveApps.Any(e => e.Id == id);
        }

        /// <summary>
        /// Loads the necessary data for dropdowns in Create and Edit views.
        /// </summary>
        private async Task LoadViewData(LeaveApp leaveApp = null)
        {
            ViewData["DurationId"] = new SelectList(
                await _context.SystemCodeDetails
                    .Include(x => x.SystemCode)
                    .Where(y => y.SystemCode.code == "LeaveDuration")
                    .ToListAsync(),
                "Id", "Description", leaveApp?.DurationId);

            ViewData["EmployeeId"] = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName", leaveApp?.EmployeeId);
            ViewData["LeaveTypeId"] = new SelectList(await _context.LeaveTypes.ToListAsync(), "Id", "name", leaveApp?.LeaveTypeId);
        }
    }
}
