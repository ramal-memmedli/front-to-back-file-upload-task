using Data.Models;
using System.Collections.Generic;

namespace FrontToBack.ViewModel
{
    public class ProductsVM
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
    }
}
