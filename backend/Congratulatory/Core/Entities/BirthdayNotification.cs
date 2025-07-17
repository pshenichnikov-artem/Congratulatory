using System.ComponentModel.DataAnnotations;

namespace CoreService.Core.Entities;

public class BirthdayNotification
{
    [Key]
    public long Id { get; set; }
    
    public long BirthdayId { get; set; }
    public Birthday Birthday { get; set; }
    
    public long UserAccountId { get; set; }
    public UserAccount UserAccount { get; set; }
    
    public DateTime NotificationDate { get; set; }
    public bool IsSent { get; set; } = false;
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}