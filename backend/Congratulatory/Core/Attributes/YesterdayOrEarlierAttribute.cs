using System.ComponentModel.DataAnnotations;

namespace CoreService.Core.Attributes;

public class YesterdayOrEarlierAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTime dateOfBirth)
            return ValidationResult.Success;

        var yesterday = DateTime.Today.AddDays(-1);
        
        return dateOfBirth.Date <= yesterday 
            ? ValidationResult.Success 
            : new ValidationResult("Дата рождения должна быть не позднее вчерашнего дня");
    }
}