using ClientInfo.Grpc;
using Microsoft.Extensions.Logging;
using BotShared.Models;

namespace BotShared.Services;

public abstract class BaseBotService : IBotService
{
    protected readonly ClientInfoService.ClientInfoServiceClient _grpcClient;
    protected readonly ILogger _logger;
    protected readonly ApiSettings _apiSettings;

    protected BaseBotService(ClientInfoService.ClientInfoServiceClient grpcClient, ILogger logger, ApiSettings apiSettings)
    {
        _grpcClient = grpcClient;
        _logger = logger;
        _apiSettings = apiSettings;
    }

    public virtual async Task HandleMessageAsync(long chatId, string username, string text, CancellationToken cancellationToken)
    {
        if (!IsValidCommand(text))
            return;

        _logger.LogInformation("Команда {Command} от {Username} ({ChatId})", text, username, chatId);

        try
        {
            var response = await _grpcClient.SendClientInfoAsync(new BotClientInfo
            {
                ChatId = chatId,
                Username = username,
                Platform = GetPlatformName()
            }, cancellationToken: cancellationToken);

            string replyText = response.Success
                ? "🎉 Готово! Данные обновлены. Теперь ты точно не забудешь про важные дни 🎂"
                : $"❌ Похоже, ты ещё не зарегистрировал свой аккаунт.\nУкажи свой аккаунт здесь: {_apiSettings.FrontendUrl}";

            await SendMessageAsync(chatId, replyText, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке команды");
            await SendMessageAsync(chatId, "⚠️ Не удалось связаться с сервером. Попробуй позже.", cancellationToken);
        }
    }

    public abstract Task SendMessageAsync(long chatId, string text, CancellationToken cancellationToken = default);

    protected abstract string GetPlatformName();

    protected virtual bool IsValidCommand(string text)
    {
        var lowerText = text.ToLower();
        return lowerText == "/start" || lowerText == "/update" || lowerText == "начало" || lowerText == "обновить";
    }
}