using System.ComponentModel.DataAnnotations;

namespace CoreService.Core.Attributes;

public class RelationshipTypeAttribute : ValidationAttribute
{
    private readonly string[] _allowedTypes = { "family", "friends", "colleagues" };

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string relationshipType || string.IsNullOrWhiteSpace(relationshipType))
            return ValidationResult.Success;

        return _allowedTypes.Contains(relationshipType.ToLower()) 
            ? ValidationResult.Success 
            : new ValidationResult("Допустимые типы отношений: family, friends, colleagues");
    }
}