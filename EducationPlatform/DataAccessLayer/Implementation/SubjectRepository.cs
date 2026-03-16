using DataAccessLayer.Interface;
using DataAccessLayer.Persistence;
using Domain.AcademicManagement.Aggregate;
using Domain.AcademicManagement.Entity;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Implementation
{
    public class SubjectRepository :
        GenericRepository<Subject>,
        ISubjectRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public SubjectRepository(EducationPlatformDBContext context) : base(context) { }

        #region Methods
        public async Task<IEnumerable<DefaultLesson>> GetDefaultLessons(
            Guid subjectId,
            Guid gradeId)
        {
            return await context.Set<DefaultLesson>()
                .Where(x => x.SubjectID == subjectId && x.GradeID == gradeId)
                .ToListAsync();
        }
        #endregion
    }
}
