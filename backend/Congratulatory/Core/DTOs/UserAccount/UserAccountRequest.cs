using System.ComponentModel.DataAnnotations;

namespace CoreService.Core.DTOs.UserAccount;

public class UserAccountRequest
{
    [Required]
    [StringLength(50)]
    public string Platform { get; set; }
    
    [Required]
    [StringLength(100)]
    public string UserName { get; set; }
}