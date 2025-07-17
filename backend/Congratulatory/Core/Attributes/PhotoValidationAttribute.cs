using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CoreService.Core.Attributes;

public class PhotoValidationAttribute : ValidationAttribute
{
    private readonly int _maxSizeInMb;
    private readonly string[] _allowedTypes;

    public PhotoValidationAttribute(int maxSizeInMb = 5)
    {
        _maxSizeInMb = maxSizeInMb;
        _allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IFormFile photo)
            return ValidationResult.Success;

        var errors = new List<string>();

        if (photo.Length > _maxSizeInMb * 1024 * 1024)
        {
            errors.Add($"Размер файла не должен превышать {_maxSizeInMb} МБ");
        }

        if (!_allowedTypes.Contains(photo.ContentType.ToLower()))
        {
            errors.Add("Допустимые форматы: JPEG, PNG, GIF");
        }

        return errors.Any() ? new ValidationResult(string.Join("; ", errors)) : ValidationResult.Success;
    }
}