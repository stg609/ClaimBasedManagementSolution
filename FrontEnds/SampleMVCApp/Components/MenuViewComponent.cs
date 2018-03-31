using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SampleMVCApp.Models;

namespace SampleMVCApp.Components
{
    public class MenuViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<MenuViewModel> menus, bool isChildLevel = false)
        {
            ViewBag.IsChildLevel = isChildLevel;
            return View(menus);
        }
    }
}
