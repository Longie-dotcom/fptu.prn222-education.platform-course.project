using BusinessLayer.DTO;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.ViewModels
{
    public class ListCourseViewModel
    {
        // Filter Parameters
        public string? Title { get; set; }
        public string? GradeName { get; set; }
        public string? SubjectName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        // Data
        public IEnumerable<CourseDTO> Courses { get; set; } = new List<CourseDTO>();

        // Dropdown Sources
        public IEnumerable<GradeDTO> Grades { get; set; } = new List<GradeDTO>();
        public IEnumerable<SubjectDTO> Subjects { get; set; } = new List<SubjectDTO>();
    }
}