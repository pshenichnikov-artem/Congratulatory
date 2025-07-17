namespace BotShared.Models;

public class BotConfiguration
{
    public BotsSettings Bots { get; set; } = new();
    public ApiSettings Api { get; set; } = new();
}

public class BotsSettings
{
    public BotSettings Telegram { get; set; } = new();
    public BotSettings Vk { get; set; } = new();
}

public class BotSettings
{
    public string Token { get; set; } = string.Empty;
    public int GrpcPort { get; set; }
}

public class ApiSettings
{
    public string Address { get; set; } = string.Empty;
    public string FrontendUrl { get; set; } = string.Empty;
}