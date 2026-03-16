using Domain.CourseManagement.Aggregate;

namespace DataAccessLayer.Interface
{
    public interface IPolicyRepository :
        IGenericRepository<Policy>,
        IRepositoryBase
    {
        Task<IEnumerable<Policy>> GetDetailPolicies();
    }
}
