using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Models;
using VivesRental.Services.Interfaces;

public class CustomerController : Controller
{
    private readonly IService<Customer> _service;

    public CustomerController(IService<Customer> service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 10)
    {
        var allCustomers = await _service.GetAllAsync();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            allCustomers = allCustomers
                .Where(c => c.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                         || c.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                         c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var totalItems = allCustomers.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var pagedCustomers = allCustomers
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var viewModel = new CustomerIndexViewModel
        {
            Customers = pagedCustomers,
            SearchTerm = searchTerm,
            CurrentPage = page,
            TotalPages = totalPages
        };

        return View(viewModel);
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
        if (customer == null)
        {
            TempData["Error"] = "Klant niet gevonden. Verwijderen mislukt.";
            return RedirectToAction(nameof(Index));
        }

        // Controleer op gekoppelde reservaties of bestellingen
        if (customer.ArticleReservations.Any() || customer.Orders.Any())
        {
            TempData["Error"] = "Klant kan niet verwijderd worden omdat er gekoppelde reservaties of bestellingen zijn.";
            return RedirectToAction(nameof(Index));
        }

        await _service.DeleteAsync(customer);
        TempData["Success"] = "Klant succesvol verwijderd.";
        return RedirectToAction(nameof(Index));
    }


}
