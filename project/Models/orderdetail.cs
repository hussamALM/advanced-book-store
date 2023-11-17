using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    public class orderdetail
    {
        public int Id { get; set; }
        public string name { get; set; }
        public int totalPrice { get; set; }
        public int quantity { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public DateTime buyDate { get; set; }

    }
}
