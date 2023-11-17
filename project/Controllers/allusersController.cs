using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.Models;

namespace project.Controllers
{

    public class allusersController : Controller
    {
        private readonly projectContext _context;
        public allusersController(projectContext context)
        {
            _context = context;
        }

        // GET: allusers
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");

            return _context.allusers != null ?
                        View(await _context.allusers.ToListAsync()) :
                        Problem("Entity set 'projectContext.allusers'  is null.");
        }

        // GET: allusers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (id == null || _context.allusers == null)
            {
                return NotFound();
            }

            var allusers = await _context.allusers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allusers == null)
            {
                return NotFound();
            }

            return View(allusers);
        }

        // GET: allusers/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            return View();
        }

        // POST: allusers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,password,role,registDate")] allusers allusers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(allusers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(allusers);
        }

        // GET: allusers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            if (id == null || _context.allusers == null)
            {
                return NotFound();
            }

            var allusers = await _context.allusers.FindAsync(id);
            if (allusers == null)
            {
                return NotFound();
            }
            return View(allusers);
        }

        // POST: allusers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,password,role,registDate")] allusers allusers)
        {
            if (id != allusers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(allusers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!allusersExists(allusers.Id))
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
            return View(allusers);
        }

        // GET: allusers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            if (id == null || _context.allusers == null)
            {
                return NotFound();
            }

            var allusers = await _context.allusers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allusers == null)
            {
                return NotFound();
            }

            return View(allusers);
        }

        // POST: allusers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.allusers == null)
            {
                return Problem("Entity set 'projectContext.allusers'  is null.");
            }
            var allusers = await _context.allusers.FindAsync(id);
            if (allusers != null)
            {
                _context.allusers.Remove(allusers);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> search() {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            return View();
}
        [HttpPost]
        public async Task<IActionResult> search(string name)
        {
            var user = await _context.allusers.FromSqlRaw("select * from allusers where name = '" + name + "' ").FirstOrDefaultAsync();

            return View(user);
        }

        public async Task<IActionResult> addAdmin()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> addAdmin(string name,string password,string password2)
        {
            if (password != password2)
            {
                ViewData["err"] = "the two password do not match";
                return View();

            }
            allusers user = await _context.allusers.FromSqlRaw("select * from allusers where name = '" + name + "' ").FirstOrDefaultAsync();
            if (user==null) {
                ViewData["err"] = "the user was not found";
                return View();
            }
            if (user.password!=password) {
                ViewData["err"] = "the password is not correct";
                return View();
            }
            user.role = "admin";
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("search");
        }
        public async Task<IActionResult> customerHome()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "customer")
                return RedirectToAction("notHere", "home");
            List<items> itemList = new List<items>();
            itemList = await _context.items.FromSqlRaw("select * from items where discount = 'yes' ").ToListAsync();
            ViewData["name"] = HttpContext.Session.GetString("name");
            return View(itemList);
        }
        public async Task<IActionResult> adminHome()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            ViewData["name"] = HttpContext.Session.GetString("name");
            return View();
        }
        public async Task<IActionResult> email()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            return View();
        }
      


        [HttpPost, ActionName("email")]
        [ValidateAntiForgeryToken]
        public IActionResult email(string address, string subject, string body)
        {
            // i deleted the email info of mine, so in order to make it works you should add yours 

            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            var mail = new MailMessage();
            mail.From = new MailAddress("example@gmail.com");
            mail.To.Add(address); // receiver email address
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;
            SmtpServer.Port = 587;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential("example@gmail.com", "somecode");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            ViewData["Message"] = "Email sent.";
            return View();

        }
        public IActionResult logout() {
            HttpContext.Session.Remove("id");
            HttpContext.Session.Remove("role");
            HttpContext.Session.Remove("name");
            HttpContext.Response.Cookies.Delete("name");
            HttpContext.Response.Cookies.Delete("role");
            HttpContext.Response.Cookies.Delete("id");
            return RedirectToAction("login","home");   
        }



        private bool allusersExists(int id)
        {
          return (_context.allusers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
