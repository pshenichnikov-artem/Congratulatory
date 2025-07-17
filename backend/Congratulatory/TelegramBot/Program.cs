using ClientInfo.Grpc;
using Grpc.Core;
using Grpc.Net.Client;
using MessageSender.Grpc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using BotShared.Models;
using BotShared.Services;
using TelegramBot.Services;

class Program
{
    static async Task Main()
    {

        var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

        var services = new ServiceCollection();
        ConfigureServices(services, configuration);
        
        var serviceProvider = services.BuildServiceProvider();
        var botService = serviceProvider.GetRequiredService<ITelegramBotService>();
        var botSettings = serviceProvider.GetRequiredService<BotSettings>();
        var botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();

        using var cts = new CancellationTokenSource();

        _ = Task.Run(() => StartGrpcServer(serviceProvider, botSettings.GrpcPort));

        botClient.StartReceiving(
            async (bot, update, ct) => await botService.HandleUpdateAsync(update, ct),
            (bot, ex, ct) => 
            {
                Console.WriteLine($"❌ Ошибка Telegram({configuration.Get<BotConfiguration>().Bots.Telegram.Token}): {ex.Message}");
                return Task.CompletedTask;
            },
            new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMe();
        Console.WriteLine($"Telegram бот запущен: @{me.Username}");
        Console.WriteLine("Нажмите Enter для остановки...");
        Console.ReadLine();
        cts.Cancel();
    }

    static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.Get<BotConfiguration>()!;
        var telegramSettings = config.Bots.Telegram;
        var apiSettings = config.Api;

        services.AddSingleton(telegramSettings);
        services.AddSingleton(apiSettings);

        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(telegramSettings.Token));
        services.AddSingleton(provider =>
        {
            var channel = GrpcChannel.ForAddress(apiSettings.Address);
            return new ClientInfoService.ClientInfoServiceClient(channel);
        });
        services.AddLogging(builder => builder.AddConsole());
        services.AddSingleton<ITelegramBotService, TelegramBotService>();
        services.AddSingleton<IBotService>(provider => provider.GetRequiredService<ITelegramBotService>());
        services.AddSingleton<MessageSenderGrpcService>();
    }

    static void StartGrpcServer(IServiceProvider serviceProvider, int port)
    {
        var server = new Server
        {
            Services = { MessageSenderService.BindService(serviceProvider.GetRequiredService<MessageSenderGrpcService>()) },
            Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
        };

        server.Start();
        Console.WriteLine($"gRPC сервер запущен на порту {port}");
    }
}