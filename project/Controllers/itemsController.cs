 using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            var categoies = await _context.categories.ToListAsync();
            ViewBag.categories = categoies;
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
        public async Task<IActionResult> Create()
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
        public async Task<IActionResult> categoryList()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            List<categories> categories = new List<categories>();
            categories = await _context.categories.ToListAsync();
            return View(categories);
        }
        public async Task<IActionResult> catalog()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "customer")
                return RedirectToAction("notHere", "home");
            List<categories> categories = new List<categories>();

           var  cats = await _context.categories.ToListAsync();
            foreach (categories cc in cats) {
                categories.Add(new categories
                { 
                name = cc.name
                });
            
            }
            ViewBag.categories = categories;
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
            var categoriesList = await _context.categories.ToListAsync();
            List<items> cats = new List<items>();
            foreach (categories category in categoriesList) {
                var builder = WebApplication.CreateBuilder();
                string conStr = builder.Configuration.GetConnectionString("projectContext");
                SqlConnection conn = new SqlConnection(conStr);
                SqlCommand comm;
                conn.Open();
                string sql = "SELECT COUNT(quantity) FROM items where category ='"+ category.name + "'";
                comm = new SqlCommand(sql, conn);

                cats.Add(new items
                {
                    name = category.name,
                    quantity = (int)comm.ExecuteScalar(),
                }) ;
                conn.Close();

            }
            ViewData["direc"] = Directory.GetCurrentDirectory();
            ViewBag.categories = cats;
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> addCategory([Bind("name")] categories category)
        {           

           _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> addItem(int? id)
        {
            string cart = HttpContext.Session.GetString("cart");
            if (cart == null)
            {
                cart = "";
            }
            else {
                cart += ",";
            }
            cart += id;
            HttpContext.Session.SetString("cart",cart);
            return RedirectToAction("cart","Items");
        }
        public async Task<IActionResult> clearCart()
        {
            HttpContext.Session.Remove("cart");
            return RedirectToAction("cart", "Items");
        }
        public async Task<IActionResult> completeOrder()
        {
            HttpContext.Session.Remove("cart");
            HttpContext.Session.SetString("orderComplete","your order is complete");
            return RedirectToAction("myPurchase", "orders");
        }
        public async Task<IActionResult> removeCartItem(int? id)
        {
            string cart = HttpContext.Session.GetString("cart");
            List<int> items = new List<int>();
            items = ConvertToList(cart);
            if (items.ToArray().Length<=1) {
                HttpContext.Session.Remove("cart");
                return RedirectToAction("cart", "Items");
            }
            items.Remove((int)id);
            cart = ConvertListToString(items);
            HttpContext.Session.SetString("cart",cart);
            return RedirectToAction("cart", "Items");
        }
        public IActionResult cart()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "customer")
                return RedirectToAction("notHere", "home");
            List<items> items = new List<items>();
            string cart = HttpContext.Session.GetString("cart");
            cart = cart == null ? "-1" : cart;
            List<int> numbers = new List<int>();
            numbers = ConvertToList(cart);
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("projectContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "select * from items";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read()) {
                if (numbers.Contains((int)reader["Id"]))
                {
                    items.Add(new items
                    {
                        Id = (int)reader["Id"],
                        name = (string)reader["name"],
                        price = (string)reader["discount"] == "no" ? (int)reader["price"] : ((int)reader["price"] - ((int)reader["price"]/10)),
                        quantity = (int)reader["quantity"],
                        discount = (string)reader["discount"],
                        imagefilename= (string)reader["imagefilename"]
                    }
                    );
                }
            }
            reader.Close();
            sql = "select * from globalConfig where name='acceptNewOrder'";
            comm = new SqlCommand(sql, conn);
            reader = comm.ExecuteReader();
            reader.Read();
            ViewData["acceptOrder"] = (int)reader["value"] != 1 ? "no" : "yes";
            conn.Close();
            ViewData["userId"] = HttpContext.Session.GetString("id");
            return View(items);
        }
        public static List<int> ConvertToList(string input)
        {
            try
            {
                string[] numberStrings = input.Split(',');

                List<int> result = new List<int>();

                foreach (var numberString in numberStrings)
                {
                    if (int.TryParse(numberString, out int number))
                    {
                        result.Add(number);
                    }
                    else
                    {
                        return null;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string ConvertListToString(List<int> numbers)
        {
            return string.Join(",", numbers);
        }

        private bool itemsExists(int id)
        {
          return (_context.items?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
