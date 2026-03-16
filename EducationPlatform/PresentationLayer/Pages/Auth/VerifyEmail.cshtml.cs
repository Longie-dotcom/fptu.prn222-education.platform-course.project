using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.Auth
{
    public class VerifyEmailModel : PageModel
    {
        #region Attributes
        private readonly IIdentityService identityService;
        #endregion

        #region Properties
        [BindProperty]
        public string Otp { get; set; } = string.Empty;
        #endregion

        public VerifyEmailModel(
            IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        #region Methods
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await identityService.VerifyEmail(Otp);

                TempData["ToastMessage"] = "Verification successful. Please login.";
                TempData["ToastType"] = "success";

                return RedirectToPage("Login");
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "danger";
                return Page();
            }
        }
        #endregion
    }
}