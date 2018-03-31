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
    public class EditMenuViewComponent : ViewComponent
    {
        private IMenuService _menuService;

        public EditMenuViewComponent(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<IViewComponentResult> InvokeAsync(EditMenuViewModel model)
        {
            //var exsited = _menuService.GetAll().ToMenuViewModels();
            //bool isUpdate = false;

            //if (model != null && model is UpdateMenuViewModel)
            //{
            //    isUpdate = true;
            //}          
            //else
            //{
            //    model = new CreateMenuViewModel(exsited);
            //}

            return View(model is UpdateMenuViewModel ? "Update" : "Create", model);
        }
    }
}
