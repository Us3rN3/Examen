using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Services;
using VivesRental.Services.Interfaces;

public class ProductController : Controller
{
    private readonly IService<Product> _service;

    public ProductController(IService<Product> service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _service.GetAllAsync();
        return View(products);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Product aanmaken mislukt. Controleer alle velden.";
            return View(product);
        }

        await _service.AddAsync(product);
        TempData["Success"] = "Product succesvol aangemaakt.";
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Edit(Guid id)
    {
        var product = await _service.FindByIdAsync(id);
        return product == null ? NotFound() : View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Product product)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Product bijwerken mislukt. Controleer de velden.";
            return View(product);
        }

        await _service.UpdateAsync(product);
        TempData["Success"] = "Product succesvol bijgewerkt.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _service.FindByIdAsync(id);
        return product == null ? NotFound() : View(product);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var product = await _service.FindByIdAsync(id);
        if (product == null)
        {
            TempData["Error"] = "Product niet gevonden. Verwijderen mislukt.";
            return RedirectToAction(nameof(Index));
        }

        // Check op gelinkte artikelen met reservaties of orderlijnen
        bool isInGebruik = product.Articles.Any(a =>
            a.ArticleReservations.Any() || a.OrderLines.Any());

        if (isInGebruik)
        {
            TempData["Error"] = "Product kan niet verwijderd worden omdat het gereserveerd of verhuurd is.";
            return RedirectToAction(nameof(Index));
        }

        await _service.DeleteAsync(product);
        TempData["Success"] = "Product succesvol verwijderd.";
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Details(Guid id)
    {
        var product = await _service.FindByIdAsync(id);
        return product == null ? NotFound() : View(product);
    }
}
