using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SampleMVCApp.Models
{
    public class UpdateMenuViewModel : EditMenuViewModel
    {
        [Required]
        public int Key { get; set; }
        public bool Visible { get; set; }

        public UpdateMenuViewModel()
        {

        }

        public UpdateMenuViewModel(List<MenuViewModel> menus)
            :base(menus)
        {

        }
    }
}
