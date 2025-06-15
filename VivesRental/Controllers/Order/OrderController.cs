using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Domains.Enums;
using VivesRental.Models.Product;
using VivesRental.Services;
using VivesRental.Services.Interfaces;

public class OrderController : Controller
{
    private readonly IService<Order> _orderService;
    private readonly IService<Customer> _customerService;
    private readonly IService<Product> _productService;
    private readonly IService<Article> _articleService;
    private readonly IService<OrderLine> _orderLineService;
    public OrderController(
    IService<Order> orderService,
    IService<Customer> customerService,
    IService<Product> productService,
    IService<Article> articleService,
    IService<OrderLine> orderLineService)
    {
        _orderService = orderService;
        _customerService = customerService;
        _productService = productService;
        _articleService = articleService;
        _orderLineService = orderLineService;
    }


    // Stap 1: Klant selecteren
    public async Task<IActionResult> Start()
    {
        var customers = await _customerService.GetAllAsync();
        return View(customers);
    }

    // Stap 2: Order aanmaken voor geselecteerde klant
    public async Task<IActionResult> Create(Guid customerId)
    {
        var customer = await _customerService.FindByIdAsync(customerId);
        if (customer == null)
            return NotFound();

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            CustomerEmail = customer.Email,
            CustomerFirstName = customer.FirstName, 
            CustomerLastName = customer.LastName,
            CustomerPhoneNumber = customer.PhoneNumber,
            CreatedAt = DateTime.Now
        };

        await _orderService.AddAsync(order);

        return RedirectToAction("SelectProducts", new { orderId = order.Id });
    }
    public async Task<IActionResult> SelectProducts(Guid orderId)
    {
        var order = await _orderService.FindByIdAsync(orderId);
        if (order == null)
            return NotFound();

        var products = await _productService.GetAllAsync();
        var articles = await _articleService.GetAllAsync();

        // Filter op beschikbare artikelen
        var availableArticles = articles
            .Where(a => a.Status == 0) // 0 = Beschikbaar
            .ToList();

        // Bouw de lijst op per product
        var model = products.Select(p =>
        {
            var count = availableArticles.Count(a => a.ProductId == p.Id);

            return new ProductWithAvailabilityViewModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                AvailableCount = count
            };
        })
        .Where(p => p.AvailableCount > 0) // Alleen tonen als er iets beschikbaar is
        .ToList();

        ViewBag.OrderId = orderId;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddToOrder(Guid orderId, Guid productId)
    {
        // Zoek eerste beschikbare artikel
        var articles = await _articleService.GetAllAsync();
        var availableArticle = articles
            .FirstOrDefault(a => a.ProductId == productId && a.Status == 0);

        if (availableArticle == null)
        {
            TempData["Error"] = "Geen beschikbaar artikel meer voor dit product.";
            return RedirectToAction("SelectProducts", new { orderId });
        }

        // Laad het product op via artikel.ProductId
        var product = await _productService.FindByIdAsync(productId);
        if (product == null)
        {
            TempData["Error"] = "Product niet gevonden.";
            return RedirectToAction("SelectProducts", new { orderId });
        }

        var now = DateTime.Now;

        // Maak nieuwe OrderLine aan
        var orderLine = new OrderLine
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ArticleId = availableArticle.Id,
            ProductName = product.Name,
            ProductDescription = product.Description,
            RentedAt = now,
            ExpiresAt = now.AddDays(7) // ← stel zelf huurtermijn in (bijv. 7 dagen)
        };

        // Artikelstatus wijzigen naar "verhuurd"
        availableArticle.Status = ArticleStatus.Verhuurd;

        // Opslaan
        await _orderLineService.AddAsync(orderLine);
        await _articleService.UpdateAsync(availableArticle);

        return RedirectToAction("SelectProducts", new { orderId });
    }

    [HttpGet]
    public IActionResult SearchReturn()
    {
        return View("SearchReturn");
    }
    public async Task<IActionResult> Return(Guid orderId)
    {
        var order = await _orderService.FindByIdAsync(orderId);
        if (order == null)
            return NotFound();

        var orderLines = await _orderLineService.GetAllAsync();

        // Filter alle lijnen van dit order
        var lines = orderLines
            .Where(ol => ol.OrderId == orderId)
            .ToList();

        ViewBag.Order = order;
        return View(lines);
    }

    [HttpPost]
    public async Task<IActionResult> ReturnArticle(Guid orderLineId, Guid orderId)
    {
        var orderLine = await _orderLineService.FindByIdAsync(orderLineId);
        if (orderLine == null)
            return NotFound();

        if (orderLine.ReturnedAt.HasValue)
        {
            // Artikel was al teruggebracht
            return RedirectToAction("Return", new { orderId });
        }

        // Zet ReturnedAt in
        orderLine.ReturnedAt = DateTime.Now;

        await _orderLineService.UpdateAsync(orderLine);

        // Haal het artikel op en maak beschikbaar
        if (orderLine.ArticleId.HasValue)
        {
            var article = await _articleService.FindByIdAsync(orderLine.ArticleId.Value);
            if (article != null)
            {
                article.Status = 0; // Beschikbaar
                await _articleService.UpdateAsync(article);
            }
        }

        return RedirectToAction("Return", new { orderId });
    }

    public async Task<IActionResult> Overview()
    {
        var orders = await _orderService.GetAllAsync();

        var grouped = orders
            .GroupBy(o => new { o.CustomerId, o.CustomerFirstName, o.CustomerLastName, o.CustomerEmail })
            .ToList();

        ViewBag.GroupedOrders = grouped;
        return View();
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var order = await _orderService.FindByIdAsync(id);
        if (order == null)
            return NotFound();

        var allOrderLines = await _orderLineService.GetAllAsync();
        var lines = allOrderLines.Where(ol => ol.OrderId == id).ToList();

        if (!lines.Any())
        {
            TempData["Info"] = "Er zijn geen artikelen gekoppeld aan deze bestelling.";
            return RedirectToAction("Overview");
        }

        ViewBag.Order = order;
        return View(lines);
    }

    [HttpPost]
    public async Task<IActionResult> ReturnAll(Guid orderId)
    {
        var orderLines = await _orderLineService.GetAllAsync();
        var lines = orderLines.Where(ol => ol.OrderId == orderId && !ol.ReturnedAt.HasValue).ToList();

        foreach (var line in lines)
        {
            line.ReturnedAt = DateTime.Now;
            await _orderLineService.UpdateAsync(line);

            if (line.ArticleId.HasValue)
            {
                var article = await _articleService.FindByIdAsync(line.ArticleId.Value);
                if (article != null)
                {
                    article.Status = 0; // Beschikbaar
                    await _articleService.UpdateAsync(article);
                }
            }
        }

        return RedirectToAction("Overview");
    }

}
