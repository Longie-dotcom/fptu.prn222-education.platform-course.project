namespace PresentationLayer.ViewModels
{
    public class ProgressViewModel
    {
        public Guid EnrollmentID { get; set; }
        public Guid ChapterID { get; set; }
        public Guid LessonID { get; set; }
        public double PlayedSeconds { get; set; }
        public double Duration { get; set; }
        public bool IsCompleted { get; set; }
    }
}
