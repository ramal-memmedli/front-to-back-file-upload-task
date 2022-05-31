using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<About> About { get; set; }
        public DbSet<AboutListItem> AboutListItems { get; set; }
        public DbSet<Expert> Experts { get; set; }
        public DbSet<BlogCard> BlogCards { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<InstagramPost> InstagramPosts { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
    }
}
