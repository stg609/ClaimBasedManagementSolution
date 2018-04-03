//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Threading.Tasks;
//using Common.Infra;
//using Microsoft.AspNetCore.Mvc;

//namespace SampleMVCApp.Controllers
//{
//    public class MetadataController : Controller
//    {
//        [Route(".metadata")]
//        public IActionResult Index()
//        {
//            return new JsonResult(from claim in ClaimsAnalyzer.GetAllClaimsOfControllers(Assembly.GetExecutingAssembly())
//                                  select new
//                                  {
//                                      Type = claim.Type,
//                                      Value = claim.Value
//                                  });
//        }
//    }
//}