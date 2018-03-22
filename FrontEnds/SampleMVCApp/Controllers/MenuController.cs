using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SampleMVCApp.Domain;
using SampleMVCApp.Models;
using SampleMVCApp.Services;

namespace SampleMVCApp.Controllers
{
    public class MenuController : Controller
    {
        private MenuService _mnuService;

        public MenuController(MenuService mnuService)
        {
            _mnuService = mnuService;
        }

        public IActionResult Index()
        {
            List<MenuDTO> menus = _mnuService.GetMenusByClaims(User.Claims);

            List<MenuViewModel> results = new List<MenuViewModel>();
            foreach (var menu in menus.Where(itm=>itm.Visible).OrderBy(itm => itm.ParentMenuKey).ThenBy(itm => itm.Order))
            {
                if (menu.ParentMenuKey == 0)
                {
                    results.Add(new MenuViewModel { Key = menu.Key, Name = menu.Name, Order = menu.Order, Url = menu.Url });
                }
                else
                {
                    var parent = Single(results, itm => itm.Key == menu.ParentMenuKey);
                    parent.Children.Add(new MenuViewModel { Key = menu.Key, Name = menu.Name, Order = menu.Order, Url = menu.Url });
                }
            }

            return PartialView(results);
        }

        private MenuViewModel Single(List<MenuViewModel> menus, Func<MenuViewModel, bool> predicate)
        {
            if (menus == null)
            {
                return null;
            }

            MenuViewModel result = menus.SingleOrDefault(predicate);

            menus.ForEach(itm =>
            {
                var menu = Single(itm.Children, predicate);
                if (menu != null && result != null)
                {
                    throw new ArgumentException();
                }

                result = menu;
            });
            return result;
        }
    }
}