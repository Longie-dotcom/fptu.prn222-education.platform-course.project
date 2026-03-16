using BusinessLayer.DTO;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Helper;

namespace PresentationLayer.Pages.Enrollments
{
    [Authorize(Roles = "Student")]
    public class ListEnrollmentModel : PageModel
    {
        #region Attributes
        private readonly IEnrollmentService enrollmentService;
        #endregion

        #region Properties
        public IEnumerable<EnrollmentDTO> Enrollments { get; set; } = new List<EnrollmentDTO>();
        #endregion

        public ListEnrollmentModel(IEnrollmentService enrollmentService)
        {
            this.enrollmentService = enrollmentService;
        }

        #region Methods
        public async Task OnGet()
        {
            try
            {
                (Guid userId, string role) = CheckClaimHelper.CheckClaim(User);
                Enrollments = await enrollmentService.GetStudentEnrollments(userId);
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = "Could not load your enrollments: " + ex.Message;
                TempData["ToastType"] = "danger";
            }
        }
        #endregion
    }
}