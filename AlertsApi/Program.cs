using AlertsApi.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using AlertsApi.Api.AutoMapper;
using AlertsApi.Api.Services;
using AlertsApi.Domain.Options;
using AlertsApi.Domain.Repositories;
using AlertsApi.Infrastructure.Repositories;
using Serilog;
using Serilog.Events;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((host, configuration) =>
{
    var seqOptions = host.Configuration.GetSection(SeqOptions.ConfigKey).Get<SeqOptions>();
    configuration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .WriteTo.Console()
        .WriteTo.Seq(seqOptions.ServerUrl!, apiKey: seqOptions.ApiKey);
});

builder.Services.AddDbContext<AlertDbContext>(contextOptionsBuilder =>
{
    var connection = builder.Configuration.GetConnectionString("NpgsqlConnection");
    contextOptionsBuilder.UseNpgsql(connection);
});

builder.Services.AddTransient<IAlertRepository, AlertRepository>();
builder.Services.AddTransient<IMessageRepository, MessageRepository>();
builder.Services.AddTransient<IAlertsService, AlertsService>();
builder.Services.AddTransient<IMessagesService, MessagesService>();

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("corsapp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();