using Domain.IdentityManagement.Aggregate;

namespace DataAccessLayer.Interface
{
    public interface IUserRepository :
        IGenericRepository<User>,
        IRepositoryBase
    {
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByPhone(string phone);
        Task<User?> GetByRefreshToken(string refreshToken);
        Task<User?> GetUserByOTP(string otp);
    }
}
