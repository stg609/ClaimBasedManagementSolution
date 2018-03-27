using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Infra;
using Constants;
using IdServer.Domain;
using IdServer.Models;
using IdServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdServer.Controllers
{
    [Authorize(Policy = ClaimConstants.PolicyPrefix + "." + IdServerConstants.ID_SERVER_IDENTITY + ".Users")]
    public class UsersController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IUserService _userService;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
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
                              model.UserName);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    AddErrors(ex.Message);
                    ViewBag.Users = GetAllUsers();
                    return View(nameof(Index), model);
                }
            }
            else
            {
                ViewBag.Users = GetAllUsers();
                return View(nameof(Index), model);
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
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new System.Exception("User doesn't existed");
            }

            return ViewComponent("EditUser", new UpdateUserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Roles = GetRolesCheckboxViewModel(_userManager.GetRolesAsync(user).Result.ToList()),
                Claims = GetClaimsCheckboxViewModel(_userManager.GetClaimsAsync(user).Result.Where(itm => itm.Type.Equals(ClaimConstants.PermissionClaimType)).ToList())
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.UpdateAsync(model.Email, model.Roles?.Where(itm => itm.Checked).Select(itm => itm.Value), model.Claims?.Where(itm => itm.Checked).Select(itm => itm.Value), model.Password);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    AddErrors(ex.Message);
                    ViewBag.Users = GetAllUsers();
                    return View(nameof(Index), model);
                }
            }
            else
            {
                ViewBag.Users = GetAllUsers();
                return View(nameof(Index), model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Lock(string email)
        {
            try
            {
                await _userService.LockAsync(email);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
                ViewBag.Users = GetAllUsers();
                return View(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UnLock(string email)
        {
            try
            {
                await _userService.UnLockAsync(email);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
                ViewBag.Users = GetAllUsers();
                return View(nameof(Index));
            }
        }

        #region Private Methods
        private List<UserViewModel> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var userVMs = from user in users
                          select new UserViewModel
                          {
                              UserName = user.UserName,
                              Email = user.Email,
                              EmailConfirmed = user.EmailConfirmed,
                              Locked = IsLocked(user),
                              Roles = _userManager.GetRolesAsync(user).Result.ToList(),
                              Claims = _userManager.GetClaimsAsync(user).Result.Where(itm => itm.Type.Equals(ClaimConstants.PermissionClaimType)).Select(itm => itm.Value).ToList()
                          };
            return userVMs.EmptyListIfEmpty();
        }

        private bool IsLocked(ApplicationUser user)
        {
            return user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now;
        }


        #endregion

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private void AddErrors(string errMessage)
        {
            ModelState.AddModelError(string.Empty, errMessage);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

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

        private List<CheckboxViewModel> GetClaimsCheckboxViewModel(IEnumerable<Claim> currentClaims = null)
        {
            //return (from itm in _claimManager.GetAllClaims()
            //        select new CheckboxViewModel
            //        {
            //            Checked = (currentClaims ?? new Claim[] { }).Any(currentClaim => currentClaim.Type.Equals(itm.Type) && currentClaim.Value.Equals(itm.Value)),
            //            Value = itm.Value,
            //            Text = itm.Value
            //        }).ToList();

            return null;
        }
        #endregion
    }
}