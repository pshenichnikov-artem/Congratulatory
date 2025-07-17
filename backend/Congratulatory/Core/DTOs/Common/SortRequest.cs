namespace CoreService.Common;

public class SortRequest
{
    public string? SortBy { get; set; }
    public SortDirection Direction { get; set; } = SortDirection.Asc;
}

public enum SortDirection
{
    Asc,
    Desc
}