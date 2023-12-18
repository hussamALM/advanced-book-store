using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project.Models;
using System.Data.SqlClient;

namespace project.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        [HttpGet("{role}")]
        public IEnumerable<allusers> Get(string role) {
            List<allusers> li = new List<allusers>();
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("projectContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "select * from allusers where role='"+role+"'";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                li.Add(new allusers
                {
                    name = (string)reader["name"]
                });
            }
            reader.Close();
            conn.Close();
            return li;
        }
        [HttpPost]
        public string Post([FromBody] apiOrder order)
        {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("projectContext");
            SqlConnection conn = new SqlConnection(conStr);
            int userid =(int)order.userId;
            int quantity=0;
            int itemid=0;
            for (int i = 0; i < order.books.Length; i++)
            {
                for (int j = 0; j < order.books[0].Length; j++)
                {
                    if (j == 0)
                    {
                        itemid = Convert.ToInt16(order.books[i][j]);
                    }
                    else {
                        quantity = Convert.ToInt16(order.books[i][j]);
                    }
                }
                string sql = "insert into orders(userid,itemid,quantity) values('" + userid + "','" + itemid + "','" + quantity + "')";
                SqlCommand comm = new SqlCommand(sql, conn);
                conn.Open();
                comm.ExecuteNonQuery();
                sql = "update items set quantity = quantity -'" + quantity + "' where Id = '" + itemid + "' ";
                comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();
                conn.Close();
            }
            return ("Done");
        }

    }
}
/* for (int i = 0; i < order.books[0].Length;i++ ) {
                for (int j = 0; i < order.books[0].Length; j++)
                {
                    msg +=" "+ order.books[i][j];
                }
            }
 msg += " " + order.books.Length;
            msg += " " + order.books[0].Length;
            msg += " " + order.books[0][0].Length;
            msg += " " + order.books[0][0].Length;*/