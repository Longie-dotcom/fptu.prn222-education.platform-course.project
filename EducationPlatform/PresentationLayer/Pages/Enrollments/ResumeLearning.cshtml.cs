using BusinessLayer.DTO;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Helper;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Pages.Enrollments
{
    [Authorize(Roles = "Student")]
    public class ResumeLearningModel : PageModel
    {
        #region Attributes
        private readonly IEnrollmentService enrollmentService;
        #endregion

        #region Properties
        public EnrollmentDetailDTO Enrollment { get; set; } = new();
        #endregion

        public ResumeLearningModel(IEnrollmentService enrollmentService)
        {
            this.enrollmentService = enrollmentService;
        }

        #region Methods
        public async Task OnGetAsync(Guid enrollmentId)
        {
            try
            {
                (Guid userId, string role) = CheckClaimHelper.CheckClaim(User);

                // Get enrollment detail
                Enrollment = await enrollmentService.GetEnrollmentDetail(enrollmentId);

                // Optionally, filter or prepare any extra data for the frontend
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = "Could not resume your enrollment: " + ex.Message;
                TempData["ToastType"] = "danger";
            }
        }

        // Update lesson progress
        public async Task<IActionResult> OnPostUpdateLessonProgress([FromBody] ProgressViewModel vm)
        {
            try
            {
                (Guid userId, string role) = CheckClaimHelper.CheckClaim(User);

                await enrollmentService.UpdateLessonProgress(
                    vm.EnrollmentID,
                    vm.ChapterID,
                    vm.LessonID,
                    vm.PlayedSeconds,
                    vm.Duration,
                    vm.IsCompleted,
                    userId);

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message }) { StatusCode = 400 };
            }
        }

        // Update quiz progress
        public async Task<IActionResult> OnPostUpdateQuizProgress([FromBody] QuizSubmitViewModel vm)
        {
            try
            {
                (Guid userId, string role) = CheckClaimHelper.CheckClaim(User);

                var result = await enrollmentService.UpdateQuizProgress(
                    vm.EnrollmentID,
                    vm.ChapterID,
                    vm.LessonID,
                    vm.QuizID,
                    vm.SelectedAnswers,
                    userId);

                return new JsonResult(new
                {
                    isCorrect = result.isCorrect,
                    explanation = result.explanation,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message }) { StatusCode = 400 };
            }
        }
        #endregion
    }
}