using System;
using System.Collections.Generic;
using Common.FrontEnd;
using Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SampleMVCApp.Domain;
using SampleMVCApp.Models;
using SampleMVCApp.Services;

namespace SampleMVCApp.Controllers
{
    [Authorize(Policy = ClaimConstants.PolicyPrefix + "." + Services.Constants.Identity + ".Actions.Menu")]
    public class MenuController : BaseController
    {
        private IMenuService _mnuService;
        private IStringLocalizer<MenuController> _localizer;

        public MenuController(IMenuService mnuService, IStringLocalizer<MenuController> localizer)
        {
            _mnuService = mnuService;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            List<MenuDTO> menus = _mnuService.GetAll();
            return View(menus.ToMenuViewModels(true));
        }

        public IActionResult Create()
        {
            return ViewComponent("EditMenu", new CreateMenuViewModel(_mnuService.GetAll().ToMenuViewModels()));
        }

        [HttpPost]
        public IActionResult Create(CreateMenuViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _mnuService.Create(new MenuDTO { Name = model.Name, Order = model.Order, Url = model.Url, ParentMenuKey = model.ParentKey, Visible = true });

                    return Content(Url.Action(nameof(Index)));
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                return BadRequest(GetModelError());
            }
        }

        public IActionResult Edit(int key = 0)
        {
            if (key <= 0)
            {
                return BadRequest(String.Format(_localizer[ErrorConstants.ARGUMENT_IS_MISSING], "Target menu"));
            }

            MenuDTO menu = _mnuService.FindByKey(key);
            if (menu == null)
            {
                return BadRequest(String.Format(_localizer[ErrorConstants.MENU_NOT_EXISTED]));
            }

            UpdateMenuViewModel model = new UpdateMenuViewModel(_mnuService.GetAll().ToMenuViewModels(true));
            model.Key = menu.Key;
            model.Name = menu.Name;
            model.Url = menu.Url;
            model.Order = menu.Order;
            model.ParentKey = menu.ParentMenuKey;
            model.Visible = menu.Visible;

            return ViewComponent("EditMenu", model);
        }

        [HttpPost]
        public IActionResult Edit(UpdateMenuViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _mnuService.Update(model.Key, model.Name, model.Url, model.Visible, model.ParentKey);

                    return Content(Url.Action(nameof(Index)));
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                return BadRequest(GetModelError());
            }
        }

        [HttpPost]
        public IActionResult Visibility(int key = 0, bool visible = true)
        {
            if (key <= 0)
            {
                return BadRequest(String.Format(_localizer[ErrorConstants.ARGUMENT_IS_MISSING], "Target menu"));
            }

            try
            {
                _mnuService.SetVisibility(key, visible);

                return Content(Url.Action(nameof(Index)));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Delete(int key = 0)
        {
            if (key <= 0)
            {
                return BadRequest(String.Format(_localizer[ErrorConstants.ARGUMENT_IS_MISSING], "Target menu"));
            }

            try
            {
                _mnuService.Delete(key);

                return Content(Url.Action(nameof(Index)));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Move(int key = 0, int step = 0)
        {
            if (key <= 0)
            {
                return BadRequest(String.Format(_localizer[ErrorConstants.ARGUMENT_IS_MISSING], "Target menu"));
            }

            try
            {
                _mnuService.Move(key, step);

                return Content(Url.Action(nameof(Index)));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}