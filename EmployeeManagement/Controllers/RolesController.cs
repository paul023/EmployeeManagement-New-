using EmployeeManagement.Data;
using EmployeeManagement.Helpers;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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

        public async Task<ActionResult> Index()
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
        public async Task<ActionResult> Create(Role model)
        {
          
                var role = new IdentityRole { Name = model.RoleName };
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                AlertHelper.AddSuccessMessage(this, "Role created successfully!");
                return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            var role = new Role();
            var result = await _roleManager.FindByIdAsync(id);
            role.RoleName = result.Name;
            role.Id = result.Id;
      
            return View(role);
        }

        [HttpPost]

        public async Task<ActionResult> Edit(string id, Role model)
        {
            var checkifexist = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!checkifexist)
            {
                var result = await _roleManager.FindByIdAsync(id);
                result.Name = model.RoleName;

                var finalresult = await _roleManager.UpdateAsync(result);
                if (finalresult.Succeeded)
                {
                    AlertHelper.AddWarningMessage(this, "Role created successfully!");
                    return RedirectToAction("Index");
                }
                else
                {
                   
                    return View(model);
                }
            }
            AlertHelper.AddErrorMessage(this, "Role already exist!");
            return View(model);
        }
      

        // POST: Roles/DeleteConfirmed/5
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

            // Prevent deleting if users are assigned
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Any())
            {
                AlertHelper.AddErrorMessage(this, "Cannot delete role because it is assigned to users.");
                return RedirectToAction(nameof(Index));
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                AlertHelper.AddErrorMessage(this, "Role deleted successfully!");
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
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            // Map to your Role model (if you're using a custom model for views)
            var model = new Role
            {
                Id = role.Id,
                RoleName = role.Name
            };

            return View(model);
        }



    }
}

