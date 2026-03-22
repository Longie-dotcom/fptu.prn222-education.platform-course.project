using BusinessLayer.DTO;
using Domain.CourseManagement.Enum;

namespace BusinessLayer.Interface
{
    public interface ICourseService
    {
        Task<IEnumerable<PolicyDTO>> GetPolicies();

        Task<IEnumerable<CourseDTO>> GetCourses(
            QueryCourseDTO dto,
            Guid callerId, 
            string callerRole);

        Task<IEnumerable<CourseDTO>> DiscoverCourses(
            QueryCourseDTO dto);

        Task<CourseDetailDTO> GetCourseDetail(
            Guid courseId,
            Guid callerId,
            string callerRole);

        Task<CourseDetailDTO> GetCourseDetail(
            Guid courseId);

        Task<CourseDetailDTO> DiscoverCourseDetail(
            Guid courseId);

        Task CreateCourse(
            CreateCourseDTO dto, 
            Guid callerId, 
            string callerRole);

        Task ReviewCourse(
            ReviewCourseDTO dto,
            Guid callerId,
            string callerRole);

        // Complaint
        Task<IEnumerable<ComplaintDTO>> GetComplaints(
            ComplaintStatus? status,
            Guid callerId,
            string userRole);

        Task<ComplaintDetailDTO> GetComplaintDetail(
            Guid complaintId);

        Task CreateComplaint(
            CreateComplaintDTO dto,
            Guid studentId);

        Task ReviewComplaint(
            ReviewComplaintDTO dto,
            Guid adminId);
    }
}
