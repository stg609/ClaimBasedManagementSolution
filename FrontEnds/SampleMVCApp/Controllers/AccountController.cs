using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SampleMVCApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("oidc");

            return new SignOutResult(new[] { "oidc", "Cookies" });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // clear any existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync("oidc");

            //// see IdentityServer4 QuickStartUI AccountController ExternalLogin
            //await HttpContext.ChallengeAsync("oidc",
            //    new AuthenticationProperties()
            //    {
            //        RedirectUri = Url.Action("LoginCallback"),
            //    });

            var callbackUrl = Url.Action(nameof(LoginCallback), new { returnUrl });

            var props = new AuthenticationProperties
            {
                RedirectUri = callbackUrl
            };

            return Challenge(props, "oidc");
        }

        [HttpGet]
        public IActionResult LoginCallback(string returnUrl = null)
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

    }
}