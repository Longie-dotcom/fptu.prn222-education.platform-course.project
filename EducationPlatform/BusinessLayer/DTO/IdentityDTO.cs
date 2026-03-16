namespace BusinessLayer.DTO
{
    public class TokenDTO
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class LoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // View DTO
    public class UserDTO
    {
        public Guid UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
    }

    // Create DTO
    public class RegisterDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public int Role { get; set; } = 1;
    }

    // Update DTO

    // Delete DTO
}
