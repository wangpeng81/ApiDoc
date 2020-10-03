using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.Models
{
    public class TreeViewItem
    {
        public string text { get; set; }
        public string href { get; set; } 
        public List<object> tags { get; set; }
        public object data { get; set; }
        public List<TreeViewItem> nodes { get; set; }
    }
}
