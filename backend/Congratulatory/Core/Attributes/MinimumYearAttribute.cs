using System.ComponentModel.DataAnnotations;

namespace CoreService.Core.Attributes;

public class MinimumYearAttribute : ValidationAttribute
{
    private readonly int _minimumYear;

    public MinimumYearAttribute(int minimumYear)
    {
        _minimumYear = minimumYear;
    }

    public override bool IsValid(object? value)
    {
        if (value is DateTime date)
        {
            return date.Year >= _minimumYear;
        }
        return true;
    }
}