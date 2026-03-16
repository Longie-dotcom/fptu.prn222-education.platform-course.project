using Domain.AIManagement.Aggregate;

namespace DataAccessLayer.Interface
{
    internal interface IAIImprovementSessionRepository :
        IGenericRepository<AIImprovementSession>,
        IRepositoryBase
    {
    }
}
