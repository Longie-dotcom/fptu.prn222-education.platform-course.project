using BusinessLayer.DTO;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        #region Attributes
        private readonly IIdentityService identityService;
        #endregion

        #region Properties
        [BindProperty]
        public RegisterDTO Register { get; set; } = new();
        #endregion

        public RegisterModel(
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
                await identityService.Register(Register);

                TempData["ToastMessage"] = "Register successful. Please login.";
                TempData["ToastType"] = "success";

                return RedirectToPage("VerifyEmail");
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