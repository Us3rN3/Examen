using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.DTO.Customer;
using VivesRental.Services.Interfaces;

[Route("api/customers")]
[ApiController]
public class CustomerApiController : ControllerBase
{
    private readonly IService<Customer> _customerService;
    private readonly IMapper _mapper;

    public CustomerApiController(IService<Customer> customerService, IMapper mapper)
    {
        _customerService = customerService;
        _mapper = mapper;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        if (customers == null)
            return NotFound();

        var dtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
        return Ok(dtos);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id)
    {
        var customer = await _customerService.FindByIdAsync(id);
        if (customer == null)
            return NotFound();

        var dto = _mapper.Map<CustomerDto>(customer);
        return Ok(dto);
    }


    [HttpPost]
    public async Task<ActionResult> Create(CustomerCreateDto dto)
    {
        var customer = _mapper.Map<Customer>(dto);
        customer.Id = Guid.NewGuid();

        await _customerService.AddAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, null);
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, CustomerUpdateDto dto)
    {
        var customer = await _customerService.FindByIdAsync(id);
        if (customer == null)
            return NotFound();

        _mapper.Map(dto, customer); // updates op bestaand object

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
