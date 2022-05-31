using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class AboutListItem : BaseEntity
    {
        public string Content { get; set; }
        public int AboutId { get; set; }
        public About About { get; set; }
    }
}
