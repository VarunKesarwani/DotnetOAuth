using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageGallery.Client.Controllers
{
    public class AuthenticationController : Controller
    {
        [Authorize]
        public async Task Logout()
        {
            // Clears local cookie and thus user cannot make further request without authentication.
            // This will log you out of web client but not out of identity provider.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            // This will call end-session endpoint in IDP
            // This Endpoint allows IDP to clear its own cookie.
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        public async Task<ViewResult> AccessDenied()
        {
            return View();
        }
    }
}
