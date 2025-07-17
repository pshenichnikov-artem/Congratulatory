using BirthdayNotificationWorker.Models;
using MessageSender.Grpc;
using Grpc.Net.Client;

namespace BirthdayNotificationWorker.Services;

public class GrpcNotificationService
{
    private readonly ILogger<GrpcNotificationService> _logger;
    private readonly Dictionary<string, string> _botAddresses;

    public GrpcNotificationService(ILogger<GrpcNotificationService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _botAddresses = new Dictionary<string, string>
        {
            { "telegram", configuration.GetConnectionString("TelegramBotAddress") ?? throw new ArgumentNullException("TelegramBotAddress") },
            { "vk", configuration.GetConnectionString("VkBotAddress") ?? throw new ArgumentNullException("VkBotAddress") }
        };
    }

    public async Task<bool> SendNotificationAsync(BirthdayNotificationDto notification)
    {
        try
        {
            if (!_botAddresses.TryGetValue(notification.Platform.ToLower(), out var address))
            {
                _logger.LogWarning("Неизвестная платформа: {Platform}", notification.Platform);
                return false;
            }

            using var channel = GrpcChannel.ForAddress(address);
            var client = new MessageSenderService.MessageSenderServiceClient(channel);

            var response = await client.SendMessageAsync(new MessageRequest
            {
                ChatId = notification.ChatId,
                Text = notification.MessageText
            });
            
            if (!response.Success)
            {
                _logger.LogError("Ошибка отправки сообщения: {Message}", response.Message);
                return false;
            }

            _logger.LogInformation("Отправка уведомления в чат {ChatId} на платформе {Platform} по адресу {Address}", 
                notification.ChatId, notification.Platform, address);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке уведомления через gRPC на платформу {Platform}", notification.Platform);
            return false;
        }
    }
}