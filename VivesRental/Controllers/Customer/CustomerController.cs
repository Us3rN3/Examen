using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Services.Interfaces;

public class CustomerController : Controller
{
    private readonly IService<Customer> _service;

    public CustomerController(IService<Customer> service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var customers = await _service.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            customers = customers.Where(c =>
                c.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return View(customers);
    }


    public async Task<IActionResult> Details(Guid id)
    {
        var customer = await _service.FindByIdAsync(id);
        return customer == null ? NotFound() : View(customer);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (!ModelState.IsValid)
            return View(customer);

        await _service.AddAsync(customer);
        TempData["Success"] = "Klant succesvol toegevoegd.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var customer = await _service.FindByIdAsync(id);
        return customer == null ? NotFound() : View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Customer customer)
    {
        if (!ModelState.IsValid)
            return View(customer);

        await _service.UpdateAsync(customer);
        TempData["Success"] = "Klant succesvol bijgewerkt.";
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _service.FindByIdAsync(id);
        return customer == null ? NotFound() : View(customer);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var customer = await _service.FindByIdAsync(id);
        if (customer != null)
        {
            await _service.DeleteAsync(customer);
            TempData["Success"] = "Klant succesvol verwijderd.";
        }
        else
        {
            TempData["Error"] = "Klant niet gevonden. Verwijderen mislukt.";
        }

        return RedirectToAction(nameof(Index));
    }

}
