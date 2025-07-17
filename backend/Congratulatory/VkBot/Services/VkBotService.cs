using ClientInfo.Grpc;
using Microsoft.Extensions.Logging;
using VkNet;
using VkNet.Model;
using System.Text.Json;
using VkNet.Enums.Filters;
using BotShared.Services;
using BotShared.Models;

namespace VkBot.Services;

public class VkBotService : BaseBotService, IVkBotService
{
    private readonly BotSettings _botSettings;
    private readonly VkApi _vkApi;
    private readonly HttpClient _httpClient;
    private ulong _ts;
    private string _key = string.Empty;
    private string _server = string.Empty;
    protected override string GetPlatformName() => "vk";

    public VkBotService(BotSettings botSettings, ClientInfoService.ClientInfoServiceClient grpcClient, ILogger<VkBotService> logger, ApiSettings apiSettings)
        : base(grpcClient, logger, apiSettings)
    {
        _vkApi = new VkApi();
        _httpClient = new HttpClient();
        _botSettings = botSettings;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await InitializeVkApi();
        _ = Task.Run(() => ListenUpdatesAsync(cancellationToken));
        _logger.LogInformation("VK бот запущен");
    }

    private async Task InitializeVkApi()
    {
        await _vkApi.AuthorizeAsync(new ApiAuthParams
        {
            AccessToken = _botSettings.Token
        });

        var server = await _vkApi.Messages.GetLongPollServerAsync();
        _key = server.Key;
        _server = server.Server;
        _ts = server.Ts;
    }

    private async Task ListenUpdatesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var url = $"https://{_server}?act=a_check&key={_key}&ts={_ts}&wait=25";
                var response = await _httpClient.GetStringAsync(url, cancellationToken);
                var data = JsonSerializer.Deserialize<JsonElement>(response);

                if (data.TryGetProperty("ts", out var tsElement))
                    _ts = tsElement.GetUInt64();

                if (data.TryGetProperty("updates", out var updates))
                {
                    foreach (var update in updates.EnumerateArray())
                    {
                        await ProcessUpdate(update);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении обновлений VK");
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    private async Task ProcessUpdate(JsonElement update)
    {
        if (update.ValueKind != JsonValueKind.Array || update.GetArrayLength() < 6 || update[0].GetInt32() != 4)
            return;

        var messageId = update[1].GetUInt64();
        var flags = update[2].GetInt32();
        var peerId = update[3].GetInt64();

        if ((flags & 2) != 0) return;

        try
        {
            var messages = await _vkApi.Messages.GetByIdAsync(
                new[] { messageId },
                Enumerable.Empty<string>()
            );

            var message = messages.FirstOrDefault();
            if (message == null) return;

            var fromId = message.FromId ?? message.PeerId;
            var text = message.Text;

            var users = await _vkApi.Users.GetAsync(
                new[] { fromId.Value },
                ProfileFields.Domain
            );
            var user = users.FirstOrDefault();
            if (user == null) return;
            string username = user.Domain;

            await HandleMessageAsync(peerId, username, text, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке входящего сообщения");
        }
    }

    public override async Task SendMessageAsync(long chatId, string text, CancellationToken cancellationToken = default)
    {
        try
        {
            await _vkApi.Messages.SendAsync(new MessagesSendParams
            {
                PeerId = chatId,
                Message = text,
                RandomId = Environment.TickCount
            });
            _logger.LogInformation("Сообщение отправлено в чат {ChatId}: {Text}", chatId, text);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке сообщения в чат {ChatId}", chatId);
        }
    }
}
