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
    }
}
