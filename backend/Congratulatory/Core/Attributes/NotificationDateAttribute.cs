using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CoreService.Core.Attributes;

public class NotificationDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string dateString || string.IsNullOrWhiteSpace(dateString))
            return new ValidationResult("Дата уведомления обязательна");

        if (!DateTime.TryParseExact(dateString, "MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            return new ValidationResult("Неверный формат даты. Используйте MM-dd HH:mm");
        }

        return ValidationResult.Success;
    }
}