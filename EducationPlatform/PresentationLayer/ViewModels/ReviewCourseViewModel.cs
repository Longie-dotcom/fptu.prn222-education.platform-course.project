using BusinessLayer.DTO;

namespace PresentationLayer.ViewModels
{
    public class ReviewCourseViewModel
    {
        public CourseDetailDTO Course { get; set; } = new CourseDetailDTO();
        public IEnumerable<PolicyDTO> Policies { get; set; } = new List<PolicyDTO>();
        public List<Guid> SelectedViolatedPolicyIDs { get; set; } = new List<Guid>();
        public ReviewCourseDTO ReviewCourseDTO { get; set; } = new ReviewCourseDTO();
    }
}