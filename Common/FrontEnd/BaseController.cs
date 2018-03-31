using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Common.FrontEnd
{
    public class BaseController : Controller
    {
        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        protected void AddErrors(string errMessage)
        {
            ModelState.AddModelError(string.Empty, errMessage);
        }

        protected IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        protected string GetIdentityErrorMessage(IEnumerable<IdentityError> errors)
        {
            if (errors == null)
            {
                return String.Empty;
            }

            return String.Join(Environment.NewLine, from err in errors
                                                    select err.Description);
        }

        protected string GetModelError()
        {
            return String.Join(Environment.NewLine, ModelState.Values.SelectMany(itm => itm.Errors).Select(itm => itm.ErrorMessage));
        }

        protected string GetIdentityError(IdentityResult result)
        {
            return String.Join(Environment.NewLine, result.Errors.Select(err => err.Description));
        }

    }
}
