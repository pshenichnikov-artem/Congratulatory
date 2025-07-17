using CoreService.Infrastructure.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using NotificationService.Grpc;

namespace CoreService.Api.Services;

public class NotificationGrpcService : NotificationService.Grpc.NotificationService.NotificationServiceBase
{
    private readonly ApplicationDbContext _context;

    public NotificationGrpcService(ApplicationDbContext context)
    {
        _context = context;
    }

    public override async Task<NotificationList> GetTodayNotifications(Empty request, ServerCallContext context)
    {
        var today = DateTime.Now;
        
        var notifications = await _context.BirthdayNotifications
            .Include(bn => bn.Birthday)
            .Include(bn => bn.UserAccount)
            .Where(bn => bn.NotificationDate.Month == today.Month &&
                        bn.NotificationDate.Day == today.Day &&
                        bn.NotificationDate.Hour <= today.Hour &&
                        bn.NotificationDate.Minute <= today.Minute &&
                        bn.UserAccount.IsVerified &&
                        !bn.IsSent)
            .Select(bn => new NotificationData
            {
                Id = bn.Id,
                BirthdayPersonName = bn.Birthday.FullName,
                ChatId = bn.UserAccount.ChatId,
                Platform = bn.UserAccount.Platform,
                RelationshipType = bn.Birthday.RelationshipType,
                DateOfBirth = bn.Birthday.DateOfBirth.ToString("dd-MM")
                
            })
            .ToListAsync();

        var result = new NotificationList();
        result.Notifications.AddRange(notifications);
        return result;
    }

    public override async Task<Empty> MarkNotificationSent(MarkSentRequest request, ServerCallContext context)
    {
        var notification = await _context.BirthdayNotifications
            .FirstOrDefaultAsync(bn => bn.Id == request.NotificationId);

        if (notification != null)
        {
            notification.IsSent = true;
            notification.SentAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return new Empty();
    }
}