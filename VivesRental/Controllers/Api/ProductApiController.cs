using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
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
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
}
