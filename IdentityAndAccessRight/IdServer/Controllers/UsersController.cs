using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Domain;
using Common.FrontEnd;
using Common.Infra;
using Common.Constants;
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
    [Authorize(Policy = ClaimConstants.PolicyPrefix + "." + IdServerConstants.ID_SERVER_IDENTITY + ".Users")]
    public class UsersController : BaseController
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IUserService _userService;
        private IClaimService _claimService;
        private IStringLocalizer<UsersController> _localizer;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserService userService, IClaimService claimService, IStringLocalizer<UsersController> localizer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _claimService = claimService;

            _localizer = localizer;
        }

        //Get all Users
        public IActionResult Index()
        {
            //返回 List，保证已经执行完数据库查询，从而避免后续数据库查询导致嵌套查询的问题
            ViewBag.Users = GetAllUsers();
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return ViewComponent("EditUser", new CreateUserViewModel
            {
                Roles = GetRolesCheckboxViewModel(),
                Claims = GetClaimsCheckboxViewModel()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.CreateAsync(model.Email,
                              model.Password,
                              model.Roles?.Where(itm => itm.Checked).Select(itm => itm.Value),
                              model.Claims?.Where(itm => itm.Checked).Select(itm => itm.Value),
                              model.Nickname);

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

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return Json($"Email {email} is already in use.");
            }

            return Json(true);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                return BadRequest(String.Format(_localizer[ErrorConstants.ARGUMENT_IS_MISSING], "Target email address"));
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(String.Format(_localizer[ErrorConstants.USER_NOT_EXISTED]));
            }

            return ViewComponent("EditUser", new UpdateUserViewModel
            {
                Nickname = user.Nickname,
                Email = user.Email,
                Roles = GetRolesCheckboxViewModel(_userManager.GetRolesAsync(user).Result.ToList()),
                Claims = GetClaimsCheckboxViewModel(_userManager.GetClaimsAsync(user).Result.SingleOrDefault(itm => itm.Type.Equals(ClaimConstants.PermissionClaimType)))
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.UpdateAsync(model.Email, model.Roles?.Where(itm => itm.Checked).Select(itm => itm.Value), model.Claims?.Where(itm => itm.Checked).Select(itm => itm.Value), model.Password);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                return BadRequest(String.Format(_localizer[ErrorConstants.ARGUMENT_IS_MISSING], "Target email address"));
            }

            try
            {
                await _userService.LockAsync(email);
                return Content(Url.Action(nameof(Index)));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnLock(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                return BadRequest(String.Format(_localizer[ErrorConstants.ARGUMENT_IS_MISSING], "Target email address"));
            }

            try
            {
                await _userService.UnLockAsync(email);
                return Content(Url.Action(nameof(Index)));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #region Private Methods
        private List<UserViewModel> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var userVMs = from user in users
                          select new UserViewModel
                          {
                              Email = user.Email,
                              EmailConfirmed = user.EmailConfirmed,
                              Nickname = user.Nickname,
                              Locked = IsLocked(user),
                              Roles = _userManager.GetRolesAsync(user).Result.ToList(),
                              Claims = _userManager.GetClaimsAsync(user).Result.SingleOrDefault(itm => itm.Type.Equals(ClaimConstants.PermissionClaimType))?.Value.Split(GeneralConstants.DelimeterSemicolon).ToList()
                          };
            return userVMs.EmptyListIfEmpty();
        }

        private bool IsLocked(ApplicationUser user)
        {
            return user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now;
        }


        #endregion

        #region Helpers       

        private List<CheckboxViewModel> GetRolesCheckboxViewModel(IEnumerable<string> currentRoles = null)
        {
            return (from itm in _roleManager.Roles
                    select new CheckboxViewModel
                    {
                        Checked = (currentRoles ?? new string[] { }).Any(currentRole => currentRole.Equals(itm.Name)),
                        Value = itm.Name,
                        Text = itm.Name
                    }).ToList();
        }

        private List<CheckboxViewModel> GetClaimsCheckboxViewModel(Claim permissionClaims = null)
        {
            return (from itm in _claimService.GetAllClaims()
                    select new CheckboxViewModel
                    {
                        Checked = permissionClaims.HasPermissionClaim(itm.Value),
                        Value = itm.Value,
                        Text = itm.Value
                    })?.ToList();
        }
        #endregion
    }
}