using BirthdayNotificationWorker.Services;

namespace BirthdayNotificationWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ApiService _apiService;
    private readonly GrpcNotificationService _grpcService;

    public Worker(ILogger<Worker> logger, ApiService apiService, GrpcNotificationService grpcService)
    {
        _logger = logger;
        _apiService = apiService;
        _grpcService = grpcService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Birthday Notification Worker started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Проверка уведомлений на {Time}", DateTime.Now);
                
                var notifications = await _apiService.GetTodayNotificationsAsync();
                _logger.LogInformation("Найдено {Count} уведомлений", notifications.Count);
                
                foreach (var notification in notifications)
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;
                        
                    var result = await _grpcService.SendNotificationAsync(notification);
                    if(result)
                        await _apiService.MarkNotificationSentAsync(notification.Id);
                }
                
                _logger.LogInformation("Обработка уведомлений завершена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке уведомлений");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
