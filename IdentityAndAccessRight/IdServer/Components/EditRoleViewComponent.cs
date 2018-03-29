using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdServer.Components
{
    public class EditRoleViewComponent : ViewComponent
    {
        private RoleManager<IdentityRole> _roleManager;

        public EditRoleViewComponent(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(EditRoleViewModel model = null)
        {
            //return View(model);
            return View(model is CreateRoleViewModel ? "Create" : "Update", model);
        }
    }
}
