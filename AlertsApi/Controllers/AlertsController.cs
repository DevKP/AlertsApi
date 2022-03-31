using AlertsApi.Api.Models.Responses;
using AlertsApi.Api.Services;
using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AlertsApi.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
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