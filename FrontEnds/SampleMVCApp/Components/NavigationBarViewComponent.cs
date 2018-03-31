using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SampleMVCApp.Domain;
using SampleMVCApp.Models;
using SampleMVCApp.Services;

namespace SampleMVCApp.Components
{
    public class NavigationBarViewComponent : ViewComponent
    {
        private IMenuService _menuService;

        public NavigationBarViewComponent(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<Claim> claims)
        {
            var menus = _menuService.GetMenusByClaims(claims);
            return View(menus.ToMenuViewModels());
        }
    }
}
