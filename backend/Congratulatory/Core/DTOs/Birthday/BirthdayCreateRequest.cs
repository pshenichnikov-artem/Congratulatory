using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using CoreService.Core.Attributes;

namespace CoreService.Core.DTOs.Birthday;

public class BirthdayCreateRequest
{
    [Required(ErrorMessage = "Имя обязательно")]
    [MaxLength(255, ErrorMessage = "Имя не может быть длиннее 255 символов")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Дата рождения обязательна")]
    [YesterdayOrEarlier]
    [MinimumYear(1900, ErrorMessage = "Дата рождения не может быть раньше 1900 года")]
    public DateTime DateOfBirth { get; set; }

    [PhotoValidation(5)]
    public IFormFile? Photo { get; set; }

    [RelationshipType]
    public string RelationshipType { get; set; } = string.Empty;
}