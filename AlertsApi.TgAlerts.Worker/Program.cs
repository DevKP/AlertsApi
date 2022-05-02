using AlertsApi.Domain.Options;
using AlertsApi.Domain.Repositories;
using AlertsApi.Infrastructure.Db;
using AlertsApi.Infrastructure.Repositories;
using AlertsApi.TgAlerts.Worker.AutoMapper;
using AlertsApi.TgAlerts.Worker.Services;
using AlertsApi.WTelegram.Hosting;
using AlertsApi.WTelegram.Hosting.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using WTelegram;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((host, configuration) =>
    {
        var seqOptions = host.Configuration.GetSection(SeqOptions.ConfigKey).Get<SeqOptions>();
        configuration
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.Seq(serverUrl: seqOptions.ServerUrl!, apiKey: seqOptions.ApiKey);
    })
    .ConfigureAppConfiguration((host, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((hostBuilder, services) => 
    {
        services.Configure<ClientOptions>(hostBuilder.Configuration.GetSection(ClientOptions.ConfigKey));
        services.AddDbContext<AlertDbContext>(contextOptionsBuilder =>
        {
            var connection = hostBuilder.Configuration.GetConnectionString("NpgsqlConnection");
            contextOptionsBuilder.UseNpgsql(connection);
        }, ServiceLifetime.Singleton);

        services.AddAutoMapper(typeof(MappingProfile));
        services.AddTransient<IAlertRepository, AlertRepository>();
        services.AddTransient<IMessageRepository, MessageRepository>();
        services.AddTransient<IAlertsService, AlertsService>();
        services.AddTransient<IMessagesParserService, MessagesParserService>();

        services.AddScoped<ITelegramBotService, TelegramBotService>();

        services.AddWTelegram(optionsBuilder =>
        {
            var options = hostBuilder.Configuration.GetSection(ClientOptions.ConfigKey).Get<ClientOptions>();
            optionsBuilder
                .WithSessionStorePath(options.SessionStorePath!)
                .WithCustomConfig("phone_number", "+380995031137")
                .WithCustomConfig("api_id", "19657090")
                .WithCustomConfig("api_hash", "01d7dcd1490c1b6f89985882379ad0ab")
                //.WithCustomConfig("server_address", "149.154.167.50:443")
                .WithCustomConfig("device_model", "model")
                .WithCustomConfig("system_version", "win10")
                .WithCustomConfig("app_version", "v0.1")
                .WithCustomConfig("system_lang_code", "en");
        });

        services.AddHostedService<TelegramFetcherService>();
    });

using var app = host.Build();
await app.RunAsync();