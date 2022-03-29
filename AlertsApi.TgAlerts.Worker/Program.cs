using AlertsApi.Domain.Repositories;
using AlertsApi.Infrastructure.Db;
using AlertsApi.Infrastructure.Repositories;
using AlertsApi.TgAlerts.Worker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostBuilder, services) =>
    {
        services.AddDbContext<AlertDbContext>(contextOptionsBuilder =>
        {
            var connection = hostBuilder.Configuration.GetConnectionString("NpgsqlConnection");
            contextOptionsBuilder.UseNpgsql(connection);
        });
        services.AddTransient<IAlertRepository, AlertRepository>();
        services.AddHostedService<TgFetcherService>();
    });

using var app = host.Build();
await app.RunAsync();