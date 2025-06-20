﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.DTO.Product;
using VivesRental.Services.Interfaces;

namespace VivesRental.Controllers.Api;

[Route("api/products")]
[ApiController]
[Authorize]
public class ProductApiController : ControllerBase
{
    private readonly IService<Product> _productService;
    private readonly IMapper _mapper;

    public ProductApiController(IService<Product> productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        if (products == null || !products.Any())
            return NotFound();

        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _productService.FindByIdAsync(id);
        if (product == null)
            return NotFound();

        var dto = _mapper.Map<ProductDto>(product);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] ProductCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
            return BadRequest(new { Errors = errors });
        }

        try
        {
            var product = _mapper.Map<Product>(dto);
            product.Id = Guid.NewGuid();

            await _productService.AddAsync(product);

            var createdDto = _mapper.Map<ProductDto>(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, createdDto);
        }
        catch (Exception)
        {
            // Log de exception hier eventueel
            return StatusCode(500, "Er is een onverwachte fout opgetreden bij het aanmaken van het product.");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] ProductUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
            return BadRequest(new { Errors = errors });
        }

        try
        {
            var product = await _productService.FindByIdAsync(id);
            if (product == null)
                return NotFound($"Product met id {id} niet gevonden.");

            _mapper.Map(dto, product);

            await _productService.UpdateAsync(product);
            return NoContent();
        }
        catch (Exception)
        {
            // Log de exception hier eventueel
            return StatusCode(500, "Er is een onverwachte fout opgetreden bij het updaten van het product.");
        }
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
