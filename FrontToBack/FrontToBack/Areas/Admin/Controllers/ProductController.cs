using Data.DAL;
using Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public ProductController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products
                                    .Where(n => !n.IsDeleted)
                                    .Include(n => n.ProductImages)
                                    .ThenInclude(n => n.Image)
                                    .Include(n => n.Category)
                                    .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            Product product = await _context.Products
                                    .Where(n => !n.IsDeleted && n.Id == id)
                                    .Include(n => n.ProductImages)
                                    .ThenInclude(n => n.Image)
                                    .Include(n => n.Category)
                                    .FirstOrDefaultAsync();

            if (product is null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Categories"] = await GetCategories();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewData["Categories"] = await GetCategories();

            if (product.ImageFile is null)
            {
                ModelState.AddModelError("ImageFile", "Image field is required for submit!");
                return View(product);
            }

            if(!product.ImageFile.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("ImageFile", "File must be an image only!");
                return View(product);
            }

            decimal size = (decimal) product.ImageFile.Length / 1024 / 1024;

            if(size > 3)
            {
                ModelState.AddModelError("ImageFile", "File must be under 3MB");
                return View(product);
            }

            string imageName = Guid.NewGuid().ToString() + product.ImageFile.FileName;

            if (imageName.Length >= 254)
            {
                imageName = imageName.Substring(imageName.Length - 254, 254);
            }

            string path = Path.Combine(_environment.WebRootPath, "assets", "uploads", "products", imageName);

            using(FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                await product.ImageFile.CopyToAsync(fileStream);
            }

            Image image = new Image();

            image.Name = imageName;

            product.CreatedDate = DateTime.Now;

            await _context.Images.AddAsync(image);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            ProductImages productImages = new ProductImages();
            productImages.ProductId = product.Id;
            productImages.ImageId = image.Id;

            await _context.ProductImages.AddAsync(productImages);
            await _context.SaveChangesAsync();


            return RedirectToAction(actionName: nameof(Index), controllerName: nameof(Product));
        }

        public async Task<IActionResult> Update(int id)
        {
            Product product = await _context.Products
                                    .Where(n => !n.IsDeleted && n.Id == id)
                                    .Include(n => n.ProductImages)
                                    .ThenInclude(n => n.Image)
                                    .Include(n => n.Category)
                                    .FirstOrDefaultAsync();

            if(product is null)
            {
                return NotFound();
            }

            ViewData["Categories"] = await GetCategories();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Product product)
        {
            ViewData["Categories"] = await GetCategories();

            Product dbProduct = await _context.Products
                                    .Where(n => !n.IsDeleted && n.Id == id)
                                    .Include(n => n.ProductImages)
                                    .ThenInclude(n => n.Image)
                                    .Include(n => n.Category)
                                    .FirstOrDefaultAsync();

            if (product is null)
            {
                return NotFound();
            }

            if(product.ImageFile != null)
            {
                decimal size = (decimal)product.ImageFile.Length / 1024 / 1024;

                if (!product.ImageFile.ContentType.Contains("image/") ){
                    ModelState.AddModelError("ImageFile", "File must be an image only!");
                    return View(product);
                }

                if (size > 3)
                {
                    ModelState.AddModelError("ImageFile", "File must be under 3MB!");
                    return View(product);
                }

                string fileName = Guid.NewGuid().ToString() + product.ImageFile.FileName;

                

                if(fileName.Length >= 254)
                {
                    fileName = fileName.Substring(fileName.Length - 254, 254);
                }

                string path = Path.Combine(_environment.WebRootPath, "assets", "uploads", "products", fileName);

                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(fileStream);
                }

                Image image = new Image();
                image.Name = fileName;

                await _context.Images.AddAsync(image);
                await _context.SaveChangesAsync();

                ProductImages productImages = await _context.ProductImages.Where(n => n.ProductId == dbProduct.Id).FirstOrDefaultAsync();

                Image dbImage = await _context.Images.Where(n => n.Id == productImages.ImageId).FirstOrDefaultAsync();

                _context.ProductImages.Remove(productImages);
                await _context.SaveChangesAsync();
                _context.Images.Remove(dbImage);

                ProductImages newProductImages = new ProductImages();

                newProductImages.ImageId = image.Id;
                newProductImages.ProductId = dbProduct.Id;

                await _context.ProductImages.AddAsync(newProductImages);
                await _context.SaveChangesAsync();
            }

        
            dbProduct.CategoryId = product.CategoryId == 0 ? dbProduct.CategoryId : product.CategoryId;
            dbProduct.Title = product.Title;
            dbProduct.Price = product.Price;

            _context.Products.Update(dbProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction(actionName: nameof(Index), controllerName: nameof(Product));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Product product = await _context.Products.Where(n => !n.IsDeleted && n.Id == id).FirstOrDefaultAsync();

            if (product is null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Product product)
        {
            Product dbProduct = await _context.Products.Where(n => !n.IsDeleted && n.Id == product.Id).FirstOrDefaultAsync();

            if (product is null)
            {
                return NotFound();
            }

            if (product.Title.Trim() == dbProduct.Title)
            {
                dbProduct.IsDeleted = true;
                _context.Products.Update(dbProduct);
                await _context.SaveChangesAsync();
            }
            else
            {
                return Content("Delete operation cancelled. Because entered product name is wrong.");
            }

            return RedirectToAction(actionName: nameof(Index), controllerName: nameof(Product));
        }

        private async Task<List<Category>> GetCategories()
        {
            List<Category> categories = await _context.Categories.Where(n => !n.IsDeleted).ToListAsync();
            return categories;
        }
    }
}
