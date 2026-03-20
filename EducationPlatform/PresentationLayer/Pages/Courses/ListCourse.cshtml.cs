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

        public int PageIndex { get; set; } = 1;
        #endregion

        public ListCourseModel(
            ICourseService courseService,
            IAcademicService academicService)
        {
            this.courseService = courseService;
            this.academicService = academicService;
        }

        #region Methods
        public async Task OnGetAsync()
        {
            try
            {
                var query = new QueryCourseDTO
                {
                    Title = VM.Title,
                    GradeName = VM.GradeName,
                    SubjectName = VM.SubjectName,
                    PageIndex = VM.CurrentPage,
                    PageSize = 9
                };

                if (User.Identity?.IsAuthenticated == true)
                {
                    (Guid userId, string role) = CheckClaimHelper.CheckClaim(User);
                    VM.Courses = await courseService.GetCourses(query, userId, role);
                }
                else
                {
                    VM.Courses = await courseService.DiscoverCourses(query);
                }

                VM.Grades = await academicService.GetGrades();
                VM.Subjects = await academicService.GetSubjects();
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