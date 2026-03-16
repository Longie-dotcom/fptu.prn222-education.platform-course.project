using DataAccessLayer.Interface;
using DataAccessLayer.Persistence;
using Domain.AcademicManagement.Aggregate;

namespace DataAccessLayer.Implementation
{
    public class GradeRepository :
        GenericRepository<Grade>,
        IGradeRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public GradeRepository(EducationPlatformDBContext context) : base(context) { }

        #region Methods
        #endregion
    }
}
