namespace DataAccessLayer.Interface
{
    public interface IUnitOfWork
    {
        T GetRepository<T>() where T : IRepositoryBase;

        Task BeginTransactionAsync();
        Task<int> CommitAsync(string? performedBy = null);
    }

    public interface IRepositoryBase
    {

    }
}
