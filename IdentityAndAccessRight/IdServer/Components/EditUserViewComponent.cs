using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdServer.Components
{
    public class EditUserViewComponent : ViewComponent
    {
        private RoleManager<IdentityRole> _roleManager;

        public EditUserViewComponent(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(EditUserViewModel model = null)
        {
            //return View(model);
            return View(model is CreateUserViewModel ? "Create" : "Update", model);
        }

        //private List<CheckboxViewModel> GetRolesCheckboxViewModel(IEnumerable<string> currentRoles = null)
        //{
        //    return (from itm in _roleManager.Roles
        //            select new CheckboxViewModel
        //            {
        //                Checked = (currentRoles ?? new string[] { }).Any(currentRole => currentRole.Equals(itm.Name)),
        //                Value = itm.Name,
        //                Text = itm.Name
        //            }).ToList();
        //}
    }
}
