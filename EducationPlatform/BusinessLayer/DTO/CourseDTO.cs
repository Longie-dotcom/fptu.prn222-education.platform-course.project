using Domain.CourseManagement.Aggregate;
using Domain.CourseManagement.Enum;
using Domain.IdentityManagement.Aggregate;

namespace BusinessLayer.DTO
{
    // View DTO
    public class CourseDTO
    {
        public Guid CourseID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string? ThumbnailName { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string? Prerequisites { get; set; } = string.Empty;
        public string? LearningOutcomes { get; set; } = string.Empty;
        public DateTime? RejectedAt { get; set; } 
        public DateTime? PublishedAt { get; set; }
        public UserDTO Teacher { get; set; } = new UserDTO();
        public GradeDTO Grade { get; set; } = new GradeDTO();
        public SubjectDTO Subject { get; set; } = new SubjectDTO();
    }

    public class CourseDetailDTO
    {
        public Guid CourseID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string? ThumbnailName { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string? Prerequisites { get; set; } = string.Empty;
        public string? LearningOutcomes { get; set; } = string.Empty;
        public DateTime? RejectedAt { get; set; }
        public string? AdminNote { get; set; }
        public DateTime? PublishedAt { get; set; }
        public UserDTO Teacher { get; set; } = new UserDTO();
        public GradeDTO Grade { get; set; } = new GradeDTO();
        public SubjectDTO Subject { get; set; } = new SubjectDTO();
        public List<ViolatedPolicyDTO> ViolatedPolicies { get; set; } = new List<ViolatedPolicyDTO>();
        public List<ChapterDTO> Chapters { get; set; } = new List<ChapterDTO>();
    }

    public class QueryCourseDTO
    {
        public string? Title { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string? TeacherName { get; set; } = string.Empty;
        public string? GradeName { get; set; } = string.Empty;
        public string? SubjectName { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 1;
    }

    public class ComplaintDTO
    {
        public Guid ComplaintID { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? EvidenceImagePath { get; set; }
        public ComplaintStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? AdminNote { get; set; }
        public Guid CourseID { get; set; }
        public Guid StudentID { get; set; }

        // Populated field
        public string CourseTitle { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
    }

    public class ComplaintDetailDTO
    {
        public Guid ComplaintID { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? EvidenceImagePath { get; set; }
        public ComplaintStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? AdminNote { get; set; }
        public Guid CourseID { get; set; }
        public Guid StudentID { get; set; }
        public CourseDTO Course { get; set; } = new CourseDTO();
        public UserDTO User { get; set; } = new UserDTO();
    }

    public class ViolatedPolicyDTO
    {
        public Guid ViolatedPolicyID { get; set; }
        public Guid PolicyID { get; set; }
        public Guid CourseID { get; set; }
        public PolicyDTO Policy { get; set; } = new PolicyDTO();
    }

    public class ChapterDTO
    {
        public Guid ChapterID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }
        public bool IsViolated { get; set; }
        public string? AdminNote { get; set; }
        public Guid CourseID { get; set; }
        public List<LessonDTO> Lessons { get; set; } = new List<LessonDTO>();
    }

    public class LessonDTO
    {
        public Guid LessonID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Objectives { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsViolated { get; set; }
        public string? AdminNote { get; set; }
        public Guid CourseID { get; set; }
        public List<QuizDTO> Quizzes { get; set; } = new List<QuizDTO>();
        public List<AssignmentDTO> Assignments { get; set; } = new List<AssignmentDTO>();
        public List<MaterialDTO> Materials { get; set; } = new List<MaterialDTO>();
    }

    public class QuizDTO
    {
        public Guid QuizID { get; set; }
        public string Question { get; set; } = string.Empty;
        public string? Note { get; set; }
        public Guid LessonID { get; set; }
        public QuizAnswerDTO Answer { get; set; } = new QuizAnswerDTO();
    }

    public class QuizAnswerDTO
    {
        public QuizType Type { get; set; }
        public List<string> CorrectAnswers { get; set; } = new List<string>();
        public List<string>? Options { get; set; } = new List<string>();
    }

    public class AssignmentDTO
    {
        public Guid AssignmentID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxScore { get; set; }
        public Guid LessonID { get; set; }
    }

    public class MaterialDTO
    {
        public Guid MaterialID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public MaterialType Type { get; set; }
        public Guid LessonID { get; set; }
    }

    public class PolicyDTO
    {
        public Guid PolicyID { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<PolicyRuleDTO> PolicyRules { get; set; } = new List<PolicyRuleDTO>();
    }

    public class PolicyRuleDTO
    {
        public Guid PolicyRuleID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid PolicyID { get; set; }
    }

    // Create DTO
    public class CreateCourseDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string ThumbnailName { get; set; } = string.Empty;
        public string? Slug { get; set; } = string.Empty;
        public string Prerequisites { get; set; } = string.Empty;
        public string LearningOutcomes { get; set; } = string.Empty;
        public Guid GradeID { get; set; }
        public Guid SubjectID { get; set; }
        public List<CreateChapterDTO> Chapters { get; set; } = new List<CreateChapterDTO>();
    }

    public class CreateChapterDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<CreateLessonDTO> Lessons { get; set; } = new List<CreateLessonDTO>();
    }

    public class CreateLessonDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Objectives { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<CreateQuizDTO> Quizzes { get; set; } = new List<CreateQuizDTO>();
        public List<CreateAssignmentDTO> Assignments { get; set; } = new List<CreateAssignmentDTO>();
        public List<CreateMaterialDTO> Materials { get; set; } = new List<CreateMaterialDTO>();
    }

    public class CreateQuizDTO
    {
        public string Question { get; set; } = string.Empty;
        public string? Note { get; set; }
        public CreateQuizAnswerDTO Answer { get; set; } = new CreateQuizAnswerDTO();
    }

    public class CreateQuizAnswerDTO
    {
        public int Type { get; set; } = 1;
        public List<string>? CorrectAnswers { get; set; } = new List<string>();
        public List<string>? Options { get; set; } = new List<string>();
        public bool? TrueOrFalse { get; set; } = false;
    }

    public class CreateAssignmentDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxScore { get; set; }
    }

    public class CreateMaterialDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public MaterialType Type { get; set; }
    }

    public class CreateComplaintDTO
    {
        public Guid CourseID { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? EvidenceImagePath { get; set; }
    }

    public class ReviewComplaintDTO
    {
        public Guid ComplaintID { get; set; }
        public bool IsApproved { get; set; }
        public string? AdminNote { get; set; }
    }

    public class ReviewCourseDTO
    {
        public Guid CourseID { get; set; }
        public List<Guid>? ViolatedPolicyIDs { get; set; } = new List<Guid>();
        public List<ViolatedChapterDTO>? ViolatedChapters { get; set; } = new List<ViolatedChapterDTO>();
        public string? AdminNote { get; set; } = string.Empty;
    }

    public class ViolatedChapterDTO
    {
        public Guid ViolatedChapterId { get; set; }
        public string AdminNote { get; set; } = string.Empty;
    }
}
