using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.DTO.Customer;
using VivesRental.Services.Interfaces;

[Route("api/customers")]
[ApiController]
[Authorize]
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
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
            return BadRequest(new { Errors = errors });
        }

        var customer = _mapper.Map<Customer>(dto);
        customer.Id = Guid.NewGuid();

        try
        {
            await _customerService.AddAsync(customer);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, null);
        }
        catch (Exception)
        {
            // Hier kun je eventueel logging toevoegen
            return StatusCode(500, "Er is een fout opgetreden bij het aanmaken van de klant.");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, CustomerUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
            return BadRequest(new { Errors = errors });
        }

        var customer = await _customerService.FindByIdAsync(id);
        if (customer == null)
            return NotFound($"Klant met id {id} niet gevonden.");

        _mapper.Map(dto, customer); // updates op bestaand object

        try
        {
            await _customerService.UpdateAsync(customer);
            return NoContent();
        }
        catch (Exception)
        {
            // Hier kun je eventueel logging toevoegen
            return StatusCode(500, "Er is een fout opgetreden bij het bijwerken van de klant.");
        }
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
