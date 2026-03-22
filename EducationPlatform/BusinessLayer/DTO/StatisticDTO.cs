using BusinessLayer.Enums;

namespace BusinessLayer.DTO
{
    // ======================= SUMMARY =======================
    public class QuerySummaryDTO
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }

    public class SummaryStatisticDTO
    {
        public SummaryUserDTO User { get; set; } = new();
        public SummaryCourseDTO Course { get; set; } = new();
        public SummaryEnrollmentDTO Enrollment { get; set; } = new();
        public SummaryRevenueDTO Revenue { get; set; } = new();
        public SummaryCourseGradeDTO CourseByGrade { get; set; } = new();
        public SummaryCourseSubjectDTO CourseBySubject { get; set; } = new();
        public SummaryEnrollmentGradeDTO EnrollmentByGrade { get; set; } = new();
        public SummaryEnrollmentSubjectDTO EnrollmentBySubject { get; set; } = new();
    }

    public class SummaryUserDTO
    {
        public int Total { get; set; }
        public int StudentCount { get; set; }
        public int TeacherCount { get; set; }
    }

    public class SummaryCourseDTO
    {
        public int Total { get; set; }
        public int InReviewCount { get; set; }
        public int RejectedCount { get; set; }
        public int PublishedCount { get; set; }
    }

    public class SummaryEnrollmentDTO
    {
        public int Total { get; set; }
        public int Completed { get; set; }
        public int NotCompleted { get; set; }
    }

    public class SummaryRevenueDTO
    {
        public int Total { get; set; }
        public int Commission { get; set; }
        public int TeacherFinance { get; set; }
    }

    public class SummaryCourseGradeDTO
    {
        public int Total { get; set; }
        public Dictionary<string, int> GradeCounts { get; set; } = new();
    }

    public class SummaryCourseSubjectDTO
    {
        public int Total { get; set; }
        public Dictionary<string, int> SubjectCounts { get; set; } = new();
    }

    public class SummaryEnrollmentGradeDTO
    {
        public int Total { get; set; }
        public Dictionary<string, int> GradeCounts { get; set; } = new();
    }

    public class SummaryEnrollmentSubjectDTO
    {
        public int Total { get; set; }
        public Dictionary<string, int> SubjectCounts { get; set; } = new();
    }

    // ======================= ANALYTICS =======================
    public class QueryAnalyticsGrowthDTO
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public AnalyticGroupDate GroupBy { get; set; } = AnalyticGroupDate.Month;
        public List<AnalyticsTimeRangeDTO>? ComparisonRanges { get; set; }
        public AnalyticsGrowthType Type { get; set; }

        // ===== Users =====
        public string? UserRole { get; set; } // Student / Teacher

        // ===== Courses =====
        public Guid? CourseGradeId { get; set; }
        public Guid? CourseSubjectId { get; set; }

        // ===== Enrollments =====
        public Guid? EnrollmentGradeId { get; set; }
        public Guid? EnrollmentSubjectId { get; set; }

        // ===== Revenue =====
        public AnalyticRevenueType? RevenueType { get; set; } = AnalyticRevenueType.All; 
    }

    public class AnalyticsTimeRangeDTO
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Label { get; set; } = default!;
    }

    public class AnalyticsGrowthDTO
    {
        public AnalyticsGrowthType Type { get; set; }
        public List<AnalyticsGrowthSeriesDTO> Series { get; set; } = new();
    }

    public class AnalyticsGrowthSeriesDTO
    {
        public string SeriesName { get; set; } = default!;
        public List<AnalyticsGrowthPointDTO> Data { get; set; } = new();
    }

    public class AnalyticsGrowthPointDTO
    {
        public string Label { get; set; } = default!;
        public decimal Value { get; set; }
    }

    // ======================= INSIGHT =======================
    public class QueryTopPerformanceDTO
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public TopPerformanceGroupBy GroupBy { get; set; } = TopPerformanceGroupBy.OfAllTime;

        // Filters
        public Guid? GradeId { get; set; }
        public Guid? SubjectId { get; set; }

        // Top N
        public int Top { get; set; } = 10;
    }

    public class TopPerformanceDTO
    {
        // Courses
        public List<TopCourseByEnrollmentDTO> CoursesByEnrollment { get; set; } = new();
        public List<TopCourseByRevenueDTO> CoursesByRevenue { get; set; } = new();

        // Subjects
        public List<TopSubjectByEnrollmentDTO> SubjectsByEnrollment { get; set; } = new();
        public List<TopSubjectByRevenueDTO> SubjectsByRevenue { get; set; } = new();

        // Grades
        public List<TopGradeByEnrollmentDTO> GradesByEnrollment { get; set; } = new();
        public List<TopGradeByRevenueDTO> GradesByRevenue { get; set; } = new();
    }

    public class TopCourseByEnrollmentDTO
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public decimal EnrollmentCount { get; set; }
    }

    public class TopCourseByRevenueDTO
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class TopSubjectByEnrollmentDTO
    {
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public decimal EnrollmentCount { get; set; }
    }

    public class TopSubjectByRevenueDTO
    {
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class TopGradeByEnrollmentDTO
    {
        public Guid GradeId { get; set; }
        public string GradeName { get; set; } = string.Empty;
        public decimal EnrollmentCount { get; set; }
    }

    public class TopGradeByRevenueDTO
    {
        public Guid GradeId { get; set; }
        public string GradeName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
}
