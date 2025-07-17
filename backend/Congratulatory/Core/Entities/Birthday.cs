using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Core.Entities;

    public class Birthday
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = null!;

        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }
        public string RelationshipType { get; set; } = "Друг";

    [MaxLength(500)]
        public string? PhotoPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<BirthdayNotification> BirthdayNotifications { get; set; } = new List<BirthdayNotification>();
    }
