using LdapPoc.Constants;
using LdapPoc.Controllers.Common;
using LdapPoc.Ldap;
using LdapPoc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LdapPoc.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService) => this.authService = authService;

        public async Task<IActionResult> Logout() 
        {
            await HttpContext.SignOutAsync(GlobalConstants.ApplicationSchema);
            return RedirectToAction("Login", "Auth"); 
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(GlobalConstants.ApplicationSchema);
            return View(new UserModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserModel userModel)
        {
            var authResult = authService.Login(userModel.User, userModel.Password);
            userModel.Message = authResult.AuthMessage;

            if (!authResult.Success)
                return View(userModel);

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, authResult.UserId),
                new Claim(ClaimTypes.Name, authResult.DisplayName),
                new Claim(ClaimTypes.Email, authResult.Email)
            };

            foreach (var group in authResult.Groups)
                claims.Add(new Claim(ClaimTypes.Role, group));
            
            await this.HttpContext.SignInAsync(
                GlobalConstants.ApplicationSchema,
                new ClaimsPrincipal(new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties { IsPersistent = userModel.RememberMe, ExpiresUtc = DateTime.UtcNow.AddMinutes(20) }
            );

            if (Url.IsLocalUrl(userModel.ReturnUrl))
                return Redirect(userModel.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
