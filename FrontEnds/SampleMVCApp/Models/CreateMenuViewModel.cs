using System.Collections.Generic;

namespace SampleMVCApp.Models
{
    public class CreateMenuViewModel : EditMenuViewModel
    {
        public CreateMenuViewModel()
        {

        }

        public CreateMenuViewModel(List<MenuViewModel> menus)
            :base(menus)
        {

        }
    }
}
