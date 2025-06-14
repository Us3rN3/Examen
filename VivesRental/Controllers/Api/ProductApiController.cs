using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.DTO.Product;
using VivesRental.Services.Interfaces;

namespace VivesRental.Controllers.Api;

[Route("api/products")]
[ApiController]
public class ProductApiController : ControllerBase
{
    private readonly IService<Product> _productService;

    public ProductApiController(IService<Product> productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        if (products == null)
            return NotFound();

        var dtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Manufacturer = p.Manufacturer,
            Publisher = p.Publisher,
            RentalExpiresAfterDays = p.RentalExpiresAfterDays
        });

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _productService.FindByIdAsync(id);
        if (product == null)
            return NotFound();

        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Manufacturer = product.Manufacturer,
            Publisher = product.Publisher,
            RentalExpiresAfterDays = product.RentalExpiresAfterDays
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult> Create(ProductCreateDto dto)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Manufacturer = dto.Manufacturer,
            Publisher = dto.Publisher,
            RentalExpiresAfterDays = dto.RentalExpiresAfterDays
        };

        await _productService.AddAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, null);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, ProductUpdateDto dto)
    {
        var product = await _productService.FindByIdAsync(id);
        if (product == null)
            return NotFound();

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Manufacturer = dto.Manufacturer;
        product.Publisher = dto.Publisher;
        product.RentalExpiresAfterDays = dto.RentalExpiresAfterDays;

        await _productService.UpdateAsync(product);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var product = await _productService.FindByIdAsync(id);
        if (product == null)
            return NotFound();

        await _productService.DeleteAsync(product);
        return NoContent();
    }
}
