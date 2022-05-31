using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Product : BaseEntity
    {
        [MaxLength(255)]
        [Required(ErrorMessage = "Title cannot be empty!")]
        public string Title { get; set; }

        [Range(0, 250000, ErrorMessage = "Price must be under 250000 and over 0!")]
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ProductImages> ProductImages { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
