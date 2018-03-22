using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleMVCApp.Domain
{
    public class MenuDTO
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Claims { get; set; }
        public int Order { get; set; }
        public int ParentMenuKey { get; set; }
        public bool Visible { get; set; }
    }
}
