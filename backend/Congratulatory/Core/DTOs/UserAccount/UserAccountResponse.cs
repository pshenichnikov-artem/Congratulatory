namespace CoreService.Core.DTOs.UserAccount;

public class UserAccountResponse
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public string Platform { get; set; }
    public string UserName { get; set; }
    public long ChatId { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}