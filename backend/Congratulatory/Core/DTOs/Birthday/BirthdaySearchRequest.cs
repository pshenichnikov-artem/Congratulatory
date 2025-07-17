using CoreService.Common;

namespace CoreService.Core.DTOs.Birthday;

public class BirthdaySearchRequest
{
    public BirthdayFilterRequest Filter { get; set; } = new();
    public SortRequest Sort { get; set; } = new();
}