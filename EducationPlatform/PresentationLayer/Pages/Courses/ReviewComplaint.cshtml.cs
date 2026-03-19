using BusinessLayer.DTO;
using BusinessLayer.Interface;
using Domain.CourseManagement.Aggregate;
using Domain.IdentityManagement.Aggregate;
using Domain.IdentityManagement.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Helper;

namespace PresentationLayer.Pages.Courses
{
    public class ReviewComplaintModel : PageModel
    {
        #region Attributes
        private readonly ICourseService courseService;
        #endregion

        #region Properties
        public ComplaintDetailDTO Complaint { get; set; } = new ComplaintDetailDTO();
        public CourseDetailDTO Course { get; set; } = new CourseDetailDTO();

        [BindProperty]
        public ReviewComplaintDTO ReviewDto { get; set; } = new ReviewComplaintDTO();
        #endregion

        public ReviewComplaintModel(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        #region Methods
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                (Guid userId, string role) = CheckClaimHelper.CheckClaim(User);
                Complaint = await courseService.GetComplaintDetail(id);
                Course = await courseService.GetCourseDetail(Complaint.CourseID, userId, role);
                ReviewDto.ComplaintID = id;
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "danger";
                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            if (!ModelState.IsValid)
                return Page();
            var (userId, role) = CheckClaimHelper.CheckClaim(User);

            try
            {

                ReviewDto.IsApproved = string.Equals(action, "approve", StringComparison.OrdinalIgnoreCase);

                await courseService.ReviewComplaint(ReviewDto, userId);

                TempData["ToastMessage"] = "Complaint reviewed successfully.";
                TempData["ToastType"] = "success";

                return RedirectToPage("ListComplaint");
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "danger";

                Complaint = await courseService.GetComplaintDetail(ReviewDto.ComplaintID);
                Course = await courseService.GetCourseDetail(Complaint.CourseID, userId, role);

                return Page();
            }
        }
        #endregion
    }
}
