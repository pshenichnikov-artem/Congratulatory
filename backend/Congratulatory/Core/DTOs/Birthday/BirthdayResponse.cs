using CoreService.Core.DTOs.BirthdayNotification;

namespace CoreService.Core.DTOs.Birthday;

public class BirthdayResponse
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string RelationshipType { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
    public List<BirthdayNotificationResponse> Notifications { get; set; }
}