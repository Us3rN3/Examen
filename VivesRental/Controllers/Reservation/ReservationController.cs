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
        // Cleanup vóór we iets ophalen
        await CleanupEmptyReservations();

        var allReservations = await _reservationService.GetAllAsync();
        if (allReservations == null)
            return View(new ReservationIndexViewModel());

        // Verlopen reserveringen verwijderen en artikel beschikbaar zetten
        var now = DateTime.Now;
        foreach (var expired in allReservations.Where(r => r.UntilDateTime < now).ToList())
        {
            var article = await _articleService.FindByIdAsync(expired.ArticleId);
            if (article != null)
            {
                article.Status = ArticleStatus.Beschikbaar;
                await _articleService.UpdateAsync(article);
            }

            await _reservationService.DeleteAsync(expired);
        }

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
        await LoadDropdowns();
        var model = new CreateReservationViewModel
        {
            FromDateTime = DateTime.Today,
            UntilDateTime = DateTime.Today.AddDays(1),
            CustomerId = null,
            ProductId = null
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReservationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns();
            return View(model);
        }

        if (!model.CustomerId.HasValue || !model.ProductId.HasValue)
        {
            ModelState.AddModelError("", "Klant en product zijn verplicht.");
            await LoadDropdowns();
            return View(model);
        }

        if (model.FromDateTime.Date < DateTime.Today)
        {
            TempData["Error"] = "Je kan geen reservering maken in het verleden.";
            await LoadDropdowns();
            return View(model);
        }

        if (model.UntilDateTime < model.FromDateTime)
        {
            ModelState.AddModelError("", "De einddatum moet na de startdatum liggen.");
            await LoadDropdowns();
            return View(model);
        }

        var article = (await _articleService.GetAllAsync())
            .FirstOrDefault(a => a.ProductId == model.ProductId.Value && a.Status == ArticleStatus.Beschikbaar);

        if (article == null)
        {
            TempData["Error"] = "Geen beschikbaar artikel voor dit product.";
            await LoadDropdowns();
            return View(model);
        }

        var reservation = new ArticleReservation
        {
            Id = Guid.NewGuid(),
            ArticleId = article.Id,
            CustomerId = model.CustomerId.Value,
            FromDateTime = model.FromDateTime,
            UntilDateTime = model.UntilDateTime
        };

        await _reservationService.AddAsync(reservation);

        article.Status = ArticleStatus.Gereserveerd;
        await _articleService.UpdateAsync(article);

        TempData["SuccessMessage"] = "Reservering succesvol aangemaakt.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns()
    {
        ViewBag.Customers = await _customerService.GetAllAsync();

        var articles = await _articleService.GetAllAsync();

        var availableProductIds = articles
            .Where(a => a.Status == ArticleStatus.Beschikbaar && a.Product != null)
            .Select(a => a.ProductId)
            .Distinct()
            .ToList();

        ViewBag.Products = articles
            .Where(a => availableProductIds.Contains(a.ProductId) && a.Product != null)
            .Select(a => a.Product!)
            .Distinct()
            .ToList();
    }
    private async Task CleanupEmptyReservations()
    {
        var allReservations = await _reservationService.GetAllAsync();
        var allArticles = await _articleService.GetAllAsync();
        var allCustomers = await _customerService.GetAllAsync();

        var validArticleIds = allArticles.Select(a => a.Id).ToHashSet();
        var validCustomerIds = allCustomers.Select(c => c.Id).ToHashSet();

        foreach (var reservation in allReservations.ToList())
        {
            var article = allArticles.FirstOrDefault(a => a.Id == reservation.ArticleId);
            var customerExists = validCustomerIds.Contains(reservation.CustomerId);

            var articleIsValid = article != null &&
                                 (article.Status == ArticleStatus.Verhuurd || article.Status == ArticleStatus.Gereserveerd);

            if (!customerExists || !articleIsValid)
            {
                await _reservationService.DeleteAsync(reservation);
            }
        }
    }

}
