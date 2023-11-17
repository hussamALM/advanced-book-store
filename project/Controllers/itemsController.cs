 using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.Models;

namespace project.Controllers
{
    public class itemsController : Controller
    {
        private readonly projectContext _context;

        public itemsController(projectContext context)
        {
            _context = context;
        }

        // GET: items
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            string role = HttpContext.Session.GetString("role");
            ViewData["role"] = role;
            ViewData["page"] = HttpContext.Session.GetString("role");
            return _context.items != null ?
                        View(await _context.items.ToListAsync()) :
                        Problem("Entity set 'projectContext.items'  is null.");

        }

        // GET: items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }
            ViewData["page"] = HttpContext.Session.GetString("role");
            return View(items);
        }

        // GET: items/Create
        public IActionResult Create()
        {

            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            return View();
        }

        // POST: items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, [Bind("Id,name,descr,price,quantity,discount,category,imagefilename,registDate")] items item)
        {
            {
                if (file != null)
                {
                    string filename = file.FileName;
                    string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                    using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    { await file.CopyToAsync(filestream); }

                    item.imagefilename = filename;
                }

                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }


        // GET: items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var items = await _context.items.FindAsync(id);
            if (items == null)
            {
                return NotFound();
            }
            return View(items);
        }

        // POST: items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile file, int id, [Bind("Id,name,descr,price,quantity,discount,category,imagefilename")] items item)
        {
            if (id != item.Id)
            { return NotFound(); }
            if (file != null)
            {
                string filename = file.FileName;
                string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                { await file.CopyToAsync(filestream); }

                item.imagefilename = filename;
            }
            _context.Update(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }

        // GET: items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // POST: items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.items == null)
            {
                return Problem("Entity set 'projectContext.items'  is null.");
            }
            var items = await _context.items.FindAsync(id);
            if (items != null)
            {
                _context.items.Remove(items);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> list()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            List<items> books = new List<items>();
            books = await _context.items.OrderBy(m => m.category).ToListAsync();
            ViewData["page"] = HttpContext.Session.GetString("role");
            return View(books);
        }
        public async Task<IActionResult> catalog()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "customer")
                return RedirectToAction("notHere", "home");
            return View(await _context.items.ToListAsync());
        }
        public async Task<IActionResult> imageSlider()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "customer")
                return RedirectToAction("notHere", "home");
            List<items> books = new List<items>();
            books = await _context.items.ToListAsync();
            return View(books);
        }
        public async Task<IActionResult> dashboard()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            string sql = "";

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("projectContext");
            SqlConnection conn = new SqlConnection(conStr);
            SqlCommand comm;
            conn.Open();
            sql = "SELECT COUNT(quantity) FROM items where category ='self-improvement'";
            comm = new SqlCommand(sql, conn);
            ViewData["d1"] = (int)comm.ExecuteScalar();

            sql = "SELECT COUNT(quantity) FROM items where category ='money-management'";
            comm = new SqlCommand(sql, conn);
            ViewData["d2"] = (int)comm.ExecuteScalar();
            return View();

        }

        private bool itemsExists(int id)
        {
          return (_context.items?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
