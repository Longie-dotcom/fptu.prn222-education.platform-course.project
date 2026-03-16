using DataAccessLayer.Interface;
using DataAccessLayer.Persistence;
using Domain.AIManagement.Aggregate;

namespace DataAccessLayer.Implementation
{
    public class AIImprovementSessionRepository :
        GenericRepository<AIImprovementSession>,
        IAIImprovementSessionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public AIImprovementSessionRepository(EducationPlatformDBContext context) : base(context) { }

        #region Methods
        #endregion
    }
}
