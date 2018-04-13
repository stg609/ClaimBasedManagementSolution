using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleMVCApp.Models;

namespace SampleMVCApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Authorize(Policy = ClaimConstants.PolicyPrefix + "." + Services.Constants.Identity + ".action.contact")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        /// <summary>
        /// 测试访问 API
        /// </summary>
        /// <returns></returns>
        [Authorize(Policy = ClaimConstants.PolicyPrefix + "." + Services.Constants.Identity + ".action.testApi")]
        public async Task<IActionResult> TestAPI()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            var content = await client.GetStringAsync("http://localhost:5001/api/values/" + DateTime.Now.Second);
            return Content(content);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
