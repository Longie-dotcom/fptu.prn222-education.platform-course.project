using BusinessLayer.DTO;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Helper;

namespace PresentationLayer.Pages.Courses
{
    public class CreateComplaintModel : PageModel
    {
        #region Attributes
        private readonly ICourseService courseService;
        private readonly IStorageService storageService;
        #endregion

        #region Properties
        [BindProperty]
        public Guid CourseId { get; set; }
        [BindProperty]
        public string Reason { get; set; } = string.Empty;
        [BindProperty]
        public IFormFile? EvidenceImage { get; set; }
        #endregion

        public CreateComplaintModel(
            ICourseService courseService,
            IStorageService storageService)
        {
            this.courseService = courseService;
            this.storageService = storageService;
        }

        #region Methods
        public IActionResult OnGet(Guid courseId)
        {
            CourseId = courseId;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Reason))
            {
                ModelState.AddModelError(nameof(Reason), "Reason is required.");
            }

            if (!ModelState.IsValid)
                return Page();

            try
            {
                var (userId, role) = CheckClaimHelper.CheckClaim(User);

                string? imagePath = null;
                if (EvidenceImage != null && EvidenceImage.Length > 0)
                {
                    imagePath = await storageService.SaveAsync(
                        EvidenceImage.OpenReadStream(),
                        Path.GetExtension(EvidenceImage.FileName).TrimStart('.'),
                        CancellationToken.None);
                }

                var dto = new CreateComplaintDTO
                {
                    CourseID = CourseId,
                    Reason = Reason,
                    EvidenceImagePath = imagePath
                };

                await courseService.CreateComplaint(dto, userId);

                TempData["ToastMessage"] = "Your complaint has been submitted and is waiting for admin review.";
                TempData["ToastType"] = "success";

                return RedirectToPage("DetailCourse", new { id = CourseId });
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
