using BusinessLayer.DTO;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Helper;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Pages.Courses
{
    public class ListCourseModel : PageModel
    {
        #region Attributes
        private readonly ICourseService courseService;
        private readonly IAcademicService academicService;
        #endregion

        #region Properties
        [BindProperty(SupportsGet = true)]
        public ListCourseViewModel VM { get; set; } = new();
        #endregion

        public ListCourseModel(
            ICourseService courseService,
            IAcademicService academicService)
        {
            this.courseService = courseService;
            this.academicService = academicService;
        }

        #region Methods
        public async Task OnGetAsync(int page = 1)
        {
            try
            {
                var query = new QueryCourseDTO
                {
                    Title = VM.Title,
                    GradeName = VM.GradeName,
                    SubjectName = VM.SubjectName,
                    PageIndex = page,
                    PageSize = 6
                };

                // Check if user is logged in
                if (User.Identity?.IsAuthenticated == true)
                {
                    // Logged-in user: get user ID and role
                    (Guid userId, string role) = CheckClaimHelper.CheckClaim(User);
                    VM.Courses = await courseService.GetCourses(query, userId, role);
                }
                else
                {
                    // Public/discovery: no login required
                    VM.Courses = await courseService.DiscoverCourses(query);
                }

                // Populate grades and subjects for filter dropdowns
                VM.Grades = await academicService.GetGrades();
                VM.Subjects = await academicService.GetSubjects();
                VM.CurrentPage = page;
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "danger";
            }
        }
        #endregion
    }
}