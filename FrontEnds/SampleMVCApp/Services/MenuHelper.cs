using System;
using System.Collections.Generic;
using System.Linq;
using SampleMVCApp.Domain;
using SampleMVCApp.Models;

namespace SampleMVCApp.Services
{
    public static class MenuHelper
    {
        public static List<MenuViewModel> ToMenuViewModels(this IEnumerable<MenuDTO> menus, bool showHiddenMenus = false)
        {
            List<MenuViewModel> results = new List<MenuViewModel>();
            foreach (var menu in menus.Where(itm => showHiddenMenus || itm.Visible).OrderBy(itm => itm.ParentMenuKey).ThenBy(itm => itm.Order))
            {
                if (menu.ParentMenuKey == 0)
                {
                    results.Add(new MenuViewModel { Key = menu.Key, Name = menu.Name, Order = menu.Order, Url = menu.Url, Visible = menu.Visible });
                }
                else
                {
                    var parent = Single(results, itm => itm.Key == menu.ParentMenuKey);
                    if (parent != null)
                    {
                        //Indicate parent is invisible
                        parent.Children.Add(new MenuViewModel { Key = menu.Key, Name = menu.Name, Order = menu.Order, Url = menu.Url, Visible = menu.Visible });
                    }
                }
            }

            return results;
        }

        private static MenuViewModel Single(List<MenuViewModel> menus, Func<MenuViewModel, bool> predicate)
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
                else if (menu != null)
                {
                    result = menu;
                }
            });
            return result;
        }
    }
}
