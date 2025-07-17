namespace BirthdayNotificationWorker.Models;

public class BirthdayNotificationDto
{
    public long Id { get; set; }
    public string BirthdayPersonName { get; set; }
    public long ChatId { get; set; }
    public string Platform { get; set; }
    public string RelationshipType { get; set; }
    public string MessageText { get; set; }
}