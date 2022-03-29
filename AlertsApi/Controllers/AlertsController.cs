using AlertsApi.Api.Models.Responses;
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
        private readonly IAlertRepository _alertRepository;
        private readonly ILogger<AlertsController> _logger;
        private readonly IMapper _mapper;

        public AlertsController(ILogger<AlertsController> logger, IAlertRepository alertRepository, IMapper mapper)
        {
            _logger = logger;
            _alertRepository = alertRepository;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlert(int id)
        {
            var alert = await _alertRepository.GetAlertAsync(id);
            return Ok(_mapper.Map<AlertResponse>(alert));
        }

        [HttpGet("GetAllAlerts")]
        public async Task<IActionResult> GetAllAlert()
        {
            var alerts = await _alertRepository.GetAllAlertsAsync();
            var alertsResponse = _mapper.Map<AlertsResponse>(alerts);
            return Ok(alertsResponse);
        }

        [HttpGet("GetActiveAlerts")]
        public async Task<IActionResult> GetActiveAlerts()
        {
            var alerts = await _alertRepository.GetAllAlertsAsync();
            var activeAlerts = alerts.Where(a => a.Active);
            var alertsResponse = _mapper.Map<AlertsResponse>(activeAlerts);
            return Ok(alertsResponse);
        }
    }
}