using System.ComponentModel.DataAnnotations;
using CoreService.Core.Attributes;

namespace CoreService.Core.DTOs.BirthdayNotification;

public class BirthdayNotificationRequest
{
    [Required(ErrorMessage = "ID аккаунта пользователя обязателен")]
    [Range(1, long.MaxValue, ErrorMessage = "ID аккаунта должен быть положительным числом")]
    public long UserAccountId { get; set; }
    
    [Required(ErrorMessage = "Дата уведомления обязательна")]
    [NotificationDate]
    public string NotificationDate { get; set; } = string.Empty;
}