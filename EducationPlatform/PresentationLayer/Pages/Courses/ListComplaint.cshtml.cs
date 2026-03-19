using BusinessLayer.DTO;
using BusinessLayer.Interface;
using Domain.CourseManagement.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Helper;

namespace PresentationLayer.Pages.Courses
{
    public class ListComplaintModel : PageModel
    {
        #region Attributes
        private readonly ICourseService courseService;
        #endregion

        #region Properties
        public IEnumerable<ComplaintDTO> Complaints { get; set; } = new List<ComplaintDTO>();

        [BindProperty(SupportsGet = true)]
        public ComplaintStatus? QueryStatus { get; set; }
        #endregion

        public ListComplaintModel(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        #region Methods
        public async Task OnGetAsync()
        {
            try
            {
                var (userId, role) = CheckClaimHelper.CheckClaim(User);
                Complaints = await courseService.GetComplaints(QueryStatus, userId, role);
            }
            catch
            {
                Complaints = new List<ComplaintDTO>();
            }
        }
        #endregion
    }
}
