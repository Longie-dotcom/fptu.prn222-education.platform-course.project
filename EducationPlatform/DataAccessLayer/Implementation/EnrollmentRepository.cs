using DataAccessLayer.Interface;
using DataAccessLayer.Persistence;
using Domain.EnrollmentManagement.Aggregate;
using Domain.EnrollmentManagement.Entity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DataAccessLayer.Implementation
{
    public class EnrollmentRepository :
        GenericRepository<Enrollment>,
        IEnrollmentRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public EnrollmentRepository(EducationPlatformDBContext context) : base(context) { }

        #region Methods

        public async Task<IEnumerable<Enrollment>> GetStudentEnrollments(Guid studentId)
        {
            return await context.Enrollments
                .AsNoTracking()
                .Include(e => e.Course)
                .Include(e => e.CourseProgress)
                .Where(e => e.StudentID == studentId)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetEnrollmentDetailByID(Guid enrollmentId)
        {
            return await context.Enrollments
                .AsNoTracking()
                .AsSplitQuery()
                .Include(e => e.Course)
                    .ThenInclude(c => c.Teacher)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Grade)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Subject)
                .Include(e => e.Course)
                    .ThenInclude(c => c.ViolatedPolicies)
                        .ThenInclude(vp => vp.Policy)
                            .ThenInclude(p => p.PolicyRules)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Chapters)
                        .ThenInclude(ch => ch.Lessons)
                            .ThenInclude(l => l.Quizzes)
                                .ThenInclude(q => q.Answer)
                .Include(e => e.CourseProgress)
                    .ThenInclude(cp => cp.ChapterProgresses)
                        .ThenInclude(chp => chp.LessonProgresses)
                            .ThenInclude(lp => lp.QuizProgresses)
                                .ThenInclude(qp => qp.Quiz)
                .Include(e => e.CourseProgress)
                    .ThenInclude(cp => cp.ChapterProgresses)
                        .ThenInclude(chp => chp.LessonProgresses)
                            .ThenInclude(lp => lp.Lesson)
                .FirstOrDefaultAsync(e => e.EnrollmentID == enrollmentId);
        }

        public async Task<Enrollment?> GetEnrollmentForUpdate(Guid enrollmentId)
        {
            return await context.Enrollments
                .AsSplitQuery()
                .Include(e => e.CourseProgress)
                    .ThenInclude(cp => cp.ChapterProgresses)
                        .ThenInclude(chp => chp.LessonProgresses)
                            .ThenInclude(lp => lp.QuizProgresses)
                .Include(e => e.CourseProgress)
                    .ThenInclude(cp => cp.ChapterProgresses)
                        .ThenInclude(chp => chp.LessonProgresses)
                            .ThenInclude(lp => lp.Lesson)
                .FirstOrDefaultAsync(e => e.EnrollmentID == enrollmentId);
        }

        public async Task<List<Guid>> GetEnrolledStudentIdsByCourseId(Guid courseId)
        {
            return await context.Enrollments
                .Where(e => e.CourseID == courseId)
                .Select(e => e.StudentID)
                .Distinct()
                .ToListAsync();
        }

        public async Task UpsertLessonProgress(Guid enrollmentId, Guid chapterId, Guid lessonId, bool isCompleted)
        {
            // Load CourseProgress with ChapterProgresses
            var cp = await context.CourseProgresses
                .Include(x => x.ChapterProgresses)
                    .ThenInclude(chp => chp.LessonProgresses)
                .FirstOrDefaultAsync(x => x.EnrollmentID == enrollmentId);

            if (cp == null)
            {
                var enrollment = await context.Enrollments.FindAsync(enrollmentId);
                if (enrollment == null) throw new InvalidOperationException("Enrollment not found");

                cp = new CourseProgress(Guid.NewGuid(), enrollmentId);
                context.CourseProgresses.Add(cp);
            }

            // Load or create ChapterProgress
            var chp = cp.ChapterProgresses.FirstOrDefault(x => x.ChapterID == chapterId);
            if (chp == null)
            {
                chp = new ChapterProgress(Guid.NewGuid(), cp.CourseProgressID, chapterId);
                cp.ChapterProgresses.ToList().Add(chp);
                context.ChapterProgresses.Add(chp);
            }

            // Load or create LessonProgress
            var lp = chp.LessonProgresses.FirstOrDefault(x => x.LessonID == lessonId);
            if (lp == null)
            {
                lp = new LessonProgress(Guid.NewGuid(), chp.ChapterProgressID, lessonId);
                chp.LessonProgresses.ToList().Add(lp);
                context.LessonProgresses.Add(lp);
            }

            // Mark as completed if needed
            if (isCompleted && !lp.IsCompleted)
                lp.MarkCompleted();

            // Recalculate all progress
            chp.RecalculateCompletion();
            cp.RecalculateCompletion();

            // Update Enrollment CompletedAt if course finished
            if (cp.IsCompleted)
            {
                var enrollment = await context.Enrollments.FindAsync(enrollmentId);
                if (enrollment != null && enrollment.CompletedAt == null)
                    enrollment.GetType().GetProperty("CompletedAt")?.SetValue(enrollment, DateTime.UtcNow);
            }
        }

        public async Task<(bool isCorrect, List<string> correctAnswers, string explanation)> UpsertQuizProgress(
            Guid enrollmentId,
            Guid chapterId,
            Guid lessonId,
            Guid quizId,
            List<string> submittedAnswers)
        {
            // Load CourseProgress
            var cp = await context.CourseProgresses
                .Include(x => x.ChapterProgresses)
                    .ThenInclude(chp => chp.LessonProgresses)
                        .ThenInclude(lp => lp.QuizProgresses)
                .FirstOrDefaultAsync(x => x.EnrollmentID == enrollmentId);

            if (cp == null)
            {
                var enrollment = await context.Enrollments.FindAsync(enrollmentId);
                if (enrollment == null)
                    throw new InvalidOperationException("Enrollment not found");

                cp = new CourseProgress(Guid.NewGuid(), enrollmentId);
                context.CourseProgresses.Add(cp);
            }

            // ChapterProgress
            var chp = cp.ChapterProgresses.FirstOrDefault(x => x.ChapterID == chapterId);
            if (chp == null)
            {
                chp = new ChapterProgress(Guid.NewGuid(), cp.CourseProgressID, chapterId);
                context.ChapterProgresses.Add(chp);
            }

            // LessonProgress
            var lp = chp.LessonProgresses.FirstOrDefault(x => x.LessonID == lessonId);
            if (lp == null)
            {
                lp = new LessonProgress(Guid.NewGuid(), chp.ChapterProgressID, lessonId);
                context.LessonProgresses.Add(lp);
            }

            // Load Quiz with Answer
            var quiz = await context.Quizzes
                .Include(q => q.Answer)
                .FirstOrDefaultAsync(q => q.QuizID == quizId);

            if (quiz == null)
                throw new InvalidOperationException("Quiz not found");

            // Normalize submitted answers
            var submitted = submittedAnswers?
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(NormalizeAnswer)
                .ToList() ?? new List<string>();

            // Normalize correct answers
            var correct = quiz.Answer.CorrectAnswers
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(NormalizeAnswer)
                .ToList();

            bool isCorrect;

            if (quiz.Answer.Type == Domain.CourseManagement.Enum.QuizType.SingleChoice ||
                quiz.Answer.Type == Domain.CourseManagement.Enum.QuizType.TrueFalse)
            {
                isCorrect = submitted.Count == 1 && correct.Contains(submitted.First());
            }
            else
            {
                isCorrect =
                    submitted.Count > 0 &&
                    submitted.Count == correct.Count &&
                    submitted.All(correct.Contains);
            }

            // QuizProgress
            var qp = lp.QuizProgresses.FirstOrDefault(x => x.QuizID == quizId);
            if (qp == null)
            {
                qp = new QuizProgress(Guid.NewGuid(), lp.LessonProgressID, quizId);
                context.QuizProgresses.Add(qp);
            }

            qp.RegisterAttempt(isCorrect);

            // Recalculate completion
            lp.RecalculateCompletion();
            chp.RecalculateCompletion();
            cp.RecalculateCompletion();

            // Mark enrollment completed
            if (cp.IsCompleted)
            {
                var enrollment = await context.Enrollments.FindAsync(enrollmentId);
                if (enrollment != null && enrollment.CompletedAt == null)
                {
                    enrollment.GetType()
                        .GetProperty("CompletedAt")?
                        .SetValue(enrollment, DateTime.UtcNow);
                }
            }

            return (isCorrect, correct, quiz.Note ?? "No explanation provided.");
        }

        public async Task<Enrollment?> GetEnrollmentStatistic(Guid enrollmentId)
        {
            return await context.Enrollments
                .AsNoTracking()
                .AsSplitQuery()
                .Include(e => e.Course)
                .Include(e => e.CourseProgress)
                    .ThenInclude(cp => cp.ChapterProgresses)
                        .ThenInclude(chp => chp.LessonProgresses)
                            .ThenInclude(lp => lp.QuizProgresses)
                .Include(e => e.CourseProgress)
                    .ThenInclude(cp => cp.ChapterProgresses)
                        .ThenInclude(chp => chp.LessonProgresses)
                            .ThenInclude(lp => lp.Lesson)
                .FirstOrDefaultAsync(e => e.EnrollmentID == enrollmentId);
        }

        private static string NormalizeAnswer(string value)
        {
            return value
                .Replace("[", "")
                .Replace("]", "")
                .Replace("\"", "")
                .Trim()
                .Normalize(NormalizationForm.FormC)
                .ToLowerInvariant();
        }
        #endregion
    }
}
