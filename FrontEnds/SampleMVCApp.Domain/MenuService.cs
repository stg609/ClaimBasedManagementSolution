using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Common.Domain;
using Common.Infra;
using Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SampleMVCApp.Domain
{
    public class MenuService : IMenuService
    {
        private static readonly object _lockObj = new object();

        private IUrlHelper _url;
        private IUnitOfWorkManager _unitOfWorkManager;
        private IEnumerable<MenuDTO> _cachedAllMenus;

        public MenuService(IUrlHelperFactory urlHelper, IActionContextAccessor accessor, IUnitOfWorkManager unitOfWorkManager)
        {
            ActionContext context = accessor.ActionContext;
            _url = urlHelper.GetUrlHelper(context);

            _unitOfWorkManager = unitOfWorkManager;
        }

        public MenuDTO FindByKey(int key)
        {
            if (key < 0)
            {
                return null;
            }

            IUnitOfWork unitOfWork;
            using (unitOfWork = _unitOfWorkManager.Begin())
            {
                var menuRepo = unitOfWork.GetRepository<MenuDTO, int>();
                return menuRepo.Get(itm => itm.Key == key);
            }
        }

        public List<MenuDTO> GetAll()
        {
            return GetAllMenus().ToList();
        }

        /// <summary>
        /// 根据当前用户的 Claims 来获得对应的可见菜单
        /// </summary>
        /// <param name="claims">用户的 Claims</param>
        /// <returns></returns>
        public List<MenuDTO> GetMenusByClaims(IEnumerable<Claim> claims)
        {
            return (from menu in GetAllMenus()
                    where IsClaimMatched(menu.Claims, claims)
                    select menu).ToList();
        }


        /// <summary>
        /// 从 Controller/Action 中生成菜单数据
        /// </summary>
        /// <param name="isSync">是否同步。True 表示同步，即删除原先的菜单数据重新生成。默认为 False，如果菜单数据已经存在，则直接返回。</param>
        /// <returns></returns>
        public List<MenuDTO> GenerateMenusByControllerAction(bool isSync = false)
        {
            //TODO, lock may affect the performance
            lock (_lockObj)
            {
                IUnitOfWork unitOfWork;
                using (unitOfWork = _unitOfWorkManager.Begin())
                {
                    var menuRepo = unitOfWork.GetRepository<MenuDTO, int>();

                    List<MenuDTO> menus;

                    if (!isSync)
                    {
                        menus = menuRepo.GetAll().ToList();
                        if (menus != null && menus.Any())
                        {
                            return menus;
                        }
                    }

                    //如果同步，则删除原先的数据重新生成。
                    menuRepo.RemoveAll();

                    Assembly assembly = Assembly.GetEntryAssembly();

                    menus = new List<MenuDTO>();
                    Regex regexClaimPolicy = new Regex(ClaimConstants.CLAIM_REGULAR_PATTERN);
                    var controlleractionlist = ClaimsAnalyzer.GetControllerActionList(assembly);

                    controlleractionlist.ForEach(action =>
                    {
                        //获得 action 级别的 attribute
                        IEnumerable<AuthorizeAttribute> controllerLevelAuthorizeAttrs = action.DeclaringType.GetCustomAttributes<AuthorizeAttribute>(true);
                        IEnumerable<AuthorizeAttribute> actionLevelAuthorizeAttrs = action.GetCustomAttributes<AuthorizeAttribute>(true);

                        //bool accessAllowed = true;
                        List<Claim> claims = GetClaimsFromAuthorizeAttribute(controllerLevelAuthorizeAttrs, regexClaimPolicy);
                        claims.AddRange(GetClaimsFromAuthorizeAttribute(actionLevelAuthorizeAttrs, regexClaimPolicy));


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
                    });

                    unitOfWork.Commit();

                    return menus;
                }
            }
        }

        public void Create(MenuDTO menu)
        {
            IUnitOfWork unitOfWork;
            using (unitOfWork = _unitOfWorkManager.Begin())
            {
                var repo = unitOfWork.GetRepository<MenuDTO, int>();
                repo.Add(menu);

                unitOfWork.Commit();
            }
        }

        public void Update(int key, string name, string url, bool visible = true, int parentKey = 0)
        {
            if (key <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            IUnitOfWork unitOfWork;
            using (unitOfWork = _unitOfWorkManager.Begin())
            {
                var repo = unitOfWork.GetRepository<MenuDTO, int>();
                var target = repo.Get(itm => itm.Key == key);

                if (target == null)
                {
                    throw new Exception(ErrorConstants.MENU_NOT_EXISTED);
                }

                target.Name = name;
                target.Url = url;
                target.Visible = visible;

                parentKey = parentKey < 0 ? 0 : parentKey;
                if (parentKey > 0)
                {
                    var parent = repo.Get(itm => itm.Key == parentKey);
                    if (parent == null)
                    {
                        throw new Exception(ErrorConstants.MENU_NOT_EXISTED);
                    }
                    target.ParentMenuKey = parentKey;
                }
                repo.Update(target);
                unitOfWork.Commit();
            }
        }

        public void Delete(int key)
        {
            if (key <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            IUnitOfWork unitOfWork;
            using (unitOfWork = _unitOfWorkManager.Begin())
            {
                var repo = unitOfWork.GetRepository<MenuDTO, int>();
                repo.Remove(key);
                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// 移动菜单
        /// </summary>
        /// <param name="key"></param>
        /// <param name="step"></param>
        public void Move(int key, int step = 0)
        {
            if (key <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if(step == 0)
            {
                return;
            }

            //step > 0 , Up
            //step < 0, Down
            IUnitOfWork unitOfWork;
            using (unitOfWork = _unitOfWorkManager.Begin())
            {
                var repo = unitOfWork.GetRepository<MenuDTO, int>();
                var target = repo.Get(itm => itm.Key == key);
                if (target == null)
                {
                    throw new Exception(ErrorConstants.MENU_NOT_EXISTED);
                }

                //Menu is ordered by asc
                target.Order = target.Order - (step);

                repo.Update(target);
                unitOfWork.Commit();
            }
        }


        public void SetVisibility(int key, bool visible)
        {
            if (key <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            IUnitOfWork unitOfWork;
            using (unitOfWork = _unitOfWorkManager.Begin())
            {
                var repo = unitOfWork.GetRepository<MenuDTO, int>();
                var target = repo.Get(itm => itm.Key == key);

                if (target == null)
                {
                    throw new Exception(ErrorConstants.MENU_NOT_EXISTED);
                }

                target.Visible = visible;
                repo.Update(target);
                unitOfWork.Commit();
            }
        }

        private IEnumerable<MenuDTO> GetAllMenus()
        {
            if (_cachedAllMenus == null)
            {
                _cachedAllMenus = GenerateMenusByControllerAction();
                //IUnitOfWork unitOfWork;
                //using (unitOfWork = _unitOfWorkManager.Begin())
                //{
                //    var menuRepo = unitOfWork.GetRepository<MenuDTO, string>();
                //    _cachedAllMenus = menuRepo.GetAll();
                //}
            }
            return _cachedAllMenus;
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
                if (!userClaims.HasPermissionClaim(claim))
                {
                    return false;
                }
            }

            return true;
        }

        private static string GetControllerName(Type controllerType)
        {
            return controllerType.Name.Substring(0, controllerType.Name.IndexOf("Controller", StringComparison.OrdinalIgnoreCase));
        }

        private List<Claim> GetClaimsFromAuthorizeAttribute(IEnumerable<AuthorizeAttribute> authorizeAttributes, Regex regexClaimPolicy)
        {
            if (authorizeAttributes != null && authorizeAttributes.Any())
            {
                return (from attr in authorizeAttributes
                        where attr.Policy != null && regexClaimPolicy.IsMatch(attr.Policy)
                        let match = regexClaimPolicy.Match(attr.Policy)
                        select new Claim(ClaimConstants.PermissionClaimType, match.Groups[0].Value)).ToList();

            }
            else
            {
                return new List<Claim>();
            }
        }

    }

}
