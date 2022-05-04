using AlertsApi.Api.Services;
using AlertsApi.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AlertsApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionsService _subscriptionsService;

    public SubscriptionsController(ISubscriptionsService subscriptionsService)
    {
        _subscriptionsService = subscriptionsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var subscriptions = await _subscriptionsService.GetAllAsync();
        return Ok(subscriptions);
    }
}