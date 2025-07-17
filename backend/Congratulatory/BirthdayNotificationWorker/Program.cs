using BirthdayNotificationWorker;
using BirthdayNotificationWorker.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ApiService>();
builder.Services.AddSingleton<GrpcNotificationService>();
builder.Services.AddHostedService<Worker>();
builder.Configuration.AddEnvironmentVariables();

var host = builder.Build();
host.Run();
