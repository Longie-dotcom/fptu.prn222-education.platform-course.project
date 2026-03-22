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
        public async Task<AnalyticsGrowthDTO> AnalyticsGrowth(QueryAnalyticsGrowthDTO dto)
        {
            // Resolve repositories
            var userRepo = unitOfWork.GetRepository<IUserRepository>();
            var courseRepo = unitOfWork.GetRepository<ICourseRepository>();
            var enrollmentRepo = unitOfWork.GetRepository<IEnrollmentRepository>();
            var orderRepo = unitOfWork.GetRepository<IOrderRepository>();

            // ===== Helper =====
            async Task<(string Name, Dictionary<string, decimal>)> ExecuteRange(
                DateTime? from,
                DateTime? to,
                string name)
            {
                Dictionary<string, List<(string Label, decimal Value)>> raw;

                switch (dto.Type)
                {
                    case AnalyticsGrowthType.User:
                        raw = await userRepo.AnalyticsGrowth(
                            from, to, dto.GroupBy.ToString(), dto.UserRole);
                        break;

                    case AnalyticsGrowthType.Course:
                        raw = await courseRepo.AnalyticsGrowth(
                            from, to, dto.GroupBy.ToString(),
                            dto.CourseGradeId, dto.CourseSubjectId);
                        break;

                    case AnalyticsGrowthType.Enrollment:
                        raw = await enrollmentRepo.AnalyticsGrowth(
                            from, to, dto.GroupBy.ToString(),
                            dto.EnrollmentGradeId, dto.EnrollmentSubjectId);
                        break;

                    case AnalyticsGrowthType.Revenue:
                        var revenueType = dto.RevenueType ?? AnalyticRevenueType.All;
                        raw = await orderRepo.AnalyticsGrowth(
                            from, to, dto.GroupBy.ToString(),
                            revenueType.ToString());
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var flattened = raw.FirstOrDefault();

                var dict = flattened.Value
                    .GroupBy(x => x.Label)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(x => x.Value)
                    );

                return (name, dict);
            }

            // ===== SEQUENTIAL EXECUTION =====
            var results = new List<(string Name, Dictionary<string, decimal>)>();

            // Main
            results.Add(await ExecuteRange(dto.From, dto.To, "Main"));

            // Comparison
            if (dto.ComparisonRanges != null)
            {
                foreach (var range in dto.ComparisonRanges)
                {
                    results.Add(await ExecuteRange(range.From, range.To, range.Label));
                }
            }

            // ===== Align labels =====
            var allLabels = results
                .SelectMany(r => r.Item2.Keys)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var seriesList = results.Select(r => new AnalyticsGrowthSeriesDTO
            {
                SeriesName = r.Name,
                Data = allLabels.Select(label => new AnalyticsGrowthPointDTO
                {
                    Label = label,
                    Value = r.Item2.ContainsKey(label) ? r.Item2[label] : 0
                }).ToList()
            }).ToList();

            // ===== DEBUG =====
            Console.WriteLine("=== AnalyticsGrowth DEBUG START ===");
            Console.WriteLine($"Type: {dto.Type}");
            Console.WriteLine($"GroupBy: {dto.GroupBy}");
            Console.WriteLine($"From: {dto.From}, To: {dto.To}");

            foreach (var series in seriesList)
            {
                Console.WriteLine($"--- Series: {series.SeriesName} ---");
                foreach (var point in series.Data)
                {
                    Console.WriteLine($"Label: {point.Label}, Value: {point.Value}");
                }
            }

            Console.WriteLine("=== AnalyticsGrowth DEBUG END ===");

            return new AnalyticsGrowthDTO
            {
                Type = dto.Type,
                Series = seriesList
            };
        }

        public async Task<AnalyticsGrowthDTO> AnalyticsDemandAndSupply(
            QueryAnalyticsGrowthDTO dto)
        {
            // Resolve repositories
            var courseRepo = unitOfWork.GetRepository<ICourseRepository>();
            var enrollmentRepo = unitOfWork.GetRepository<IEnrollmentRepository>();

            // ===== 1. Get data (SEQUENTIAL - IMPORTANT) =====
            var courseRaw = await courseRepo.AnalyticsGrowth(
                dto.From,
                dto.To,
                dto.GroupBy.ToString(),
                dto.CourseGradeId,
                dto.CourseSubjectId
            );

            var enrollmentRaw = await enrollmentRepo.AnalyticsGrowth(
                dto.From,
                dto.To,
                dto.GroupBy.ToString(),
                dto.EnrollmentGradeId,
                dto.EnrollmentSubjectId
            );

            // ===== 2. Flatten =====
            var courseDict = courseRaw.FirstOrDefault().Value
                .GroupBy(x => x.Label)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.Value)
                );

            var enrollmentDict = enrollmentRaw.FirstOrDefault().Value
                .GroupBy(x => x.Label)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.Value)
                );

            // ===== 3. Align labels =====
            var allLabels = courseDict.Keys
                .Union(enrollmentDict.Keys)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            // ===== 4. Build series =====
            var seriesList = new List<AnalyticsGrowthSeriesDTO>
            {
                new AnalyticsGrowthSeriesDTO
                {
                    SeriesName = "Supply (Courses)",
                    Data = allLabels.Select(label => new AnalyticsGrowthPointDTO
                    {
                        Label = label,
                        Value = courseDict.ContainsKey(label) ? courseDict[label] : 0
                    }).ToList()
                },
                new AnalyticsGrowthSeriesDTO
                {
                    SeriesName = "Demand (Enrollments)",
                    Data = allLabels.Select(label => new AnalyticsGrowthPointDTO
                    {
                        Label = label,
                        Value = enrollmentDict.ContainsKey(label) ? enrollmentDict[label] : 0
                    }).ToList()
                }
            };

            // ===== 5. DEBUG =====
            Console.WriteLine("=== Demand vs Supply DEBUG START ===");
            Console.WriteLine($"GroupBy: {dto.GroupBy}, From: {dto.From}, To: {dto.To}");

            foreach (var series in seriesList)
            {
                Console.WriteLine($"--- {series.SeriesName} ---");
                foreach (var point in series.Data)
                {
                    Console.WriteLine($"{point.Label}: {point.Value}");
                }
            }

            Console.WriteLine("=== Demand vs Supply DEBUG END ===");

            // ===== 6. Return =====
            return new AnalyticsGrowthDTO
            {
                Type = AnalyticsGrowthType.Enrollment, // or define new enum if you want
                Series = seriesList
            };
        }

        public async Task<AnalyticsGrowthDTO> AnalyticsNormalizedGrowth(
            QueryAnalyticsGrowthDTO dto)
        {
            var userRepo = unitOfWork.GetRepository<IUserRepository>();
            var courseRepo = unitOfWork.GetRepository<ICourseRepository>();
            var enrollmentRepo = unitOfWork.GetRepository<IEnrollmentRepository>();
            var orderRepo = unitOfWork.GetRepository<IOrderRepository>();

            // ===== 1. Get all raw data (SEQUENTIAL) =====
            var userRaw = await userRepo.AnalyticsGrowth(dto.From, dto.To, dto.GroupBy.ToString(), dto.UserRole);
            var courseRaw = await courseRepo.AnalyticsGrowth(dto.From, dto.To, dto.GroupBy.ToString(), dto.CourseGradeId, dto.CourseSubjectId);
            var enrollmentRaw = await enrollmentRepo.AnalyticsGrowth(dto.From, dto.To, dto.GroupBy.ToString(), dto.EnrollmentGradeId, dto.EnrollmentSubjectId);
            var revenueRaw = await orderRepo.AnalyticsGrowth(dto.From, dto.To, dto.GroupBy.ToString(), (dto.RevenueType ?? AnalyticRevenueType.All).ToString());

            // ===== 2. Flatten =====
            Dictionary<string, decimal> Flatten(Dictionary<string, List<(string Label, decimal Value)>> raw)
            {
                return raw.First().Value
                    .GroupBy(x => x.Label)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Value));
            }

            var userDict = Flatten(userRaw);
            var courseDict = Flatten(courseRaw);
            var enrollmentDict = Flatten(enrollmentRaw);
            var revenueDict = Flatten(revenueRaw);

            // ===== 3. Align labels =====
            var allLabels = userDict.Keys
                .Union(courseDict.Keys)
                .Union(enrollmentDict.Keys)
                .Union(revenueDict.Keys)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            // ===== 4. Normalize =====
            List<AnalyticsGrowthPointDTO> Normalize(Dictionary<string, decimal> dict)
            {
                var firstValue = dict.OrderBy(x => x.Key).FirstOrDefault().Value;

                return allLabels.Select(label =>
                {
                    var value = dict.ContainsKey(label) ? dict[label] : 0;

                    return new AnalyticsGrowthPointDTO
                    {
                        Label = label,
                        Value = firstValue == 0
                            ? 0
                            : Math.Round((value / firstValue) * 100, 2)
                    };
                }).ToList();
            }

            // ===== 5. Build series =====
            var series = new List<AnalyticsGrowthSeriesDTO>
            {
                new AnalyticsGrowthSeriesDTO { SeriesName = "Users", Data = Normalize(userDict) },
                new AnalyticsGrowthSeriesDTO { SeriesName = "Courses", Data = Normalize(courseDict) },
                new AnalyticsGrowthSeriesDTO { SeriesName = "Enrollments", Data = Normalize(enrollmentDict) },
                new AnalyticsGrowthSeriesDTO { SeriesName = "Revenue", Data = Normalize(revenueDict) }
            };

            // ===== DEBUG BLOCK =====
            Console.WriteLine("=== AnalyticsNormalizedGrowth DEBUG START ===");
            Console.WriteLine($"From: {dto.From}, To: {dto.To}, GroupBy: {dto.GroupBy}");

            void PrintDict(string name, Dictionary<string, decimal> dict)
            {
                Console.WriteLine($"--- {name} RAW ---");
                foreach (var kv in dict.OrderBy(x => x.Key))
                {
                    Console.WriteLine($"Label: {kv.Key}, Value: {kv.Value}");
                }
            }

            void PrintSeries(string name, List<AnalyticsGrowthPointDTO> data)
            {
                Console.WriteLine($"--- {name} NORMALIZED ---");
                foreach (var point in data)
                {
                    Console.WriteLine($"Label: {point.Label}, Value: {point.Value}");
                }
            }

            PrintDict("Users", userDict);
            PrintDict("Courses", courseDict);
            PrintDict("Enrollments", enrollmentDict);
            PrintDict("Revenue", revenueDict);

            Console.WriteLine("--- ALL LABELS ---");
            foreach (var label in allLabels)
            {
                Console.WriteLine(label);
            }

            foreach (var s in series)
            {
                PrintSeries(s.SeriesName, s.Data);
            }

            Console.WriteLine("=== AnalyticsNormalizedGrowth DEBUG END ===");
            // ===== END DEBUG BLOCK =====

            return new AnalyticsGrowthDTO
            {
                Type = dto.Type,
                Series = series
            };
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
