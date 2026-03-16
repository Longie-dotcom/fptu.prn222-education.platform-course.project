using BusinessLayer.DTO;

namespace BusinessLayer.Interface
{
    public interface IIdentityService
    {
        Task<TokenDTO> Login(
            LoginDTO dto);

        Task Register(
            RegisterDTO dto);

        Task VerifyEmail(
            string otp);

        Task<TokenDTO> RefreshToken(
            string refreshToken);
    }
}
