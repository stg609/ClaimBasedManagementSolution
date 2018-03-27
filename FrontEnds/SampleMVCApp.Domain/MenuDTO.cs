using Common.Domain;

namespace SampleMVCApp.Domain
{
    public class MenuDTO : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Claims { get; set; }
        public int Order { get; set; }
        public int ParentMenuKey { get; set; }
        public bool Visible { get; set; }
    }
}
