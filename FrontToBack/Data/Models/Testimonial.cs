using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Testimonial : BaseEntity
    {
        public string FullName { get; set; }
        public string Profession { get; set; }
        public string ImageUrl { get; set; }
        public string Paragraph { get; set; }
    }
}
