using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Domains.Enums;
using VivesRental.DTO.Article;
using VivesRental.Services.Interfaces;

namespace VivesRental.Controllers.Api;

[Route("api/articles")]
[ApiController]
public class ArticleApiController : ControllerBase
{
    private readonly IService<Article> _articleService;

    public ArticleApiController(IService<Article> articleService)
    {
        _articleService = articleService;
    }

    // GET: api/articles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetAll()
    {
        var articles = await _articleService.GetAllAsync();
        if (articles == null) return NotFound();

        var dtos = articles.Select(a => new ArticleDto
        {
            Id = a.Id,
            ProductName = a.Product?.Name ?? "Onbekend",
            Status = (int)a.Status
        });

        return Ok(dtos);
    }

    // GET: api/articles/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ArticleDto>> GetById(Guid id)
    {
        var article = await _articleService.FindByIdAsync(id);
        if (article == null) return NotFound();

        var dto = new ArticleDto
        {
            Id = article.Id,
            ProductName = article.Product?.Name ?? "Onbekend",
            Status = (int)article.Status
        };

        return Ok(dto);
    }

    // POST: api/articles
    [HttpPost]
    public async Task<ActionResult> Create(ArticleCreateDto dto)
    {
        var article = new Article
        {
            Id = Guid.NewGuid(),
            ProductId = dto.ProductId,
            Status = (ArticleStatus)dto.Status
        };

        await _articleService.AddAsync(article);

        return CreatedAtAction(nameof(GetById), new { id = article.Id }, dto);
    }

    // PUT: api/articles/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, ArticleUpdateDto dto)
    {
        var article = await _articleService.FindByIdAsync(id);
        if (article == null) return NotFound();

        article.Status = (ArticleStatus)dto.Status;

        await _articleService.UpdateAsync(article);

        return NoContent();
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
