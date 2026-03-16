using DataAccessLayer.Interface;
using DataAccessLayer.Persistence;
using Domain.IdentityManagement.Aggregate;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Implementation
{
    public class UserRepository :
        GenericRepository<User>,
        IUserRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public UserRepository(EducationPlatformDBContext context) : base(context) { }

        #region Methods
        public async Task<User?> GetUserByEmail(string email)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByPhone(string phone)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Phone == phone);
        }

        public async Task<User?> GetByRefreshToken(string refreshToken)
        {
            var users = await context.Users
                .Where(u => u.RefreshToken != null)
                .ToListAsync();

            return users.FirstOrDefault(u =>
                u.RefreshToken.Verify(refreshToken));
        }

        public async Task<User?> GetUserByOTP(string otp)
        {
            return await context.Users
                .FirstOrDefaultAsync(u => u.EmailOtp == otp);
        }
        #endregion
    }
}
