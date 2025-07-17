using ClientInfo.Grpc;
using CoreService.Core.Interfaces;
using Grpc.Core;

namespace CoreService.Api.Services;

public class ClientInfoGrpcService : ClientInfoService.ClientInfoServiceBase
{
    private readonly IUserService _userService;

    public ClientInfoGrpcService(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<StatusResponse> SendClientInfo(BotClientInfo request, ServerCallContext context)
    {
        var result = await _userService.VerifyUserAccountAsync(request.Platform, request.Username, request.ChatId);
        
        return new StatusResponse
        {
            Success = result.Success,
            Message = result.Success ? "Данные обновлены" : "Пользователь не зарегистрирован"
        };
    }
}