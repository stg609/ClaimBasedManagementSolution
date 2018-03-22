using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleMVCApp.Models
{
    public class MenuViewModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public int Key { get; set; }
        public bool Visible { get; set; }

        public List<MenuViewModel> Children { get; set; } = new List<MenuViewModel>();
    }
}
