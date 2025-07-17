using System.ComponentModel.DataAnnotations;
using CoreService.Core.Attributes;

namespace CoreService.Core.DTOs.User;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Текущий пароль обязателен")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Новый пароль обязателен")]
    [PasswordValidation]
    public string NewPassword { get; set; } = string.Empty;
}