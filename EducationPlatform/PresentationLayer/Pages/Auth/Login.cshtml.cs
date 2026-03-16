using BusinessLayer.DTO;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using PresentationLayer.Hubs;
using System.Security.Claims;

namespace PresentationLayer.Pages.Auth
{
    public class LoginModel : PageModel
    {
        #region Attributes
        private readonly IIdentityService identityService;
        private readonly IHubContext<AuthHub> hubContext;
        #endregion

        #region Properties
        [BindProperty]
        public LoginDTO Login { get; set; } = new();
        #endregion

        public LoginModel(
            IIdentityService identityService,
            IHubContext<AuthHub> hubContext)
        {
            this.identityService = identityService;
            this.hubContext = hubContext;
        }

        #region Methods
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var token = await identityService.Login(Login);

                Response.Cookies.Append("access_token", token.Token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(60)
                    });

                Response.Cookies.Append("refresh_token", token.RefreshToken,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });

                return RedirectToPage("/Courses/ListCourse");
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "danger";
                return Page();
            }
        }

        public async Task<IActionResult> OnGetLogout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await AuthHub.ForceLogout(hubContext, userId);
            }

            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            TempData.Clear();
            return RedirectToAction("Login");
        }
        #endregion
    }
}