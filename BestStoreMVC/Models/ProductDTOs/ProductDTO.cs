using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BestStoreMVC.Models.ProductDTOs
{
    public class ProductDTO
    {
        [Required]
        public string Name { get; set; } = "";
        [Required]

        public string Brand { get; set; } = "";
        [Required]

        public string Category { get; set; } = "";

        [Required]
        public decimal Price { get; set; }
        [Required]

        public string Description { get; set; } = "";
        public IFormFile ImageFileName { get; set; } 
    }
}
