using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Domains.Enums;
using VivesRental.DTO.Order;
using VivesRental.Services;
using VivesRental.Services.Interfaces;

[Route("api/orders")]
[ApiController]
public class OrderApiController : ControllerBase
{
    private readonly IService<Order> _orderService;
    private readonly ArticleService _articleService;
    private readonly IMapper _mapper;

    public OrderApiController(IService<Order> orderService, ArticleService articleService, IMapper mapper)
    {
        _orderService = orderService;
        _articleService = articleService;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> Get(Guid id)
    {
        var order = await _orderService.FindByIdAsync(id);
        if (order == null) return NotFound();

        var dto = _mapper.Map<OrderDto>(order);
        return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
    {
        var orders = await _orderService.GetAllAsync();
        if (orders == null) return NotFound();

        var dtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
        return Ok(dtos);
    }

    [HttpPost]
    public async Task<ActionResult> Create(OrderCreateDto dto)
    {
        var order = _mapper.Map<Order>(dto);
        order.Id = Guid.NewGuid();
        order.CreatedAt = DateTime.UtcNow;
        order.OrderLines = new List<OrderLine>();

        foreach (var productId in dto.ProductIds)
        {
            // TODO: hier moet je een methode hebben die het eerste beschikbare artikel van een product vindt
            var article = await _articleService.FindFirstAvailableByProductIdAsync(productId);
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

            article.Status = ArticleStatus.Verhuurd;
            await _articleService.UpdateAsync(article);
        }

        await _orderService.AddAsync(order);

        return CreatedAtAction(nameof(Get), new { id = order.Id }, null);
    }

    [HttpPut("{id}/return")]
    public async Task<ActionResult> ReturnOrder(Guid id)
    {
        var order = await _orderService.FindByIdAsync(id);
        if (order == null) return NotFound();

        foreach (var line in order.OrderLines)
        {
            line.ReturnedAt = DateTime.UtcNow;
        }

        await _orderService.UpdateAsync(order);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var order = await _orderService.FindByIdAsync(id);
        if (order == null) return NotFound();

        await _orderService.DeleteAsync(order);
        return NoContent();
    }
}
