using AlertsApi.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using AlertsApi.Api.AutoMapper;
using AlertsApi.Domain.Repositories;
using AlertsApi.Infrastructure.Repositories;
using Serilog;
using Serilog.Events;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, configuration) => configuration
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:8081/")
);

//Seq token ksbsVPaSniW4uTygTJFU

// Add services to the container.

builder.Services.AddDbContext<AlertDbContext>(contextOptionsBuilder =>
{
    var connection = builder.Configuration.GetConnectionString("NpgsqlConnection");
    contextOptionsBuilder.UseNpgsql(connection);
});

builder.Services.AddTransient<IAlertRepository, AlertRepository>();

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
