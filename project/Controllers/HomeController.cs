using Microsoft.AspNetCore.Mvc;
using project.Models;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace project.Controllers

{


    /*
     if (HttpContext.Session.GetString("name")==null)
                return RedirectToAction("login", "home");
    
    if (HttpContext.Session.GetString("role")!="admin")
                return RedirectToAction("notHere", "home");

     
     
     */
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
      

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult login()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("name") && HttpContext.Session.GetString("name") == null)
                return View();
            string role;
            if (HttpContext.Session.GetString("name")==null) {
                string name = HttpContext.Request.Cookies["name"].ToString();
                 role = HttpContext.Request.Cookies["role"].ToString();
                string id = HttpContext.Request.Cookies["id"].ToString();
                HttpContext.Session.SetString("name", name);
                HttpContext.Session.SetString("role", role);
                HttpContext.Session.SetString("id", id);
            }
             role = HttpContext.Session.GetString("role");
            

            if (role == "customer")
            {
                return RedirectToAction("customerhome", "allusers");
            }
            return RedirectToAction("adminhome", "allusers");
        }
        [HttpPost]
        public IActionResult login(string username, string password,bool remember)
        {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("projectContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "select * from allusers";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                if (username == (string)reader["name"])
                {
                    if (password != (string)reader["password"])
                    {
                        ViewData["err"] = "the password is not correct";
                        return View("login");
                    }
                    else
                    {
                        string name = (string)reader["name"];
                        int id = (int)reader["id"];
                        string role=(string)reader["role"];
                        HttpContext.Session.SetString("role", role);
                        HttpContext.Session.SetString("id", Convert.ToString(id));
                        HttpContext.Session.SetString("name",name);
                        if (remember==true) {
                            var cookieOptions = new CookieOptions
                            { Expires = DateTime.Now.AddDays(30) }; 
                            HttpContext.Response.Cookies.Append("name", name, cookieOptions);
                            HttpContext.Response.Cookies.Append("role", role, cookieOptions);
                            HttpContext.Response.Cookies.Append("id", Convert.ToString(id), cookieOptions);

                        }
                        if (role=="customer") {
                            return RedirectToAction("customerhome","allusers");
                        }
                        return RedirectToAction("adminhome", "allusers");
                    }
                }
            }
            reader.Close();
            conn.Close();
            ViewData["err"] = "the user was not found";
            return View("login");
        }
        public IActionResult registration() {
            return View();
        }
        [HttpPost]
        public IActionResult registration(string username, string password,string password2)
        {
            if (password!=password2) {
                ViewData["err"] = "the two password do not match";
                return View("registration");

            }
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("projectContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "select * from allusers";
            SqlCommand comm = new SqlCommand(sql,conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                if (username == (string)reader["name"]) {
                    ViewData["err"] = "the username is already used";
                    return View("registration");
                }
            }
            reader.Close();
            sql = "insert into allusers(name,password,role) values('" + username + "', '" + password + "' , 'customer')";
            comm = new SqlCommand(sql, conn);
            comm.ExecuteNonQuery();
            conn.Close();
            return View("login");
        }
        public IActionResult notHere()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}