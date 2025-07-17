using ClientInfo.Grpc;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using BotShared.Services;
using BotShared.Models;

namespace TelegramBot.Services;

public class TelegramBotService : BaseBotService, ITelegramBotService
{
    private readonly ITelegramBotClient _botClient;
    protected override string GetPlatformName() => "telegram";

    public TelegramBotService(ITelegramBotClient botClient, ClientInfoService.ClientInfoServiceClient grpcClient, ILogger<TelegramBotService> logger, ApiSettings apiSettings)
        : base(grpcClient, logger, apiSettings)
    {
        _botClient = botClient;
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message || update.Message?.Text == null)
            return;

        var message = update.Message;
        var chatId = message.Chat.Id;
        var username = message.From?.Username ?? throw new Exception("Username is null");
        var text = message.Text.ToLower();
        
       await HandleMessageAsync(chatId, username, text, cancellationToken);
    }

    public override async Task SendMessageAsync(long chatId, string text, CancellationToken cancellationToken = default)
    {
        await _botClient.SendMessage(chatId, text, cancellationToken: cancellationToken);
        _logger.LogInformation("Отправлено сообщение в чат {ChatId}: {Text}", chatId, text);
    }
}