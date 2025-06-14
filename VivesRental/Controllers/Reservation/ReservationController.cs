using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Domains.Enums;
using VivesRental.Models;
using VivesRental.Services.Interfaces;

public class ReservationController : Controller
{
    private readonly IService<ArticleReservation> _reservationService;
    private readonly IService<Customer> _customerService;
    private readonly IService<Article> _articleService;

    public ReservationController(
        IService<ArticleReservation> reservationService,
        IService<Customer> customerService,
        IService<Article> articleService)
    {
        _reservationService = reservationService;
        _customerService = customerService;
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Customers = await _customerService.GetAllAsync();

        ViewBag.Articles = (await _articleService.GetAllAsync())
            .Where(a => a.Status == 0 && a.Product != null)
            .ToList();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReservationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Customers = await _customerService.GetAllAsync();
            ViewBag.Articles = await _articleService.GetAllAsync();
            return View(model);
        }

        var reservation = new ArticleReservation
        {
            Id = Guid.NewGuid(),
            ArticleId = model.ArticleId,
            CustomerId = model.CustomerId,
            FromDateTime = model.FromDateTime,
            UntilDateTime = model.UntilDateTime
        };

        await _reservationService.AddAsync(reservation);

        var article = await _articleService.FindByIdAsync(model.ArticleId);
        if (article != null)
        {
            article.Status = ArticleStatus.Gereserveerd;
            await _articleService.UpdateAsync(article);
        }

        return RedirectToAction("Index", "Home");
    }
    [HttpGet]
    public async Task<IActionResult> Index(string? search)
    {
        var allReservations = await _reservationService.GetAllAsync();
        if (allReservations == null)
            return View(new List<ArticleReservation>());

        var filtered = allReservations;

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            filtered = allReservations.Where(r =>
                r.Customer.FirstName.ToLower().Contains(search) ||
                r.Customer.LastName.ToLower().Contains(search)
            );
        }

        return View(filtered.OrderByDescending(r => r.FromDateTime).ToList());
    }

}
