using AlertsApi.Api.Models.Responses;
using AlertsApi.Api.Services;
using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Queries;
using AlertsApi.Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AlertsApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly IAlertsService _alertsService;

        public AlertsController(IAlertsService alertsService)
        {
            _alertsService = alertsService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var alert = await _alertsService.GetAlertById(id);
            if (alert is null)
                return NotFound();

            return Ok(alert);
        }

        [HttpGet(Name = "Get")]
        public async Task<IActionResult> Get([FromQuery]AlertsQuery query)
        {
            var alert = await _alertsService.Get(query);
            if (alert is null)
                return NotFound();

            return Ok(alert);
        }

        [HttpGet("LastHours/{hours}")]
        public async Task<IActionResult> LastHours(int hours)
        {
            var query = new AlertsQuery()
            {
                From = DateTime.UtcNow.AddHours(-hours),
                To = DateTime.UtcNow,
                Active = false
            };
            var alerts = await _alertsService.Get(query);
            if (alerts is null)
                return NotFound();

            return Ok(alerts);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            var alerts = await _alertsService.GetAllAlerts();
            return Ok(alerts);
        }

        [HttpGet("Active")]
        public async Task<IActionResult> GetActive()
        {
            var alerts = await _alertsService.GetActiveAlerts();
            return Ok(alerts);
        }
    }
}