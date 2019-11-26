using LdapPoc.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace LdapPoc.Controllers.Common
{
    public class BaseController : Controller
    {
        // -> workaround for [allowanonymous]
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next();

            var auth = await HttpContext.AuthenticateAsync(GlobalConstants.ApplicationSchema); 

            if (auth.Succeeded)
                context.HttpContext.User = auth.Principal;
        }
    }
}
