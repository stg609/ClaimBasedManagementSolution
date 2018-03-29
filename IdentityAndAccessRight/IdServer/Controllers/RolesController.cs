using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Constants;
using IdServer.Domain;
using IdServer.Models;
using IdServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace IdServer.Controllers
{
    [Authorize(Policy = ClaimConstants.PolicyPrefix + "." + IdServerConstants.ID_SERVER_IDENTITY + ".Roles")]
    public class RolesController : BaseController
    {
        private RoleManager<IdentityRole> _roleManager;
        private IRoleService _roleService;
        private IClaimService _claimService;
        private IStringLocalizer<RolesController> _localizer;

        public RolesController(RoleManager<IdentityRole> roleManager, IRoleService roleService, IClaimService claimService, IStringLocalizer<RolesController> localizer)
        {
            _roleManager = roleManager;
            _roleService = roleService;
            _claimService = claimService;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            ViewBag.Roles = GetAllRoles();
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return ViewComponent("EditRole", new CreateRoleViewModel
            {
                Claims = GetClaimsCheckboxViewModel()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _roleService.CreateAsync(model.Name, model.Claims?.Where(itm => itm.Checked).Select(itm => itm.Value));
                    return Content(Url.Action(nameof(Index)));
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            return BadRequest(GetModelError());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                return BadRequest(String.Format(_localizer[ErrorConstants.ARGUMENT_IS_MISSING], "Target role"));
            }

            var role = await _roleManager.FindByNameAsync(name);

            if (role != null)
            {
                if (User.IsInRole(name))
                {
                    return BadRequest(String.Format(_localizer[ErrorConstants.ROLE_IS_IN_USING]));
                }

                IdentityResult result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, GetIdentityError(result));
                }
            }

            return Content(Url.Action(nameof(Index)));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                return BadRequest(String.Format(_localizer[ErrorConstants.ARGUMENT_IS_MISSING], "Target role"));
            }

            var role = await _roleManager.FindByNameAsync(name);
            if (role == null)
            {
                return BadRequest(String.Format(_localizer[ErrorConstants.ARGUMENT_IS_NOT_EXISTED], "Target role"));
            }

            return ViewComponent("EditRole", new UpdateRoleViewModel
            {
                Claims = GetClaimsCheckboxViewModel(_roleManager.GetClaimsAsync(role).Result.Where(itm => itm.Type.Equals(ClaimConstants.PermissionClaimType)).ToList())
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _roleService.UpdateAsync(model.Name, model.Claims?.Where(itm => itm.Checked).Select(itm => itm.Value));
                    return Content(Url.Action(nameof(Index)));
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }

            return BadRequest(GetModelError());
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyRoleName(string name)
        {
            var role = await _roleManager.FindByNameAsync(name);
            if (role != null)
            {
                return Json($"Role {name} is already in use.");
            }

            return Json(true);
        }

        #region Private methods

        private IEnumerable<RoleViewModel> GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();

            var roleVMs = from role in roles
                          select new RoleViewModel
                          {
                              Name = role.Name,
                              Claims = _roleManager.GetClaimsAsync(role).Result.Select(itm => itm.Value)
                          };
            return roleVMs;
        }

        private List<CheckboxViewModel> GetClaimsCheckboxViewModel(IEnumerable<Claim> currentClaims = null)
        {
            return (from itm in _claimService.GetAllClaims()
                    select new CheckboxViewModel
                    {
                        Checked = (currentClaims ?? new Claim[] { }).Any(currentClaim => currentClaim.Type.Equals(itm.Type) && currentClaim.Value.Equals(itm.Value)),
                        Value = itm.Value,
                        Text = itm.Value
                    })?.ToList();
        }
        #endregion
    }
}