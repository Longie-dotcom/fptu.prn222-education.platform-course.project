using DataAccessLayer.Interface;
using DataAccessLayer.Persistence;
using Domain.IdentityManagement.Aggregate;
using Domain.IdentityManagement.ValueObject;
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

        public async Task<(int TotalUsers, int TotalTeachers, int TotalStudents)> Summary(DateTime? from, DateTime? to)
        {
            var query = context.Users.AsQueryable();

            // ===== Apply date filters =====
            if (from.HasValue)
                query = query.Where(u => u.CreatedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(u => u.CreatedAt <= to.Value);

            var result = await query
                .GroupBy(u => 1)
                .Select(g => new
                {
                    TotalUsers = g.Count(),
                    TotalTeachers = g.Count(u => u.Role == Role.Teacher),
                    TotalStudents = g.Count(u => u.Role == Role.Student)
                })
                .FirstOrDefaultAsync();

            return result == null
                ? (0, 0, 0)
                : (result.TotalUsers, result.TotalTeachers, result.TotalStudents);
        }
        #endregion
    }
}
