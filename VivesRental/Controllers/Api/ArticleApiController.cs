using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Domains.Enums;
using VivesRental.DTO.Article;
using VivesRental.Services.Interfaces;

namespace VivesRental.Controllers.Api;

[Route("api/articles")]
[ApiController]
[Authorize]
public class ArticleApiController : ControllerBase
{
    private readonly IService<Article> _articleService;
    private readonly IMapper _mapper;

    public ArticleApiController(IService<Article> articleService, IMapper mapper)
    {
        _articleService = articleService;
        _mapper = mapper;
    }


    // GET: api/articles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetAll()
    {
        var articles = await _articleService.GetAllAsync();
        if (articles == null) return NotFound();

        var dtos = _mapper.Map<IEnumerable<ArticleDto>>(articles);

        return Ok(dtos);
    }


    // GET: api/articles/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ArticleDto>> GetById(Guid id)
    {
        var article = await _articleService.FindByIdAsync(id);
        if (article == null) return NotFound();

        var dto = _mapper.Map<ArticleDto>(article);
        return Ok(dto);
    }

    // POST: api/articles
    [HttpPost]
    public async Task<ActionResult> Create(ArticleCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
            return BadRequest(new { Errors = errors });
        }

        var article = _mapper.Map<Article>(dto);
        article.Id = Guid.NewGuid();

        try
        {
            await _articleService.AddAsync(article);
            return CreatedAtAction(nameof(GetById), new { id = article.Id }, dto);
        }
        catch (Exception)
        {
            // Optioneel: log de fout hier
            return StatusCode(500, "Er is een fout opgetreden bij het aanmaken van het artikel.");
        }
    }

    // POST: api/articles/bulk
    [HttpPost("bulk")]
    public async Task<ActionResult> BulkCreate(ArticleBulkCreateDto dto)
    {
        if (dto.Amount <= 0 || dto.Amount > 1000)
            return BadRequest("Aantal moet tussen 1 en 1000 liggen.");

        var articles = Enumerable.Range(0, dto.Amount)
            .Select(_ =>
            {
                var article = _mapper.Map<Article>(dto);
                article.Id = Guid.NewGuid();
                return article;
            });

        foreach (var article in articles)
        {
            await _articleService.AddAsync(article);
        }


        return Ok(new { Message = $"{dto.Amount} artikelen aangemaakt." });
    }


    // PUT: api/articles/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, ArticleUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
            return BadRequest(new { Errors = errors });
        }

        var article = await _articleService.FindByIdAsync(id);
        if (article == null)
            return NotFound($"Artikel met id {id} niet gevonden.");

        _mapper.Map(dto, article); // update waarden op bestaand object

        try
        {
            await _articleService.UpdateAsync(article);
            return NoContent();
        }
        catch (Exception)
        {
            // Optioneel: log de fout hier
            return StatusCode(500, "Er is een fout opgetreden bij het bijwerken van het artikel.");
        }
    }

    // DELETE: api/articles/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var article = await _articleService.FindByIdAsync(id);
        if (article == null) return NotFound();

        await _articleService.DeleteAsync(article);

        return NoContent();
    }
}
