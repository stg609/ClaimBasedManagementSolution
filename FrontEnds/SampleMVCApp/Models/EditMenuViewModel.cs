using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SampleMVCApp.Models
{
    public class EditMenuViewModel
    {
        [Required]
        public string Name { get; set; }
        public int Order { get; set; }
        public string Url { get; set; }
        /// <summary>
        /// Existed Menus
        /// </summary>
        public SelectList Parent { get; }
        public int ParentKey { get; set; }

        public EditMenuViewModel()
        {

        }

        public EditMenuViewModel(List<MenuViewModel> menus)
        {
            Parent = new SelectList(menus, nameof(MenuViewModel.Key), nameof(MenuViewModel.Name));
        }
    }
}
