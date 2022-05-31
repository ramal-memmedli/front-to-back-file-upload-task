using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class About : BaseEntity
    {
        public string VideoThumbnailUrl { get; set; }
        public string Header { get; set; }
        public string Paragraph { get; set; }
        public List<AboutListItem> AboutListItems { get; set; }
    }
}
