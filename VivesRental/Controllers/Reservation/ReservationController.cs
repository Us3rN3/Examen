using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Domains.Enums;
using VivesRental.Models.Reservation;
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
    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
    {
        var allReservations = await _reservationService.GetAllAsync();
        if (allReservations == null)
            return View(new ReservationIndexViewModel());

        var filtered = allReservations;

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            filtered = allReservations.Where(r =>
                r.Customer.FirstName.ToLower().Contains(search) ||
                r.Customer.LastName.ToLower().Contains(search)
            );
        }

        var totalCount = filtered.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var reservations = filtered
            .OrderByDescending(r => r.FromDateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var viewModel = new ReservationIndexViewModel
        {
            Reservations = reservations,
            Search = search,
            PageNumber = page,
            TotalPages = totalPages
        };

        return View(viewModel);
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
}
