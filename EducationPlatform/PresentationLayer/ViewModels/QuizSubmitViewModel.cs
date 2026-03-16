namespace PresentationLayer.ViewModels
{
    public class QuizSubmitViewModel
    {
        public List<string> SelectedAnswers { get; set; } = new List<string>();
        public Guid EnrollmentID { get; set; }
        public Guid ChapterID { get; set; }
        public Guid LessonID { get; set; }
        public Guid QuizID { get; set; }
    }
}
