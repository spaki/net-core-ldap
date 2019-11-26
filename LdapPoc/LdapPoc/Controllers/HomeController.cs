using LdapPoc.Constants;
using LdapPoc.Controllers.Common;
using LdapPoc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LdapPoc.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index() => View();

        [Authorize(AuthenticationSchemes = GlobalConstants.ApplicationSchema)]
        public IActionResult Privacy() => View();
        
        [Authorize(Roles = "top", AuthenticationSchemes = GlobalConstants.ApplicationSchema)]
        public IActionResult Details() => View();

        [Authorize(Roles = "TopSecret", AuthenticationSchemes = GlobalConstants.ApplicationSchema)]
        public IActionResult Secrets() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        public IActionResult AccessDenied() => View();
    }
}
