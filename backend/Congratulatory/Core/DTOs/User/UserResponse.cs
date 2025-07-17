using CoreService.Core.DTOs.UserAccount;

namespace CoreService.Core.DTOs.User;

public class UserResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<UserAccountResponse> UserAccounts { get; set; } = new List<UserAccountResponse>();
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserResponse User { get; set; } = null!;
}