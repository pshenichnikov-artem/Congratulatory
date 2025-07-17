using Microsoft.AspNetCore.Identity;

namespace CoreService.Core.Entities;

    public class ApplicationUser : IdentityUser<Guid>
    {

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
        public virtual ICollection<Birthday> Birthdays { get; set; } = new List<Birthday>();
    }
