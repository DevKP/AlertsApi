using AlertsApi.Api.Services;
using AlertsApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AlertsApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessagesService _messagesService;
    
    public MessagesController(IMessagesService messagesService)
    {
        _messagesService = messagesService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _messagesService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _messagesService.GetByIdAsync(id));
    }
}