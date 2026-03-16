using Domain.CourseManagement.Aggregate;
using Domain.CourseManagement.Entity;
using Domain.IdentityManagement.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Interface
{
    public interface ICourseRepository :
        IGenericRepository<Course>,
        IRepositoryBase
    {
        Task<IEnumerable<Course>> GetAllCourses(
            string? title,
            decimal? price,
            string? teacherName,
            string? gradeName,
            string? subjectName,
            int pageIndex,
            int pageSize,
            Guid? teacherId,
            Role? callerRole);

        Task<Course?> GetCourseDetailByID(
            Guid courseId);

        Task<Complaint?> GetComplaintDetailByID(
            Guid complaintId);

        Task<IEnumerable<Complaint>> GetPendingComplaintsAsync();

        Task<IEnumerable<Complaint>> GetTeacherApprovedComplaintsAsync(
            Guid teacherId);

        Task<int> CountApprovedByCourseAsync(
            Guid courseId);

        void ReplaceViolatedPolicies(
            Guid courseId,
            IEnumerable<ViolatedPolicy> newViolatedPolicies);

        void AddChapters(
            IEnumerable<Chapter> chapters);

        void AddLessons(
            IEnumerable<Lesson> lessons);

        void AddQuizzes(
            IEnumerable<Quiz> quizzes);

        void AddAssignments(
            IEnumerable<Assignment> assignments);

        void AddMaterials(
            IEnumerable<Material> materials);

        void CreateComplaint(
            Complaint complaint);

        void UpdateComplaint(
            Complaint complaint);
    }
}
