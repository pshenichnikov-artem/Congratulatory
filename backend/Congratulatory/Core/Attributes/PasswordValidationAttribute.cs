using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CoreService.Core.Attributes;

public class PasswordValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string password || string.IsNullOrWhiteSpace(password))
            return ValidationResult.Success;

        if (password.Length < 8)
            return new ValidationResult("Минимальная длина пароля 8 символов");

        if (password.Length > 100)
            return new ValidationResult("Максимальная длина пароля 100 символов");

        if (!Regex.IsMatch(password, @"[A-ZА-ЯЁ]"))
            return new ValidationResult("Пароль должен содержать заглавную букву");

        if (!Regex.IsMatch(password, @"[a-zа-яё]"))
            return new ValidationResult("Пароль должен содержать строчную букву");

        if (!Regex.IsMatch(password, @"\d"))
            return new ValidationResult("Пароль должен содержать цифру");

        if (!Regex.IsMatch(password, @"[!@#$%^&*]"))
            return new ValidationResult("Пароль должен содержать специальный символ");

        return ValidationResult.Success;
    }
}