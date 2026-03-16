using BusinessLayer.DTO;

namespace PresentationLayer.ViewModels
{
    public class CreateCourseViewModel
    {
        public CreateCourseDTO CreateCourseDTO { get; set; } = new CreateCourseDTO();
        public IFormFile Thumbnail { get; set; }
        public IEnumerable<GradeDTO> Grades { get; set; } = new List<GradeDTO>();
        public IEnumerable<SubjectDTO> Subjects { get; set; } = new List<SubjectDTO>();
    }
}
