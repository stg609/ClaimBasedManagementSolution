using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Constants;
using Domain;
using Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SampleMVCApp.Domain
{
    public class MenuService
    {
        private static readonly object _lockObj = new object();

        private IUrlHelper _url;
        private IUnitOfWorkManager _unitOfWorkManager;

        public MenuService(IUrlHelperFactory urlHelper, IActionContextAccessor accessor, IUnitOfWorkManager unitOfWorkManager)
        {
            ActionContext context = accessor.ActionContext;
            _url = urlHelper.GetUrlHelper(context);

            _unitOfWorkManager = unitOfWorkManager;
        }

        public List<MenuDTO> GetMenusByClaims(IEnumerable<Claim> claims)
        {
            return (from menu in GenerateMenusByControllerAction()
                    where IsClaimMatched(menu.Claims, claims)
                    select menu).ToList();
        }

        private bool IsClaimMatched(string requiredClaims, IEnumerable<Claim> userClaims)
        {
            if (String.IsNullOrWhiteSpace(requiredClaims))
            {
                return true;
            }
            else if (userClaims == null || !userClaims.Any())
            {
                return false;
            }

            var requiredClaimsArr = requiredClaims.Split(GeneralConstants.DelimeterSemicolon);
            foreach (var claim in requiredClaimsArr)
            {
                if (!userClaims.HasClaim(ClaimConstants.PermissionClaimType, claim))
                {
                    return false;
                }
            }

            return true;
        }

        public List<MenuDTO> GenerateMenusByControllerAction()
        {
            //TODO, lock may affect the performance
            lock (_lockObj)
            {
                IUnitOfWork unitOfWork;
                using (unitOfWork = _unitOfWorkManager.Begin())
                {
                    var menuRepo = unitOfWork.GetRepository<MenuDTO, string>();
                    List<MenuDTO> menus = menuRepo.GetAll().ToList();
                    if (menus != null && menus.Any())
                    {
                        return menus;
                    }

                    Assembly assembly = Assembly.GetEntryAssembly();

                    menus = new List<MenuDTO>();
                    Regex regexClaimPolicy = new Regex(ClaimConstants.CLAIM_REGULAR_PATTERN);
                    var controlleractionlist = ClaimsAnalyzer.GetControllerActionList(assembly);

                    controlleractionlist.ForEach(action =>
                    {
                        IEnumerable<AuthorizeAttribute> authorizeAttrs = action.GetCustomAttributes<AuthorizeAttribute>();

                        //bool accessAllowed = true;
                        IEnumerable<Claim> claims = null;
                        if (authorizeAttrs != null && authorizeAttrs.Any())
                        {
                            claims = from attr in authorizeAttrs
                                     where attr.Policy != null && regexClaimPolicy.IsMatch(attr.Policy)
                                     let match = regexClaimPolicy.Match(attr.Policy)
                                     select new Claim(ClaimConstants.PermissionClaimType, match.Groups[0].Value);

                            //if (user != null)
                            //{
                            //    foreach (var claim in claims)
                            //    {
                            //        if (!user.Claims.HasClaim(claim.Type, claim.Value))
                            //        {
                            //            accessAllowed = false;
                            //            break;
                            //        }
                            //    }

                            //    //if (accessAllowed)
                            //    //{
                            //    //    MenuDTO menu = new MenuDTO
                            //    //    {
                            //    //        Name = action.Name,
                            //    //        Url = _url.Action(action.Name, action.DeclaringType.Name)
                            //    //    };

                            //    //    menus.Add(menu);
                            //    //}
                            //}
                        }

                        //if (accessAllowed)
                        //{
                        string controllerName = GetControllerName(action.DeclaringType);
                        MenuDTO menu = new MenuDTO
                        {
                            Name = controllerName + action.Name,
                            Url = _url.Action(action.Name, controllerName),
                            Order = 0,
                            Claims = String.Join(GeneralConstants.DelimeterSemicolon, claims ?? new List<Claim>()),
                            Visible = true
                        };

                        menus.Add(menu);

                        menuRepo.AddRange(menus);
                        //}
                    });

                    unitOfWork.Commit();

                    return menus;
                }
            }
        }

        //public object GetMenusByUser(ClaimsPrincipal user)
        //{

        //}

        private static string GetControllerName(Type controllerType)
        {
            return controllerType.Name.Substring(0, controllerType.Name.IndexOf("Controller", StringComparison.OrdinalIgnoreCase));
        }
    }

}
