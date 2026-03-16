using Domain.AcademicManagement.Aggregate;

namespace DataAccessLayer.Interface
{
    public interface IGradeRepository : 
        IGenericRepository<Grade>,
        IRepositoryBase
    {
    }
}
