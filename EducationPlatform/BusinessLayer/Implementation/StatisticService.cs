using AutoMapper;
using BusinessLayer.DTO;
using BusinessLayer.Enums;
using BusinessLayer.Interface;
using DataAccessLayer.Interface;

namespace BusinessLayer.Implementation
{
    public class StatisticService : IStatisticService
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        #endregion

        #region Properties
        #endregion

        public StatisticService(
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        #region Methods
        public Task<AnalyticsGrowthDTO> AnalyticsDemandAndSupply(
            QueryAnalyticsGrowthDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<AnalyticsGrowthDTO> AnalyticsGrowth(
            QueryAnalyticsGrowthDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<AnalyticsGrowthDTO> AnalyticsNormalizedGrowth(
            QueryAnalyticsGrowthDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<TopPerformanceDTO> GetTopPerformance(
            QueryTopPerformanceDTO query)
        {
            throw new NotImplementedException();
        }

        public async Task<SummaryStatisticDTO> SummaryStatistic(
            QuerySummaryDTO dto)
        {
            // Resolve repositories
            var userRepo = unitOfWork.GetRepository<IUserRepository>();
            var courseRepo = unitOfWork.GetRepository<ICourseRepository>();
            var enrollmentRepo = unitOfWork.GetRepository<IEnrollmentRepository>();
            var orderRepo = unitOfWork.GetRepository<IOrderRepository>();

            // ===== Execute SEQUENTIALLY (IMPORTANT) =====

            // User
            var (totalUsers, totalTeachers, totalStudents) =
                await userRepo.Summary(dto.From, dto.To);

            // Course
            var (inReview, rejected, published, courseGradeDict, courseSubjectDict) =
                await courseRepo.Summary(dto.From, dto.To);

            // Enrollment
            var (totalEnrollments, completedEnrollments, enrollmentGradeDict, enrollmentSubjectDict) =
                await enrollmentRepo.Summary(dto.From, dto.To);

            // Revenue
            var (totalRevenue, commission, teacherFinance) =
                await orderRepo.Summary(dto.From, dto.To);

            // ===== Build DTO =====
            return new SummaryStatisticDTO
            {
                User = new SummaryUserDTO
                {
                    Total = totalUsers,
                    TeacherCount = totalTeachers,
                    StudentCount = totalStudents
                },

                Course = new SummaryCourseDTO
                {
                    Total = inReview + rejected + published,
                    InReviewCount = inReview,
                    RejectedCount = rejected,
                    PublishedCount = published
                },

                Enrollment = new SummaryEnrollmentDTO
                {
                    Total = totalEnrollments,
                    Completed = completedEnrollments,
                    NotCompleted = totalEnrollments - completedEnrollments
                },

                Revenue = new SummaryRevenueDTO
                {
                    Total = totalRevenue,
                    Commission = commission,
                    TeacherFinance = teacherFinance
                },

                CourseByGrade = new SummaryCourseGradeDTO
                {
                    Total = courseGradeDict.Values.Sum(),
                    GradeCounts = courseGradeDict
                },

                CourseBySubject = new SummaryCourseSubjectDTO
                {
                    Total = courseSubjectDict.Values.Sum(),
                    SubjectCounts = courseSubjectDict
                },

                EnrollmentByGrade = new SummaryEnrollmentGradeDTO
                {
                    Total = enrollmentGradeDict.Values.Sum(),
                    GradeCounts = enrollmentGradeDict
                },

                EnrollmentBySubject = new SummaryEnrollmentSubjectDTO
                {
                    Total = enrollmentSubjectDict.Values.Sum(),
                    SubjectCounts = enrollmentSubjectDict
                }
            };
        }


        #endregion
    }
}
