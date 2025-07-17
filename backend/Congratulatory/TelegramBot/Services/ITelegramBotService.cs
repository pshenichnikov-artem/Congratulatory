using Telegram.Bot.Types;
using BotShared.Services;

namespace TelegramBot.Services;

public interface ITelegramBotService : IBotService
{
    Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
}