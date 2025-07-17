using CoreService.Core.DTOs.UserAccount;

namespace CoreService.Core.DTOs.BirthdayNotification;

public class BirthdayNotificationResponse
{
    public long Id { get; set; }
    public long BirthdayId { get; set; }
    public UserAccountResponse UserAccount { get; set; }
    public string NotificationDate { get; set; }
}