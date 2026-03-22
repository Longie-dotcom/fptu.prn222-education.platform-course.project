using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Helper;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Pages.AISupports
{
    [Authorize(Roles = "Student")]
    public class AISupportModel : PageModel
    {
        #region Attributes
        private readonly IStorageService storageService;
        private readonly ICourseService courseService;
        private readonly IEnrollmentService enrollmentService;
        private readonly IAIService aiService;
        #endregion

        #region Properties
        [BindProperty]
        public AISupportViewModel ViewModel { get; set; } = new();
        #endregion

        public AISupportModel(
            IStorageService storageService,
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            IAIService aiService)
        {
            this.storageService = storageService;
            this.courseService = courseService;
            this.enrollmentService = enrollmentService;
            this.aiService = aiService;
        }

        #region Methods
        public async Task OnGet()
        {
            await LoadInitialData();
        }

        // 1. When Enrollment/Course is selected
        public async Task<IActionResult> OnPostLoadCourse()
        {
            await LoadInitialData();
            if (ViewModel.SelectedEnrollmentId.HasValue)
            {
                var enrollment = ViewModel.Enrollments.FirstOrDefault(e => e.EnrollmentID == ViewModel.SelectedEnrollmentId);
                if (enrollment != null)
                {
                    var courseDetail = await courseService.GetCourseDetail(enrollment.CourseID);
                    ViewModel.SelectedCourse = courseDetail;
                    ViewModel.Chapters = courseDetail.Chapters.OrderBy(c => c.Order).ToList();
                }
            }
            return Page();
        }

        // 2. When Chapter is selected
        public async Task<IActionResult> OnPostLoadChapter()
        {
            await OnPostLoadCourse(); // Maintain hierarchy
            if (ViewModel.SelectedChapterId.HasValue)
            {
                var chapter = ViewModel.Chapters.FirstOrDefault(c => c.ChapterID == ViewModel.SelectedChapterId);
                if (chapter != null)
                {
                    ViewModel.Lessons = chapter.Lessons.OrderBy(l => l.Order).ToList();
                }
            }
            return Page();
        }

        // 3. When Lesson is selected (Optional: if you need to load lesson-specific metadata before generating)
        public async Task<IActionResult> OnPostLoadLesson()
        {
            await OnPostLoadChapter();
            if (ViewModel.SelectedLessonId.HasValue)
            {
                ViewModel.SelectedLesson = ViewModel.Lessons.FirstOrDefault(l => l.LessonID == ViewModel.SelectedLessonId);
            }
            return Page();
        }

        // 4. Final AI Generation
        public async Task<IActionResult> OnPostGenerateQuiz()
        {
            try
            {
                await OnPostLoadLesson();

                if (ViewModel.SelectedLesson == null)
                {
                    ShowToast("Please select a lesson first.", "warning");
                    return Page();
                }

                // Get Transcript from Storage Service using the VideoUrl
                // Assuming VideoUrl is the path/name stored in your cloud storage
                string transcript = await storageService.GetTranscriptFromVideoAsync(
                            ViewModel.SelectedLesson.VideoUrl,
                            default // CancellationToken
                        );

                // Call AI with Grade, Subject, and Transcript
                ViewModel.GeneratedQuiz = await aiService.GenerateQuizAsync(
                    ViewModel.SelectedCourse.Grade.Name,
                    ViewModel.SelectedCourse.Subject.Name,
                    transcript
                );

                return Page();
            }
            catch (Exception ex)
            {
                ShowToast($"AI Error: {ex.Message}", "danger");
                return Page();
            }
        }

        private async Task LoadInitialData()
        {
            var (userId, _) = CheckClaimHelper.CheckClaim(User);
            var enrollments = await enrollmentService.GetStudentEnrollments(userId);
            ViewModel.Enrollments = enrollments.ToList();
        }

        private void ShowToast(string message, string type)
        {
            TempData["ToastMessage"] = message;
            TempData["ToastType"] = type;
        }
        #endregion
    }
}