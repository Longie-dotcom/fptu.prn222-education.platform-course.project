using Domain.AuditManagement.Aggregate;

namespace DataAccessLayer.Interface
{
    public interface IAuditLogRepository :
        IGenericRepository<AuditLog>,
        IRepositoryBase
    {
    }
}
