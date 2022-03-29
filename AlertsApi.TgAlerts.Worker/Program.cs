using AlertsApi.Domain.Repositories;
using AlertsApi.Infrastructure.Db;
using AlertsApi.Infrastructure.Repositories;
using AlertsApi.TgAlerts.Worker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args);
host.ConfigureServices(ServicesConfiguration);

using var app = host.Build();
await app.RunAsync();

void ServicesConfiguration(HostBuilderContext hostBuilder, IServiceCollection services)
{
    services.AddDbContext<AlertDbContext>(contextOptionsBuilder =>
    {
        var connection = hostBuilder.Configuration.GetConnectionString("DefaultConnection");
        contextOptionsBuilder.UseSqlServer(connection);
    });
    services.AddTransient<IAlertRepository, AlertRepository>();
    services.AddHostedService<TgFetcherService>();
}