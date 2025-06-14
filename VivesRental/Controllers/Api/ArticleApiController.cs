using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Models;
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetAll()
    {
        var articles = await _articleService.GetAllAsync();
        if (articles == null)
            return NotFound();

        var dtos = articles.Select(a => new ArticleDto
        {
            Id = a.Id,
            ProductName = a.Product?.Name ?? "Onbekend",
            Status = a.Status
        });

        return Ok(dtos);
    }
}
