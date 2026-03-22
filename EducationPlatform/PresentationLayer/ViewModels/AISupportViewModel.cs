using BusinessLayer.DTO;

namespace PresentationLayer.ViewModels
{
    public class AISupportViewModel
    {
        public List<EnrollmentDTO> Enrollments { get; set; } = new(); // Get course ID from here
        public Guid? SelectedEnrollmentId { get; set; }
        public CourseDetailDTO SelectedCourse { get; set; } = new();

        public List<ChapterDTO> Chapters { get; set; } = new();  // Get chapter ID from here
        public Guid? SelectedChapterId { get; set; }

        public List<LessonDTO> Lessons { get; set; } = new(); // Get lesson from here
        public Guid? SelectedLessonId { get; set; }
        public LessonDTO? SelectedLesson { get; set; }

        public List<StudentWeaknessDTO> Weaknesses { get; set; } = new();

        public string? GeneratedQuiz { get; set; }
    }
}