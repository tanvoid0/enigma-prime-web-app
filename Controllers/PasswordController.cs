using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using enigma_prime.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace enigma_prime.Controllers
{
    public class PasswordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;


        public PasswordController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [Authorize]
        // GET: Password
        public async Task<IActionResult> Index()
        {
            var passwords = await _context.Password.ToListAsync();
            foreach (var password in passwords)
            {
                password.Pass = decrypt(password.Pass);
            }
            return View(passwords);
        }

        [Authorize]
        // GET: Password/Details/5
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

        [Authorize]
        // GET: Password/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        // POST: Password/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Pass,Type,Url,Developer,CreatedAt,UpdatedAt")] Password password)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var pass = new Password
                {
                    Name = password.Name,
                    Pass = encrypt(password.Pass),
                    Type = password.Type,
                    Url = password.Url,
                    Developer = password.Developer,
                    UserId = currentUserId,
                };
                _context.Add(pass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(password);
        }

        [Authorize]
        // GET: Password/Edit/5
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

        [Authorize]
        // POST: Password/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Pass,Type,Url,Developer,CreatedAt,UpdatedAt")] Password password)
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

        [Authorize]
        // GET: Password/Delete/5
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

        [Authorize]
        // POST: Password/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var password = await _context.Password.FindAsync(id);
            _context.Password.Remove(password);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        private bool PasswordExists(int id)
        {
            return _context.Password.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        public static string encrypt(string str)
        {
            char[] newStr = str.ToCharArray();
            for(int i=0; i<str.Length; i++){
                newStr[i] = Convert.ToChar(System.Convert.ToInt32(str[i])-5);
            }
            return new string(newStr);
        }
        
        public static string decrypt(string str)
        {
            char[] newStr = str.ToCharArray();
            for(int i=0; i<str.Length; i++){
                newStr[i] = Convert.ToChar(System.Convert.ToInt32(str[i])+5);
            }
            return new string(newStr);
        }

        public async Task createRolesandUsers()
        {
            // bool x = await _roleManager.RoleExistsAsync("Admin");
            // if (!x)
            // {
            //     // first we create Admin role
            //     var role = new IdentityRole();
            //     role.Name = "Admin";
            //     await _roleManager.CreateAsync(role);
            //     
            //     // Here we create an Admin super user who will maintain the website
            //     var user = new ApplicationUser();
            //     user.UserName = "root";
            //     
            // }
        }
    }
}
