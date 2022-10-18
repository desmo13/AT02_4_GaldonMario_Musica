using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicaAut_GaldonMario.Areas.Identity.Data;
using MusicaAut_GaldonMario.Models;
using System.Data;

namespace MusicaAut_GaldonMario.Controllers
{
    public class AdminController : Controller
    {
        private readonly ChinookContext _context;
        private readonly AuthContext _authContext;
        
        public AdminController(ChinookContext context, AuthContext authContext)
        {
            _context = context;
            _authContext = authContext;
        }

        // GET: Admin
        [Authorize(Roles = "Manager,Administrator")]
        public  IActionResult Index()
        {

            return View();
        }
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> CustomerList()
        {
            var chinookContext = _context.Customers.Include(c => c.SupportRep);
            return View(await chinookContext.ToListAsync());
        }
        // GET: Admin/Details/5
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.SupportRep)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [Authorize(Roles = "Manager,Administrator")]
        // GET: Admin/Create
        public IActionResult Create()
        {
            ViewData["SupportRepId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName");
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Company,Address,City,State,Country,PostalCode,Phone,Fax,Email,SupportRepId")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.CustomerId = _context.Customers.Max(a => a.CustomerId)+1;
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CustomerList));
            }
            ViewData["SupportRepId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", customer.SupportRepId);
            return View(customer);
        }

        // GET: Admin/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["SupportRepId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", customer.SupportRepId);
            return View(customer);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,FirstName,LastName,Company,Address,City,State,Country,PostalCode,Phone,Fax,Email,SupportRepId")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(CustomerList));
            }
            ViewData["SupportRepId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", customer.SupportRepId);
            return View(customer);
        }

        // GET: Admin/Delete/5
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.SupportRep)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'ChinookContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CustomerList));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UserList()
        {
            var listaUsuarios = await _authContext.Users.ToListAsync();
            return View(listaUsuarios);
        }
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UserEdit(string? id)
        {
            if (id == null ||  _authContext.Users== null)
            {
                return NotFound();
            }

            var user = await _authContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
           
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UserEdit(string Id, [Bind("Id,UserName,PhoneNumber,Email,EmailConfirmer")] AuthUser user)
        {
            if (Id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //user.LockoutEnd = _authContext.Users.LockoutEnd.find()
                    _authContext.Update(user);
                    await _authContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExist(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(UserList));
            }
            
            return View(user);
        }
        private bool UserExist(string Id)
        {
            return _authContext.Users.Any(e => e.Id == Id);
        }
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UserDelete(string? id)
        {
            if (id == null || _authContext.Users == null)
            {
                return NotFound();
            }

            var user = await _authContext.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("UserDelete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UserDeleteConfirmed(string id)
        {
            if (_authContext.Users == null)
            {
                return Problem("Problema con la base de datos de usuario");
            }
            var user = await _authContext.Users.FindAsync(id);
            if (user != null)
            {
                _authContext.Users.Remove(user);
            }

            await _authContext.SaveChangesAsync();
            return RedirectToAction(nameof(UserList));
        }
    }
}
