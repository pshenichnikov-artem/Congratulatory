using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreService.Core.Entities;

public class UserAccount
{
    [Key]
    public long Id { get; set; }
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }
    
    public string Platform { get; set; }
    public string UserName { get; set; }
    public long ChatId { get; set; }
    public bool IsVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<BirthdayNotification> BirthdayNotifications { get; set; } = new List<BirthdayNotification>();
}