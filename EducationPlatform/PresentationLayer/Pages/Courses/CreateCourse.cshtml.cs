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
    [Authorize(Roles = "Teacher")]
    public class CreateCourseModel : PageModel
    {
        #region Attributes
        private readonly ICourseService courseService;
        private readonly IAcademicService academicService;
        private readonly IStorageService storageService;
        private readonly IHubContext<CourseHub> courseHub;
        #endregion

        #region Properties
        [BindProperty]
        public CreateCourseViewModel VM { get; set; } = new();
        #endregion

        public CreateCourseModel(
            ICourseService courseService,
            IAcademicService academicService,
            IStorageService storageService,
            IHubContext<CourseHub> courseHub)
        {
            this.courseService = courseService;
            this.academicService = academicService;
            this.storageService = storageService;
            this.courseHub = courseHub;
        }

        #region Methods
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                VM.Grades = await academicService.GetGrades();
                VM.Subjects = await academicService.GetSubjects();
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
                string thumbnailName = await storageService.SaveAsync(
                    VM.Thumbnail.OpenReadStream(),
                    Path.GetExtension(VM.Thumbnail.FileName).TrimStart('.'),
                    CancellationToken.None
                );

                var (userId, role) = CheckClaimHelper.CheckClaim(User);
                VM.CreateCourseDTO.ThumbnailName = thumbnailName;

                await courseService.CreateCourse(VM.CreateCourseDTO, userId, role);

                await courseHub.Clients.All.SendAsync("CourseCreated", VM.CreateCourseDTO.Title);

                TempData["ToastMessage"] = "Course submitted for admin review.";
                TempData["ToastType"] = "success";

                return RedirectToPage("ListCourse");
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "danger";

                VM.Grades = await academicService.GetGrades();
                VM.Subjects = await academicService.GetSubjects();

                return Page();
            }
        }

        public async Task<IActionResult> OnPostUploadChunkAsync(
            IFormFile chunk,
            string uploadId,
            int index,
            CancellationToken ct)
        {
            if (chunk == null || chunk.Length == 0)
                return BadRequest("Empty chunk");

            await using var stream = chunk.OpenReadStream();

            await storageService.SaveChunkAsync(stream, uploadId, index, ct);

            return new OkResult();
        }

        public async Task<IActionResult> OnPostCompleteAsync(
            string uploadId,
            string extension,
            CancellationToken ct)
        {
            var path = await storageService.CompleteUploadAsync(uploadId, extension, ct);

            return new JsonResult(new { url = path });
        }

        public async Task<IActionResult> OnGetDefaultLessonsAsync(Guid subjectId, Guid gradeId)
        {
            try
            {
                var defaultLessons = await academicService.GetDefaultLessons(subjectId, gradeId);
                return new JsonResult(defaultLessons);
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "danger";

                VM.Grades = await academicService.GetGrades();
                VM.Subjects = await academicService.GetSubjects();

                return Page();
            }
        }
        #endregion
    }
}