using System.ComponentModel.DataAnnotations;
using CoreService.Core.Attributes;

namespace CoreService.Core.DTOs.User;

public class LoginRequest
{
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [PasswordValidation]
    public string Password { get; set; } = string.Empty;
}