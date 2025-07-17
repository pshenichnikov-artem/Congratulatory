using BirthdayNotificationWorker.Models;
using Grpc.Net.Client;
using NotificationService.Grpc;

namespace BirthdayNotificationWorker.Services;

public class ApiService
{
    private readonly ILogger<ApiService> _logger;
    private readonly string _apiAddress;

    public ApiService(ILogger<ApiService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _apiAddress = configuration.GetConnectionString("ApiAddress") ?? throw new ArgumentNullException("ApiAddress");
    }

    public async Task<List<BirthdayNotificationDto>> GetTodayNotificationsAsync()
    {
        try
        {
            using var channel = GrpcChannel.ForAddress(_apiAddress);
            var client = new NotificationService.Grpc.NotificationService.NotificationServiceClient(channel);
            
            var response = await client.GetTodayNotificationsAsync(new Empty());
            
            return response.Notifications.Select(n => new BirthdayNotificationDto
            {
                Id = n.Id,
                BirthdayPersonName = n.BirthdayPersonName,
                ChatId = n.ChatId,
                Platform = n.Platform,
                RelationshipType = n.RelationshipType,
                MessageText = n.RelationshipType switch
                {
                    "family" => $"👨‍👩‍👧 {n.DateOfBirth} у твоего родственника {n.BirthdayPersonName} день рождения 🎉\nТеплые слова и немного заботы точно пригодятся ❤️",
                    "friend" => $"🎈 Не забудь, у твоего друга {n.BirthdayPersonName} {n.DateOfBirth} день рождения!\nПорадуй хорошим словом или приятным сюрпризом 🎁",
                    "colleague" => $"💼 {n.DateOfBirth} у твоего коллеги {n.BirthdayPersonName} день рождения 🎉\nПожелай удачи и успехов!",
                    _ => $"🎉 {n.DateOfBirth} {n.BirthdayPersonName} отмечает день рождения!\nСамое время поздравить 🥳"
                },
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при получении уведомлений через gRPC({_apiAddress})");
            return new List<BirthdayNotificationDto>();
        }
    }

    public async Task MarkNotificationSentAsync(long notificationId)
    {
        try
        {
            using var channel = GrpcChannel.ForAddress(_apiAddress);
            var client = new NotificationService.Grpc.NotificationService.NotificationServiceClient(channel);
            
            await client.MarkNotificationSentAsync(new MarkSentRequest { NotificationId = notificationId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отметке уведомления как отправленного");
        }
    }
}