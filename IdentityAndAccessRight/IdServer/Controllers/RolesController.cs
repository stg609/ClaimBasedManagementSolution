using System.Linq;
using IdServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdServer.Controllers
{
    public class RolesController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            //返回 List，保证已经执行完数据库查询，从而避免后续数据库查询导致嵌套查询的问题
            var roles = _roleManager.Roles.ToList();

            var roleVMs = from role in roles
                          select new RoleViewModel
                          {
                              Name = role.Name,
                              Claims = _roleManager.GetClaimsAsync(role).Result.ToList()
                          };

            return View(roleVMs);
        }
    }
}