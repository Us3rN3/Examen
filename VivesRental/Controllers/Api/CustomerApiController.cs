using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Services.Interfaces;

[Route("api/[controller]")]
[ApiController]
public class CustomerApiController : ControllerBase
{
    private readonly IService<Customer> _customerService;

    public CustomerApiController(IService<Customer> customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Post(Customer customer)
    {
        await _customerService.AddAsync(customer);
        return CreatedAtAction(nameof(Post), new { id = customer.Id }, customer);
    }
}
