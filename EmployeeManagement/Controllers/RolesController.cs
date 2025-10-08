using EmployeeManagement.Data;
using EmployeeManagement.Helpers;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public RolesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role model)
        {

            // Check if role exists
            var roleName = model.RoleName.Trim();

            // Check if role already exists (case-insensitive)
            var exists = await _roleManager.RoleExistsAsync(roleName);
            if (exists)
            {
                AlertHelper.AddErrorMessage(this, "Role already exists!");
                return View(model);
            }

            // Create new role
            var role = new IdentityRole(model.RoleName);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Role created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // If creation fails, show the errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            TempData["ErrorMessage"] = "Failed to create role.";
            return View(model);
        }


        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var result = await _roleManager.FindByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            var role = new Role
            {
                Id = result.Id,
                RoleName = result.Name
            };

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, Role model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                AlertHelper.AddErrorMessage(this, "Role not found.");
                return RedirectToAction(nameof(Index));
            }

            var checkIfExist = await _roleManager.RoleExistsAsync(model.RoleName);
            if (checkIfExist && role.Name != model.RoleName)
            {
                AlertHelper.AddErrorMessage(this, "Role already exists!");
                return View(model);
            }

            role.Name = model.RoleName;
            var finalResult = await _roleManager.UpdateAsync(role);

            if (finalResult.Succeeded)
            {
                AlertHelper.AddWarningMessage(this, "Role updated successfully!");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in finalResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                AlertHelper.AddErrorMessage(this, "Invalid role ID.");
                return RedirectToAction(nameof(Index));
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                AlertHelper.AddErrorMessage(this, "Role not found.");
                return RedirectToAction(nameof(Index));
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Any())
            {
                AlertHelper.AddErrorMessage(this, "Cannot delete role because it is assigned to users.");
                return RedirectToAction(nameof(Index));
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                AlertHelper.AddSuccessMessage(this, "Role deleted successfully!");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                AlertHelper.AddErrorMessage(this, "Error deleting role.");
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var model = new Role
            {
                Id = role.Id,
                RoleName = role.Name
            };

            return View(model);
        }
    }
}
