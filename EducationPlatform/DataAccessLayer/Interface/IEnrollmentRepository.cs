using Domain.EnrollmentManagement.Aggregate;

namespace DataAccessLayer.Interface
{
    public interface IEnrollmentRepository :
        IGenericRepository<Enrollment>,
        IRepositoryBase
    {
        // Get all enrollments for a student
        Task<IEnumerable<Enrollment>> GetStudentEnrollments(Guid studentId);

        // Get detailed enrollment with course, chapters, lessons, progress, quizzes
        Task<Enrollment?> GetEnrollmentDetailByID(Guid enrollmentId);

        Task<List<Guid>> GetEnrolledStudentIdsByCourseId(
            Guid courseId);

        // Get student statistics across enrollments
        Task<Enrollment?> GetEnrollmentStatistic(Guid enrollmentId);

        // Get enrollment for update (tracked)
        Task<Enrollment?> GetEnrollmentForUpdate(Guid enrollmentId);

        // Upsert lesson progress (handles chapters internally)
        Task UpsertLessonProgress(Guid enrollmentId, Guid chapterId, Guid lessonId, bool isCompleted);

        // Upsert quiz progress (handles chapters/lessons internally)
        Task<(bool isCorrect, List<string> correctAnswers, string explanation)> UpsertQuizProgress(
            Guid enrollmentId,
            Guid chapterId,
            Guid lessonId,
            Guid quizId,
            List<string> submittedAnswers);

        Task<(
            int Total,
            int Completed,
            Dictionary<string, int> GradeCounts,
            Dictionary<string, int> SubjectCounts
        )> Summary(
            DateTime? from,
            DateTime? to);

        Task<Dictionary<string, List<(string Label, decimal Value)>>> AnalyticsGrowth(
            DateTime? from,
            DateTime? to,
            string groupBy,
            Guid? gradeId,
            Guid? subjectId);
    }
}