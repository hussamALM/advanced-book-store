using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    public class orders
    {
        public int Id { get; set; }
        public int itemid { get; set; }
        public int userid { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public DateTime buyDate { get; set; }
        public int quantity { get; set; }


    }
}
