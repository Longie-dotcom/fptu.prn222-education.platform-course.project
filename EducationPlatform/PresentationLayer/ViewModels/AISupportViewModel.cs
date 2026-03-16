using BusinessLayer.DTO;

namespace PresentationLayer.ViewModels
{
    public class AISupportViewModel
    {
        public List<EnrollmentDTO> Enrollments { get; set; } = new();

        public Guid? SelectedEnrollmentId { get; set; }

        public List<StudentWeaknessDTO> Weaknesses { get; set; } = new();

        public string? GeneratedQuiz { get; set; }
    }
}