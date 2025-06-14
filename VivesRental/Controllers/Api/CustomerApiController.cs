using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.DTO.Customer;
using VivesRental.Services.Interfaces;

[Route("api/customers")]
[ApiController]
public class CustomerApiController : ControllerBase
{
    private readonly IService<Customer> _customerService;

    public CustomerApiController(IService<Customer> customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        if (customers == null)
            return NotFound();

        var dtos = customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber
        });

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id)
    {
        var customer = await _customerService.FindByIdAsync(id);
        if (customer == null)
            return NotFound();

        var dto = new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CustomerCreateDto dto)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };

        await _customerService.AddAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, null);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, CustomerUpdateDto dto)
    {
        var customer = await _customerService.FindByIdAsync(id);
        if (customer == null)
            return NotFound();

        customer.FirstName = dto.FirstName;
        customer.LastName = dto.LastName;
        customer.Email = dto.Email;
        customer.PhoneNumber = dto.PhoneNumber;

        await _customerService.UpdateAsync(customer);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var customer = await _customerService.FindByIdAsync(id);
        if (customer == null)
            return NotFound();

        await _customerService.DeleteAsync(customer);
        return NoContent();
    }
}
