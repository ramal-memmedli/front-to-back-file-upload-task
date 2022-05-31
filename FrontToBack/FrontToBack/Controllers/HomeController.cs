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
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context; 
        }

        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM();
            homeVM.Sliders = await _context.Sliders.Where(n => !n.IsDeleted).ToListAsync();
            homeVM.Products = await _context.Products
                .Where(n => !n.IsDeleted)
                .Include(n => n.Category)
                .Include(n => n.ProductImages)
                .ThenInclude(n => n.Image)
                .Take(8).OrderByDescending(n => n.CreatedDate).ToListAsync();
            homeVM.Categories = await _context.Categories.Where(n => !n.IsDeleted).ToListAsync();
            homeVM.About = await _context.About.Where(n => !n.IsDeleted).FirstOrDefaultAsync();
            homeVM.AboutListItems = await _context.AboutListItems.Where(n => !n.IsDeleted).ToListAsync();
            homeVM.Experts = await _context.Experts.Where(n => !n.IsDeleted).ToListAsync();
            homeVM.BlogCards = await _context.BlogCards.Where(n => !n.IsDeleted).ToListAsync();
            homeVM.Testimonials = await _context.Testimonials.Where(n => !n.IsDeleted).ToListAsync();
            homeVM.InstagramPosts = await _context.InstagramPosts.Where(n => !n.IsDeleted).ToListAsync();
            return View(homeVM);
        }
    }
}
