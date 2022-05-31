using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Expert : BaseEntity
    {
        public string FullName { get; set; }
        public string Profession { get; set; }
        public string ImageUrl { get; set; }
    }
}
