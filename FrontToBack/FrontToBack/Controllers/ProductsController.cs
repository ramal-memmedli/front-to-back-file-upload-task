using Data.DAL;
using Data.Models;
using FrontToBack.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        ProductsVM productsVM = new ProductsVM();

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            

            productsVM.Products = await _context.Products.Where(n => !n.IsDeleted)
                                                  .Include(n => n.Category)
                                                  .Include(n => n.ProductImages)
                                                  .ThenInclude(n => n.Image)
                                                  .Take(4).ToListAsync();

            productsVM.Categories = await _context.Categories.Where(n => !n.IsDeleted).ToListAsync();

            return View(productsVM);
        }

        [Route("{controller}/loadmore/{skip}")]
        public async Task<IActionResult> LoadMore(int skip)
        {
            productsVM.Products = await _context.Products.Where(n => !n.IsDeleted)
                                                  .Include(n => n.Category)
                                                  .Include(n => n.ProductImages)
                                                  .ThenInclude(n => n.Image)
                                                  .Skip(skip)
                                                  .Take(4).ToListAsync();
            return PartialView("_ProductPartial", productsVM);
        }

        public async Task<IActionResult> AllProductCount()
        {
            int productCount = await _context.Products.Where(n => !n.IsDeleted).CountAsync();

            return Json(productCount);
        }
    }
}
