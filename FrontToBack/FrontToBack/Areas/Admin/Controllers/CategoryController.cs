using Data.DAL;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Categories
                .Where(n => !n.IsDeleted).ToListAsync();

            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            Category category = await _context.Categories.Where(n => !n.IsDeleted && n.Id == id).FirstOrDefaultAsync();

            if(category is null)
            {
                return NotFound();
            }

            return View(category);
        }

        public async Task<IActionResult> Update(int id)
        {
            Category category = await _context.Categories.Where(n => !n.IsDeleted && n.Id == id).FirstOrDefaultAsync();

            if (category is null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, string categoryName)
        {
            Category category = await _context.Categories.Where(n => !n.IsDeleted && n.Id == id).FirstOrDefaultAsync();

            if (category is null || string.IsNullOrEmpty(categoryName.Trim()))
            {
                return NotFound();
            }

            category.Name = categoryName.Trim();
            category.UpdatedDate = DateTime.Now;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(actionName: nameof(Details), controllerName: nameof(Category), routeValues: new {id});
        }

        public async Task<IActionResult> Delete(int id)
        {
            Category category = await _context.Categories.Where(n => !n.IsDeleted && n.Id == id).FirstOrDefaultAsync();

            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Category category)
        {
            Category dbCategory = await _context.Categories.Where(n => !n.IsDeleted && n.Id == category.Id).FirstOrDefaultAsync();

            if (category is null)
            {
                return NotFound();
            }

            if(category.Name.Trim() == dbCategory.Name)
            {
                dbCategory.IsDeleted = true;
                _context.Categories.Update(dbCategory);
                await _context.SaveChangesAsync();
            }else
            {
                return Content("Delete operation cancelled. Because entered category name is wrong.");
            }

            return RedirectToAction(actionName: nameof(Index), controllerName: nameof(Category));
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if(await IsCategoryExist(category))
            {
                return Content("This category already exists.");
            }

            category.CreatedDate = DateTime.Now;
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(actionName: nameof(Index), controllerName: nameof(Category));
        }

        private async Task<bool> IsCategoryExist(Category category)
        {
            bool result = await _context.Categories.AnyAsync(n => n.Name.ToLower() == category.Name.Trim().ToLower());
            return result;
        }
    }
}
