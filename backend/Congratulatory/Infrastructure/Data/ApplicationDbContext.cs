using CoreService.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CoreService.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Birthday> Birthdays { get; set; }
    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<BirthdayNotification> BirthdayNotifications { get; set; }

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Congratulatory;Username=postgres;Password=20051022");

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasMany(u => u.Birthdays)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.UserAccounts)
                .WithOne(ua => ua.User)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<UserAccount>(entity =>
        {
            entity.HasMany(ua => ua.BirthdayNotifications)
                .WithOne(bn => bn.UserAccount)
                .HasForeignKey(bn => bn.UserAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<BirthdayNotification>(entity =>
        {
            entity.HasOne(bn => bn.Birthday)
                .WithMany(b => b.BirthdayNotifications)
                .HasForeignKey(bn => bn.BirthdayId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}