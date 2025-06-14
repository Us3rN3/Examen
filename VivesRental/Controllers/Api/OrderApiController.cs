using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.DTO.Order;
using VivesRental.Services;
using VivesRental.Services.Interfaces;

[Route("api/orders")]
[ApiController]
public class OrderApiController : ControllerBase
{
    private readonly IService<Order> _orderService;
    private readonly IService<Article> _articleService;

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
            CreatedAt = DateTime.UtcNow,
            OrderLines = new List<OrderLine>()
        };

        foreach (var productId in dto.ProductIds)
        {
            // Zoek eerste beschikbare artikel
            var article = await _articleService.FindByIdAsync(productId);
            if (article == null)
                return BadRequest($"Geen beschikbaar artikel gevonden voor product {productId}");

            var product = article.Product;

            var orderLine = new OrderLine
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ArticleId = article.Id,
                ProductName = product.Name,
                ProductDescription = product.Description,
                RentedAt = DateTime.UtcNow,
                ExpiresAt = dto.ExpiresAt
            };

            order.OrderLines.Add(orderLine);
        }

        await _orderService.AddAsync(order);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, null);
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
