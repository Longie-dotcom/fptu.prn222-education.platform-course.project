using DataAccessLayer.Interface;
using DataAccessLayer.Persistence;
using Domain.AuditManagement.Aggregate;

namespace DataAccessLayer.Implementation
{
    public class AuditLogRepository :
        GenericRepository<AuditLog>,
        IAuditLogRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public AuditLogRepository(EducationPlatformDBContext context) : base(context) { }

        #region Methods
        #endregion
    }
}
