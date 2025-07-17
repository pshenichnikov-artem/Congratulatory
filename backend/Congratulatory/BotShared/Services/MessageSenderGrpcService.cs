using Grpc.Core;
using MessageSender.Grpc;

namespace BotShared.Services;

public class MessageSenderGrpcService : MessageSenderService.MessageSenderServiceBase
{
    private readonly IBotService _botService;

    public MessageSenderGrpcService(IBotService botService)
    {
        _botService = botService;
    }

    public override async Task<StatusResponse> SendMessage(MessageRequest request, ServerCallContext context)
    {
        try
        {
            await _botService.SendMessageAsync(request.ChatId, request.Text);
            return new StatusResponse { Success = true, Message = "Сообщение отправлено" };
        }
        catch (Exception ex)
        {
            return new StatusResponse { Success = false, Message = ex.Message };
        }
    }
}