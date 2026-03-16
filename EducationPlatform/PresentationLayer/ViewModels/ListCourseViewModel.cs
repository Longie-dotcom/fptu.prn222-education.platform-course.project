using BusinessLayer.DTO;

namespace PresentationLayer.ViewModels
{
    public class ListCourseViewModel
    {
        // Filter Parameters
        public string? Title { get; set; }
        public string? GradeName { get; set; }
        public string? SubjectName { get; set; }

        // Data
        public IEnumerable<CourseDTO> Courses { get; set; } = new List<CourseDTO>();

        // Dropdown Sources (Normalized Data)
        public IEnumerable<GradeDTO> Grades { get; set; } = new List<GradeDTO>();
        public IEnumerable<SubjectDTO> Subjects { get; set; } = new List<SubjectDTO>();

        // Pagination Info (Optional but recommended)
        public int CurrentPage { get; set; }
    }
}