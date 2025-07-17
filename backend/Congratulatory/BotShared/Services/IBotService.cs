namespace BotShared.Services;

public interface IBotService
{
    Task HandleMessageAsync(long chatId, string username, string text, CancellationToken cancellationToken);
    Task SendMessageAsync(long chatId, string text, CancellationToken cancellationToken = default);
}