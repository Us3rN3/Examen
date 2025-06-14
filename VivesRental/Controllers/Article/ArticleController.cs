using Microsoft.AspNetCore.Mvc;
using VivesRental.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using VivesRental.Domains.EntitiesDB;
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

    public async Task<IActionResult> Index()
    {
        var articles = await _service.GetAllAsync();
        return View(articles);
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
                Status = 0 // Standaard op "Beschikbaar"
            };

            await _service.AddAsync(article);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var article = await _service.FindByIdAsync(id);

        if (article == null)
            return NotFound();

        Console.WriteLine("Edit GET - ProductId: " + article.ProductId); // Check of ProductId bestaat

        return View(article);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(Article article)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("Validatiefout: " + error.ErrorMessage);
            }

            return View(article);
        }

        await _service.UpdateAsync(article);
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
        if (article != null)
            await _service.DeleteAsync(article);

        return RedirectToAction(nameof(Index));
    }
}
