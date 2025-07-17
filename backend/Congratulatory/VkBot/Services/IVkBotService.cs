using BotShared.Services;

namespace VkBot.Services;

public interface IVkBotService : IBotService
{
    Task StartAsync(CancellationToken cancellationToken = default);
}