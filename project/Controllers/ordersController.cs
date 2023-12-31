﻿using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project.Models;
using System.Data.SqlClient;
using System.Diagnostics;

namespace project.Controllers
{
    public class ordersController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
             return RedirectToAction("notHere", "home");      
        }
        public IActionResult orderAcceptance()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("projectContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "select * from globalConfig where name='acceptNewOrder'";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            reader.Read();
            ViewData["status"] = (int)reader["value"] == 1 ? "on" : "off";
            conn.Close();
            return View();
        }
        public IActionResult serverOn()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("projectContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "update globalConfig set value=1 where name='acceptNewOrder'";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            comm.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("orderAcceptance");
        }
        public IActionResult serverOff()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("projectContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "update globalConfig set value=0 where name='acceptNewOrder'";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            comm.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("orderAcceptance");
        }
        public IActionResult orderDetails(int? id) {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            List<orderdetail> orders = new List<orderdetail>();
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("projectContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "select allusers.name as username, orders.Id as id, items.name as name, items.name as title, orders.quantity as quantity, orders.buyDate as buyDate,items.price as price from allusers,orders,items where orders.itemid = items.id and orders.userid  = allusers.Id and orders.userid=" + id+ " order by buyDate desc";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                orders.Add(new orderdetail
                {
                    Id = (int)reader["id"],
                    name = (string)reader["name"],
                    totalPrice = (int)reader["price"] * (int)reader["quantity"],
                    quantity = (int)reader["quantity"],
                    buyDate = (DateTime)reader["buyDate"]



                });
                ViewData["name"] = (string)reader["username"];

            }
            reader.Close();
            return View(orders);
        }
      
        public IActionResult buy(int id) {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "customer")
                return RedirectToAction("notHere", "home");

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("projectContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "select * from items where Id=" + id;
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            items item = new items();
            if (reader.Read()) {
                item.Id = (int)reader["Id"];
                item.name = (string)reader["name"];
                item.price = (int)reader["price"];
                item.discount = (string)reader["discount"];
                item.category = (string)reader["category"];
                item.imagefilename = (string)reader["imagefilename"];
                item.quantity = (int)reader["quantity"];
            }
            reader.Close();

             sql = "select * from globalConfig where name='acceptNewOrder'";
             comm = new SqlCommand(sql, conn);
             reader = comm.ExecuteReader();
            reader.Read();
            if ((int)reader["value"] != 1)
            {
                return RedirectToAction("noOrder", "Home"); ;
            }
            if (HttpContext.Session.GetString("noQuantity") != null)
            {
                ViewData["msg"] = "you cannot order more than "+item.quantity;
                HttpContext.Session.Remove("noQuantity");
            }

            return View(item);
        }

        [HttpPost]
        public IActionResult buy([Bind("Id,quantity")] orders order, int oldQuantity) {
            if (order.quantity>oldQuantity) {
                HttpContext.Session.SetString("noQuantity","the order is more than the quantity available");
                return RedirectToAction("buy", new { id = order.Id });
            }

            int userid = Convert.ToInt32(HttpContext.Session.GetString("id"));
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("projectContext");
            SqlConnection conn = new SqlConnection(conStr);
            /*
           if (order.quantity < oldQuantity)
           {
                sql = "select * from items where Id=" + order.Id;
                comm = new SqlCommand(sql, conn);
               conn.Open();
               SqlDataReader reader = comm.ExecuteReader();
               items item = new items();
               if (reader.Read())
               {
                   item.Id = (int)reader["Id"];
                   item.name = (string)reader["name"];
                   item.price = (int)reader["price"];
                   item.discount = (string)reader["discount"];
                   item.category = (string)reader["category"];
                   item.imagefilename = (string)reader["imagefilename"];
                   item.quantity = (int)reader["quantity"];
               }
               reader.Close();
               ViewData["msg"] = "you cannot order more than " + item.quantity;
               return View(item);

           } */
            string sql = "insert into orders(userid,itemid,quantity) values('" + userid + "','" + order.Id + "','" + order.quantity + "')";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            comm.ExecuteNonQuery();
            sql = "update items set quantity = quantity -'" + order.quantity + "' where Id = '" + order.Id + "' ";
            comm = new SqlCommand(sql, conn);
            comm.ExecuteNonQuery();
            conn.Close();
            HttpContext.Session.SetString("orderComplete", "done");
            return RedirectToAction("myPurchase", "orders");
        }
        public IActionResult myPurchase() {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "customer")
                return RedirectToAction("notHere", "home");
            int userid = Convert.ToInt32(HttpContext.Session.GetString("id"));
            List<orderdetail> orders = new List<orderdetail>();
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("projectContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "select orders.Id as id, items.name as name, items.name as title, orders.quantity as quantity, orders.buyDate as buyDate,items.price as price from allusers,orders,items where orders.itemid = items.id and orders.userid  = allusers.Id and orders.userid=" + userid+ " order by buyDate desc";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();

            while (reader.Read())
            {
                orders.Add(new orderdetail
                {
                    name = (string)reader["name"],
                    totalPrice = (int)reader["price"] * (int)reader["quantity"],
                    quantity = (int)reader["quantity"],
                    buyDate = (DateTime)reader["buyDate"]



                }
                    );
            }
            if (HttpContext.Session.GetString("orderComplete") != null) {
                ViewData["msg"] = "your order is complete !";
                HttpContext.Session.Remove("orderComplete");
            }
            return View(orders);
        }
        public IActionResult purchaseReport() {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("login", "home");
            if (HttpContext.Session.GetString("role") != "admin")
                return RedirectToAction("notHere", "home");
            List<report> reports = new List<report>();
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("projectContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "select allusers.id as Id, allusers.name as customername, sum (orders.quantity * price)  as total from items, orders , allusers where  itemid= items.Id and userid= allusers.Id group by allusers.Id, allusers.name";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                reports.Add(new report
                {
                    Id =(int)reader["Id"],
                    customername = (string)reader["customername"],
                    total = (int)reader["total"]
                }
                    );
            }
            return View(reports);
        }
    }
}
