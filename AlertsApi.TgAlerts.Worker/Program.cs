using AlertsApi.Domain.Options;
using AlertsApi.Domain.Repositories;
using AlertsApi.Infrastructure.Db;
using AlertsApi.Infrastructure.Repositories;
using AlertsApi.TgAlerts.Worker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((host, configuration) =>
    {
        var seqOptions = host.Configuration.GetSection(SeqOptions.ConfigKey).Get<SeqOptions>();
        configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.Console()
            .WriteTo.Seq(serverUrl: seqOptions.ServerUrl!, apiKey: seqOptions.ApiKey);
    })
    .ConfigureServices((hostBuilder, services) =>
    {
        services.Configure<TgAlarmOptions>(hostBuilder.Configuration.GetSection(TgAlarmOptions.ConfigKey));

        services.AddDbContext<AlertDbContext>(contextOptionsBuilder =>
        {
            var connection = hostBuilder.Configuration.GetConnectionString("NpgsqlConnection");
            contextOptionsBuilder.UseNpgsql(connection);
        }, ServiceLifetime.Singleton);
        services.AddSingleton<IAlertRepository, AlertRepository>();
        services.AddSingleton<IMessageRepository, MessageRepository>();
        services.AddHostedService<TgFetcherService>();
    });

using var app = host.Build();
await app.RunAsync();