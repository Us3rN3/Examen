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
        if (!ModelState.IsValid) return View(product);
        await _service.AddAsync(product);
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
        if (!ModelState.IsValid) return View(product);
        await _service.UpdateAsync(product);
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
        if (product != null)
            await _service.DeleteAsync(product);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var product = await _service.FindByIdAsync(id);
        return product == null ? NotFound() : View(product);
    }
}
