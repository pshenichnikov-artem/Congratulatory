using ClientInfo.Grpc;
using Grpc.Core;
using Grpc.Net.Client;
using MessageSender.Grpc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BotShared.Models;
using VkBot.Services;
using BotShared.Services;

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
        var botService = serviceProvider.GetRequiredService<IVkBotService>();
        var botSettings = serviceProvider.GetRequiredService<BotSettings>();

        using var cts = new CancellationTokenSource();

        _ = Task.Run(() => StartGrpcServer(serviceProvider, botSettings.GrpcPort));
        _ = Task.Run(() => botService.StartAsync(cts.Token));

        Console.WriteLine("VK бот запущен");
        Console.WriteLine("Нажмите Enter для остановки...");
        Console.ReadLine();
        cts.Cancel();
    }

    static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.Get<BotConfiguration>()!;
        var vkSettings = config.Bots.Vk;
        var apiSettings = config.Api;

        services.AddSingleton(vkSettings);
        services.AddSingleton(apiSettings);
        services.AddSingleton(provider =>
        {
            var channel = GrpcChannel.ForAddress(apiSettings.Address);
            return new ClientInfoService.ClientInfoServiceClient(channel);
        });
        services.AddLogging(builder => builder.AddConsole());
        services.AddSingleton<IVkBotService, VkBotService>();
        services.AddSingleton<IBotService>(provider => provider.GetRequiredService<IVkBotService>());
        services.AddSingleton<MessageSenderGrpcService>();
    }

    static void StartGrpcServer(IServiceProvider serviceProvider, int port)
    {
        try
        {
            Console.WriteLine($"Попытка запуска gRPC на порту {port}");
            var server = new Server
            {
                Services = { MessageSenderService.BindService(serviceProvider.GetRequiredService<MessageSenderGrpcService>()) },
                Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
            };

            server.Start();
            Console.WriteLine($"gRPC сервер запущен на порту {port}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при запуске gRPC сервера: {ex}");
        }
    }
}