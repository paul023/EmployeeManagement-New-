using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }
        public async Task <ActionResult> Index()
        {
            
            var users = await _context.Users.Include(x=>x.Role).ToListAsync();
           
            return View(users);
        }
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id","Name");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(User User)
        {
            ApplicationUser user = new ApplicationUser();
            user.UserName = User.UserName;
            user.FirstName = User.FirstName;
            user.MiddleName = User.MiddleName;
            user.LastName = User.LastName;
            user.NationalId = User.NationalId;
            user.NormalizedUserName = User.UserName;
            user.Email = User.Email;
            user.EmailConfirmed = true;
            user.PhoneNumber = User.PhoneNumber;
            user.PhoneNumberConfirmed = true;
            user.CreatedOn = DateTime.Now;
            user.CreateById = "Paul";
            user.RoleId = User.RoleId;
            var result = await _userManager.CreateAsync(user,User.Password);
            
            if(result.Succeeded)
             {
                return RedirectToAction("Index");
            }
            else
            {
                return View(User);
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", User.RoleId);
        }
    }
}
