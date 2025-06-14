using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Services.Interfaces;

[Route("api/[controller]")]
[ApiController]
public class OrderApiController : ControllerBase
{
    private readonly IService<Order> _orderService;

    public OrderApiController(IService<Order> orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> Get(Guid id)
    {
        var order = await _orderService.FindByIdAsync(id);
        if (order == null)
            return NotFound();

        return Ok(order);
    }
}
