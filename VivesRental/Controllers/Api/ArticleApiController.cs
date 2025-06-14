using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
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
    public async Task<ActionResult<IEnumerable<Article>>> GetAll()
    {
        var articles = await _articleService.GetAllAsync();
        return Ok(articles);
    }
}
