namespace CoreService.Core.DTOs.Birthday;

public class BirthdayFilterRequest
{
    public string? Name { get; set; }
    public int? Month { get; set; }
    public int? UpcomingDays { get; set; }
    public string? RelationshipType { get; set; }
}