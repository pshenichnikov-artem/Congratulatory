using System.ComponentModel.DataAnnotations;

namespace CoreService.Core.Attributes;

public class PastDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateTime date)
        {
            return date <= DateTime.Today;
        }
        return true;
    }
}