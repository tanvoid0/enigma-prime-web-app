using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using enigma_prime.Data;
using enigma_prime.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace enigma_prime.Controllers
{
    public class PasswordController : Controller
    {
        // To read/write database
        private readonly ApplicationDbContext _context;
        // To manage user roles
        private readonly RoleManager<IdentityRole> _roleManager;
        // To manage auth user data
        private readonly UserManager<IdentityUser> _userManager;

        // Initialize
        public PasswordController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Get authenticated user
        private Task<IdentityUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // Get authenticated user's Id
        private async Task<string> GetCurrentUserId()
        {
            IdentityUser usr = await GetCurrentUserAsync();
            return usr?.Id;
        }
        
        /**
         * Access password page as user
         * @method: GET
         * @path: /
         * @auth: required
         */
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Check for admin access
            if (User.IsInRole("Admin"))
            {
                // if admin, redirect to Admin page
                return RedirectToAction(nameof(Admin));
            }
            
            var userId = await GetCurrentUserId();
            // get authenticated users passwords only
            var passwords = await _context.Password.Where(p => p.UserId == userId).ToListAsync();

            // Update the user field based on the userid foreign key
            foreach (var password in passwords)
            {
                password.Pass = decrypt(password.Pass);
                password.User = await _userManager.FindByIdAsync(password.UserId);
            }
            
            return View(passwords);
        }
        
        /**
         * Access password page as admin
         * @method: GET
         * @path: Admin
         * @auth: required, admin
         */
        [Authorize]
        public async Task<IActionResult> Admin()
        {
            // Check if the authenticated user is an admin
            if (!User.IsInRole("Admin"))
            {
                // If not, redirect to users page instead
                return RedirectToAction(nameof(Index));
            }
            
            // get passwords for all users
            var passwords = await _context.Password.ToListAsync();
            foreach (var password in passwords)
            {
                password.Pass = decrypt(password.Pass);
                password.User = await _userManager.FindByIdAsync(password.UserId);
            }
            return View(passwords);
        }

        /**
         * Get details of a specific password
         * @method: GET
         * @path: Password/Details/5
         * @auth: required
         */
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var password = await _context.Password
                .FirstOrDefaultAsync(m => m.Id == id);
            if (password == null)
            {
                return NotFound();
            }

            password.Pass = decrypt(password.Pass);

            return View(password);
        }

        /**
         * Create Password page
         * @method: GET
         * @path: Password/Create
         * @auth: required
         */
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        /**
         * Create password api
         * @method: post
         * @path: Password/Edit/:id
         * @auth: required
         */
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Pass,Type,Url,Developer,CreatedAt,UpdatedAt")] Password password)
        {
            if (ModelState.IsValid)
            {
                // var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var pass = new Password
                {
                    Name = password.Name,
                    Pass = encrypt(password.Pass),
                    Type = password.Type,
                    Url = password.Url,
                    Developer = password.Developer,
                    UserId = await GetCurrentUserId(),
                };
                _context.Add(pass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(password);
        }

        /**
         * Edit password page
         * @method: get
         * @path: Password/Edit/:id
         * @auth: required
         */
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var password = await _context.Password.FindAsync(id);
            if (password == null)
            {
                return NotFound();
            }

            password.Pass = decrypt(password.Pass);
            return View(password);
        }

        /**
         * Edit password api
         * @method: post
         * @path: Password/Edit/:id
         * @auth: required
         */
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Pass,Type,Url,Developer,UserId,CreatedAt,UpdatedAt")] Password password)
        {
            if (id != password.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(password);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PasswordExists(password.Id))
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
            return View(password);
        }

        /**
         * Delete password
         * @method: get
         * @path: Password/Delete/:id
         * @auth: required
         */
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var password = await _context.Password
                .FirstOrDefaultAsync(m => m.Id == id);
            if (password == null)
            {
                return NotFound();
            }

            return View(password);
        }

        /**
         * Delete password api
         * @method: post
         * @path: Password/Delete/:id
         * @auth: required
         */
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var password = await _context.Password.FindAsync(id);
            _context.Password.Remove(password);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /**
         * Check if password exists or not
         */
        [Authorize]
        private bool PasswordExists(int id)
        {
            return _context.Password.Any(e => e.Id == id);
        }

        /**
         * Logout
         */
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        public static string encrypt(string str)
        {
            // convert the string to array of characters to easily access the indexes
            char[] newStr = str.ToCharArray();
            for(int i=0; i<str.Length; i++){
                // Shift each characters to -5 ASCII character
                newStr[i] = Convert.ToChar(System.Convert.ToInt32(str[i])-5);
            }
            return new string(newStr);
        }
        
        public static string decrypt(string str)
        {
            // convert the string to array of characters to easily access the indexes
            char[] newStr = str.ToCharArray();
            for(int i=0; i<str.Length; i++){
                // Shift each characters to +5 ASCII character
                newStr[i] = Convert.ToChar(System.Convert.ToInt32(str[i])+5);
            }
            return new string(newStr);
        }

       
    }
}
