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

    public async Task<IActionResult> Index()
    {
        var customers = await _service.GetAllAsync();
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
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("Validatiefout: " + error.ErrorMessage);
            }

            return View(customer);
        }


        await _service.AddAsync(customer);
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
        if (!ModelState.IsValid) return View(customer);

        await _service.UpdateAsync(customer);
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
            await _service.DeleteAsync(customer);

        return RedirectToAction(nameof(Index));
    }
}
