using AutoMapper;
using BusinessLayer.BusinessException;
using BusinessLayer.DTO;
using BusinessLayer.Interface;
using DataAccessLayer.Interface;
using Domain.CourseManagement.Aggregate;
using Domain.CourseManagement.Entity;
using Domain.CourseManagement.Enum;
using Domain.IdentityManagement.ValueObject;
using Domain.OrderManagement.Aggregate;

namespace BusinessLayer.Implementation
{
    public class CourseService : ICourseService
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        #endregion

        #region Properties
        #endregion

        public CourseService(
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        #region Methods
        public async Task<IEnumerable<PolicyDTO>> GetPolicies()
        {
            // Validate policies list existence
            var list = await unitOfWork
                .GetRepository<IPolicyRepository>()
                .GetDetailPolicies();

            if (list == null || !list.Any())
                throw new NotFound(
                    "The policy list is not found or empty");

            return mapper.Map<IEnumerable<PolicyDTO>>(list);
        }

        public async Task<IEnumerable<CourseDTO>> GetCourses(
             QueryCourseDTO dto,
             Guid callerId,
             string callerRole)
        {
            // ---- Role parsing ----
            if (!Enum.TryParse<Role>(callerRole, true, out var role))
                throw new AuthenticateException(
                    "Invalid role");

            // ---- Teacher scoping ----
            Guid? teacherId = role == Role.Teacher ? callerId : null;

            var list = await unitOfWork
                .GetRepository<ICourseRepository>()
                .GetAllCourses(
                    dto.Title,
                    dto.Price,
                    dto.TeacherName,
                    dto.GradeName,
                    dto.SubjectName,
                    dto.PageIndex,
                    dto.PageSize,
                    teacherId,
                    role);

            if (list == null || !list.Any())
                throw new NotFound(
                    "Course list is not found or empty");

            return mapper.Map<IEnumerable<CourseDTO>>(list);
        }

        public async Task<IEnumerable<CourseDTO>> DiscoverCourses(QueryCourseDTO dto)
        {
            // No callerId or role needed for discovery
            var list = await unitOfWork
                .GetRepository<ICourseRepository>()
                .GetAllCourses(
                    dto.Title,
                    dto.Price,
                    dto.TeacherName,
                    dto.GradeName,
                    dto.SubjectName,
                    dto.PageIndex,
                    dto.PageSize,
                    teacherId: null, // no teacher restriction
                    callerRole: null       // no role restriction
                );

            if (list == null || !list.Any())
                throw new NotFound("No courses available for discovery.");

            return mapper.Map<IEnumerable<CourseDTO>>(list);
        }

        public async Task<CourseDetailDTO> GetCourseDetail(
            Guid courseId,
            Guid callerId,
            string callerRole)
        {
            // Validate course list existence
            var course = await unitOfWork
                .GetRepository<ICourseRepository>()
                .GetCourseDetailByID(courseId);

            if (course == null)
                throw new NotFound(
                    $"Course with ID: {courseId} is not found");

            // Mapping
            var dto = mapper.Map<CourseDetailDTO>(course);

            // Restriction: Hide lessons from student users
            if (!Enum.TryParse<Role>(callerRole, true, out var role))
                throw new AuthenticateException(
                    "Invalid role");

            if (role == Role.Student)
                dto.Chapters = new List<ChapterDTO>();

            return dto;
        }

        public async Task<CourseDetailDTO> DiscoverCourseDetail(Guid courseId)
        {
            // Fetch the course detail
            var course = await unitOfWork
                .GetRepository<ICourseRepository>()
                .GetCourseDetailByID(courseId);

            if (course == null)
                throw new NotFound($"Course with ID: {courseId} is not found");

            // Map to DTO
            var dto = mapper.Map<CourseDetailDTO>(course);

            // Restriction: Hide chapters/lessons for public users
            dto.Chapters = new List<ChapterDTO>();

            return dto;
        }

        public async Task CreateCourse(
            CreateCourseDTO dto,
            Guid callerId,
            string callerRole)
        {
            // Apply domain
            // Restore course
            var course = new Course(
                Guid.NewGuid(),
                dto.Title,
                dto.Description,
                dto.Price,
                dto.ThumbnailName,
                dto.Slug,
                dto.Prerequisites,
                dto.LearningOutcomes,
                callerId,
                dto.GradeID,
                dto.SubjectID);
            var chapters = new List<Chapter>();
            var lessons = new List<Lesson>();
            var quizzes = new List<Quiz>();
            var assignments = new List<Assignment>();
            var materials = new List<Material>();

            foreach (var chapterDto in dto.Chapters)
            {
                var chapter = course.AddChapter(
                    chapterDto.Title,
                    chapterDto.Description,
                    chapterDto.Order);

                foreach (var lessonDto in chapterDto.Lessons)
                {
                    var lesson = chapter.AddLesson(
                        lessonDto.Title,
                        lessonDto.Objectives,
                        lessonDto.Description,
                        lessonDto.VideoUrl);

                    foreach (var quizDto in lessonDto.Quizzes)
                    {
                        var quiz = lesson.AddQuiz(
                            quizDto.Question,
                            quizDto.Note);

                        quiz.AddAnswer(
                            (QuizType)quizDto.Answer.Type,
                            quizDto.Answer.CorrectAnswers,
                            quizDto.Answer.Options,
                            quizDto.Answer.TrueOrFalse);

                        // Restore quiz
                        quizzes.Add(quiz);
                    }

                    foreach (var materialDto in lessonDto.Materials)
                    {
                        var material = lesson.AddMaterial(
                            materialDto.Name,
                            materialDto.Description,
                            materialDto.Url,
                            materialDto.Type);

                        // Restore material
                        materials.Add(material);
                    }

                    foreach (var assignmentDto in lessonDto.Assignments)
                    {
                        var assignment = lesson.AddAssignment(
                            assignmentDto.Title,
                            assignmentDto.Description,
                            assignmentDto.MaxScore);

                        // Restore assignment
                        assignments.Add(assignment);
                    }

                    // Restore lesson
                    lessons.Add(lesson);
                }

                chapters.Add(chapter);
            }

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<ICourseRepository>()
                .Add(course);
            unitOfWork
                .GetRepository<ICourseRepository>()
                .AddChapters(chapters);
            unitOfWork
                .GetRepository<ICourseRepository>()
                .AddLessons(lessons);
            unitOfWork
                .GetRepository<ICourseRepository>()
                .AddQuizzes(quizzes);
            unitOfWork
                .GetRepository<ICourseRepository>()
                .AddAssignments(assignments);
            unitOfWork
                .GetRepository<ICourseRepository>()
                .AddMaterials(materials);
            await unitOfWork.CommitAsync(callerId.ToString());
        }

        public async Task ReviewCourse(
            ReviewCourseDTO dto,
            Guid callerId,
            string callerRole)
        {
            // Validate course existence
            var course = await unitOfWork
                .GetRepository<ICourseRepository>()
                .GetCourseDetailByID(dto.CourseID);

            if (course == null)
                throw new NotFound($"Course with ID: {dto.CourseID} is not found");


            // Convert DTO → tuple
            var violatedChapters = dto.ViolatedChapters?
                .Select(x => (x.ViolatedChapterId, x.AdminNote))
                .ToList();


            // Apply domain
            var violatedPolicies = course.ReviewCourse(
                dto.ViolatedPolicyIDs,
                violatedChapters,
                dto.AdminNote);


            // Persistence
            await unitOfWork.BeginTransactionAsync();

            unitOfWork
                .GetRepository<ICourseRepository>()
                .Update(course.CourseID, course);

            unitOfWork
                .GetRepository<ICourseRepository>()
                .ReplaceViolatedPolicies(course.CourseID, violatedPolicies);

            await unitOfWork.CommitAsync(callerId.ToString());
        }

        // Complaint
        public async Task<IEnumerable<ComplaintDTO>> GetComplaints(
            ComplaintStatus? status,
            Guid callerId,
            string userRole)
        {
            var list = await unitOfWork
                .GetRepository<ICourseRepository>()
                .GetComplaintsAsync(status, userRole == Role.Teacher.ToString() ? callerId : null);

            if (list == null || !list.Any())
                throw new NotFound("No pending complaints");

            return mapper.Map<List<ComplaintDTO>>(list);
        }

        public async Task<ComplaintDetailDTO> GetComplaintDetail(Guid complaintId)
        {
            var complaint = await unitOfWork
                .GetRepository<ICourseRepository>()
                .GetComplaintDetailByID(complaintId);

            if (complaint == null)
                throw new NotFound($"Complaint with ID: {complaintId} not found");

            return mapper.Map<ComplaintDetailDTO>(complaint);
        }

        public async Task CreateComplaint(CreateComplaintDTO dto, Guid studentId)
        {
            // Validate enrollment
            var enrollments = await unitOfWork
                .GetRepository<IEnrollmentRepository>()
                .GetStudentEnrollments(studentId);

            var isEnrolled = enrollments.Any(e => e.CourseID == dto.CourseID);
            if (!isEnrolled)
                throw new Conflict(
                    "You can only submit complaints for courses you have enrolled in.");

            // Apply domain
            var complaint = new Complaint(
                Guid.NewGuid(),
                dto.CourseID,
                studentId,
                dto.Reason,
                dto.EvidenceImagePath);

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<ICourseRepository>()
                .CreateComplaint(complaint);
            await unitOfWork.CommitAsync(studentId.ToString());
        }

        public async Task ReviewComplaint(ReviewComplaintDTO dto, Guid adminId)
        {
            var courseRepo = unitOfWork.GetRepository<ICourseRepository>();

            var complaint = await courseRepo.GetComplaintDetailByID(dto.ComplaintID)
                ?? throw new NotFound($"Complaint with ID: {dto.ComplaintID} is not found");

            if (complaint.Status != ComplaintStatus.Pending)
                throw new Conflict("This complaint has already been reviewed.");

            await unitOfWork.BeginTransactionAsync();

            if (dto.IsApproved)
            {
                // Approve (in-memory)
                complaint.Approve(dto.AdminNote);

                // Get already-approved complaints
                var approvedComplaints = await courseRepo
                    .GetApprovedByCoursesAsync(complaint.CourseID);

                var totalApproved = approvedComplaints.Count() + 1;

                if (totalApproved >= 2)
                {
                    var course = complaint.Course
                        ?? throw new NotFound($"Course with ID: {complaint.CourseID} is not found");

                    // Reject course
                    course.RejectByComplaint(
                        "Course removed due to multiple approved complaints.");

                    var orderRepo = unitOfWork.GetRepository<IOrderRepository>();

                    // Get enrolled students
                    var students = await unitOfWork
                        .GetRepository<IEnrollmentRepository>()
                        .GetEnrolledStudentIdsByCourseId(course.CourseID);

                    // 7.5% per student
                    var couponAmount = course.Price.Amount * 0.075m;

                    // Create coupons
                    var coupons = students.Select(studentId =>
                        new Coupon(
                            Guid.NewGuid(),
                            studentId,
                            $"CMP-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                            couponAmount,
                            "Compensation for removed course"
                        )).ToList();

                    orderRepo.CreateCoupons(coupons);

                    // Total penalty = total coupons
                    if (coupons.Any())
                    {
                        var totalPenaltyAmount = coupons.Sum(c => c.DiscountAmount);

                        var penalty = new Penalty(
                            Guid.NewGuid(),
                            course.TeacherID,
                            course.CourseID,
                            totalPenaltyAmount,
                            $"Penalty equals total compensation for removed course for {students.Count()} enrollments"
                        );

                        orderRepo.CreatePenalty(penalty);
                    }

                    // Remove complaints (including current one)
                    var allToRemove = approvedComplaints.Append(complaint).ToList();
                    courseRepo.RemoveComplaints(allToRemove);
                }
            }
            else
            {
                complaint.Reject(dto.AdminNote);
            }

            // Persist everything
            courseRepo.UpdateComplaint(complaint);

            await unitOfWork.CommitAsync(adminId.ToString());
        }
        #endregion
    }
}
