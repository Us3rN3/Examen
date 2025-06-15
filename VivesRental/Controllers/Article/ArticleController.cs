using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Domains.Enums;
using VivesRental.Models;
using VivesRental.Services.Interfaces;

public class ArticleController : Controller
{
    private readonly IService<Article> _service;
    private readonly IService<Product> _productService;

    public ArticleController(IService<Article> service, IService<Product> productService)
    {
        _service = service;
        _productService = productService;
    }

    public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 15)
    {
        var allArticles = await _service.GetAllAsync() ?? new List<Article>();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            allArticles = allArticles
                .Where(a => a.Product != null &&
                            a.Product.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var totalItems = allArticles.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var pagedArticles = allArticles
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var viewModel = new ArticleIndexViewModel
        {
            Articles = pagedArticles,
            SearchTerm = searchTerm,
            CurrentPage = page,
            TotalPages = totalPages
        };

        return View(viewModel);
    }


    public async Task<IActionResult> Details(Guid id)
    {
        var article = await _service.FindByIdAsync(id);
        return article == null ? NotFound() : View(article);
    }

    public async Task<IActionResult> Create()
    {
        var products = await _productService.GetAllAsync();

        var viewModel = new ArticleBulkCreateViewModel
        {
            Products = products.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            })
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ArticleBulkCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var products = await _productService.GetAllAsync();
            model.Products = products.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });

            return View(model);
        }

        for (int i = 0; i < model.Amount; i++)
        {
            var article = new Article
            {
                Id = Guid.NewGuid(),
                ProductId = model.ProductId,
                Status = ArticleStatus.Beschikbaar
            };

            await _service.AddAsync(article);
        }

        TempData["Success"] = $"{model.Amount} artikel(en) succesvol aangemaakt.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var article = await _service.FindByIdAsync(id);
        if (article == null)
            return NotFound();

        return View(article);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Article article)
    {
        if (!ModelState.IsValid)
        {
            return View(article);
        }

        await _service.UpdateAsync(article);
        TempData["Success"] = "Artikel succesvol bijgewerkt.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var article = await _service.FindByIdAsync(id);
        return article == null ? NotFound() : View(article);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var article = await _service.FindByIdAsync(id);
        if (article == null)
        {
            TempData["Error"] = "Artikel niet gevonden.";
            return RedirectToAction(nameof(Index));
        }

        // Validatie: niet verwijderen als verhuurd of gereserveerd
        if (article.Status == ArticleStatus.Verhuurd || article.Status == ArticleStatus.Gereserveerd)
        {
            TempData["Error"] = "Artikel kan niet verwijderd worden omdat het verhuurd of gereserveerd is.";
            return RedirectToAction(nameof(Index));
        }

        await _service.DeleteAsync(article);
        TempData["Success"] = "Artikel succesvol verwijderd.";
        return RedirectToAction(nameof(Index));
    }
}
