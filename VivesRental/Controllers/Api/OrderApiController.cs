using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.DTO.Order;
using VivesRental.Services.Interfaces;

[Route("api/orders")]
[ApiController]
public class OrderApiController : ControllerBase
{
    private readonly IService<Order> _orderService;

    public OrderApiController(IService<Order> orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> Get(Guid id)
    {
        var order = await _orderService.FindByIdAsync(id);
        if (order == null)
            return NotFound();

        var dto = new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerFirstName = order.CustomerFirstName,
            CustomerLastName = order.CustomerLastName,
            CustomerEmail = order.CustomerEmail,
            CustomerPhoneNumber = order.CustomerPhoneNumber,
            CreatedAt = order.CreatedAt
        };

        return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
    {
        var orders = await _orderService.GetAllAsync();
        if (orders == null)
            return NotFound();

        var dtos = orders.Select(order => new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerFirstName = order.CustomerFirstName,
            CustomerLastName = order.CustomerLastName,
            CustomerEmail = order.CustomerEmail,
            CustomerPhoneNumber = order.CustomerPhoneNumber,
            CreatedAt = order.CreatedAt
        });

        return Ok(dtos);
    }

    [HttpPost]
    public async Task<ActionResult> Create(OrderCreateDto dto)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = dto.CustomerId,
            CustomerFirstName = dto.CustomerFirstName,
            CustomerLastName = dto.CustomerLastName,
            CustomerEmail = dto.CustomerEmail,
            CustomerPhoneNumber = dto.CustomerPhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        await _orderService.AddAsync(order);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, null);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult> CreateBulk(OrderBulkCreateDto bulkDto)
    {
        foreach (var dto in bulkDto.Orders)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = dto.CustomerId,
                CustomerFirstName = dto.CustomerFirstName,
                CustomerLastName = dto.CustomerLastName,
                CustomerEmail = dto.CustomerEmail,
                CustomerPhoneNumber = dto.CustomerPhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            await _orderService.AddAsync(order); // eventueel batch optimalisatie mogelijk
        }

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var order = await _orderService.FindByIdAsync(id);
        if (order == null)
            return NotFound();

        await _orderService.DeleteAsync(order);
        return NoContent();
    }
}
