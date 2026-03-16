using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Presentation.Helper;
using PresentationLayer.Hubs;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Pages.Courses
{
    [Authorize(Roles = "Admin")]
    public class ReviewCourseModel : PageModel
    {
        #region Attributes
        private readonly ICourseService courseService;
        private readonly IHubContext<CourseHub> courseHub;
        #endregion

        #region Properties
        [BindProperty]
        public ReviewCourseViewModel VM { get; set; } = new();
        #endregion

        public ReviewCourseModel(
            ICourseService courseService,
            IHubContext<CourseHub> courseHub)
        {
            this.courseService = courseService;
            this.courseHub = courseHub;
        }

        #region Methods
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var (userId, role) = CheckClaimHelper.CheckClaim(User);

                VM.Course = await courseService.GetCourseDetail(id, userId, role);
                VM.Policies = await courseService.GetPolicies();

                return Page();
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "danger";
                return RedirectToPage("ListCourse");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var (userId, role) = CheckClaimHelper.CheckClaim(User);

                await courseService.ReviewCourse(VM.ReviewCourseDTO, userId, role);
                
                await courseHub.Clients.All.SendAsync("CourseReviewed", VM.ReviewCourseDTO.CourseID);

                TempData["ToastMessage"] = "Course reviewed successfully.";
                TempData["ToastType"] = "success";

                return RedirectToPage("ListCourse");
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "danger";

                VM.Policies = await courseService.GetPolicies();
                VM.Course = await courseService.GetCourseDetail(
                    VM.ReviewCourseDTO.CourseID,
                    CheckClaimHelper.CheckClaim(User).userId,
                    CheckClaimHelper.CheckClaim(User).role);

                return Page();
            }
        }
        #endregion
    }
}