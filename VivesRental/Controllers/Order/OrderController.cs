using Microsoft.AspNetCore.Mvc;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Domains.Enums;
using VivesRental.Models.Order;
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

    public async Task<IActionResult> Start()
    {
        var customers = await _customerService.GetAllAsync();
        return View(customers);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid customerId, string? searchTerm, int page = 1, int pageSize = 10)
    {
        var customer = await _customerService.FindByIdAsync(customerId);
        if (customer == null)
        {
            TempData["Error"] = "Klant niet gevonden.";
            return RedirectToAction(nameof(Start));
        }

        var allProducts = await _productService.GetAllAsync();
        var allArticles = await _articleService.GetAllAsync();

        var availableArticles = allArticles
            .Where(a => a.Status == ArticleStatus.Beschikbaar)
            .ToList();

        // Filter
        var filteredProducts = allProducts
            .Where(p => string.IsNullOrWhiteSpace(searchTerm) ||
                        p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Mapping + Available count
        var productModels = filteredProducts.Select(p =>
        {
            var count = availableArticles.Count(a => a.ProductId == p.Id);
            return new ProductWithAvailabilityViewModel
            {
                ProductId = p.Id,
                ProductName = p.Name,
                AvailableCount = count
            };
        }).Where(p => p.AvailableCount > 0).ToList();

        // Paging
        var total = productModels.Count;
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        var paginatedProducts = productModels
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var model = new SelectProductsViewModel
        {
            Products = paginatedProducts,
            SearchTerm = searchTerm,
            PageNumber = page,
            TotalPages = totalPages,
            CustomerId = customerId
        };

        ViewBag.Customer = customer;
        return View("SelectProducts", model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid customerId, Dictionary<Guid, int> quantities)
    {
        var customer = await _customerService.FindByIdAsync(customerId);
        if (customer == null)
        {
            TempData["Error"] = "Klant niet gevonden.";
            return RedirectToAction(nameof(Start));
        }

        // Maak nieuwe Order aan met alle verplichte velden
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow,
            CustomerFirstName = customer.FirstName,
            CustomerLastName = customer.LastName,
            CustomerEmail = customer.Email,
            CustomerPhoneNumber = customer.PhoneNumber ?? "Onbekend" // fallback als null
        };

        await _orderService.AddAsync(order);

        var allArticles = await _articleService.GetAllAsync();
        var allProducts = await _productService.GetAllAsync();

        foreach (var entry in quantities)
        {
            var productId = entry.Key;
            var quantity = entry.Value;

            if (quantity <= 0)
                continue;

            var availableArticles = allArticles
                .Where(a => a.ProductId == productId && a.Status == ArticleStatus.Beschikbaar)
                .Take(quantity)
                .ToList();

            var product = allProducts.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                TempData["Error"] = "Product niet gevonden.";
                return RedirectToAction(nameof(Start));
            }

            foreach (var article in availableArticles)
            {
                var now = DateTime.Now;

                var orderLine = new OrderLine
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ArticleId = article.Id,
                    RentedAt = now,
                    ExpiresAt = now.AddDays(7), // bijvoorbeeld 7 dagen huren
                    ProductName = product.Name,
                    ProductDescription = product.Description
                };

                // Sla OrderLine op
                await _orderLineService.AddAsync(orderLine); // zorg dat deze AddAsync heet i.p.v. UpdateAsync

                // Update artikel status
                article.Status = ArticleStatus.Verhuurd;
                await _articleService.UpdateAsync(article);
            }
        }

        TempData["Success"] = "Order succesvol aangemaakt.";
        return RedirectToAction(nameof(Overview));
    }


    public async Task<IActionResult> Overview(string searchTerm, int pageNumber = 1, int pageSize = 10)
    {
        await CleanupEmptyOrders();

        var allOrders = await _orderService.GetAllAsync();

        // Filter orders waarbij niet alle artikelen zijn teruggebracht
        var allOrderLines = await _orderLineService.GetAllAsync();
        allOrders = allOrders.Where(order =>
            allOrderLines.Any(ol => ol.OrderId == order.Id && !ol.ReturnedAt.HasValue)
        ).ToList();

        // Filteren op zoekterm als die is ingevuld
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerTerm = searchTerm.Trim().ToLower();
            allOrders = allOrders.Where(o =>
                ($"{o.CustomerFirstName} {o.CustomerLastName}".ToLower().Contains(lowerTerm)) ||
                (o.CustomerEmail?.ToLower().Contains(lowerTerm) ?? false)
            ).ToList();
        }

        // Groeperen per klant
        var grouped = allOrders
            .GroupBy(o => new CustomerGroupKey
            {
                CustomerId = (Guid)o.CustomerId,
                CustomerFirstName = o.CustomerFirstName,
                CustomerLastName = o.CustomerLastName,
                CustomerEmail = o.CustomerEmail
            })
            .OrderBy(g => g.Key.CustomerLastName)
            .ThenBy(g => g.Key.CustomerFirstName)
            .ToList();


        // Paging
        int totalGroups = grouped.Count;
        int totalPages = (int)Math.Ceiling(totalGroups / (double)pageSize);

        grouped = grouped
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var model = new OrderOverviewViewModel
        {
            GroupedOrders = grouped,
            PageNumber = pageNumber,
            TotalPages = totalPages,
            SearchTerm = searchTerm
        };

        return View(model);
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
            TempData["Info"] = "Dit artikel werd al teruggebracht.";
            return RedirectToAction("Return", new { orderId });
        }

        orderLine.ReturnedAt = DateTime.Now;
        await _orderLineService.UpdateAsync(orderLine);

        if (orderLine.ArticleId.HasValue)
        {
            var article = await _articleService.FindByIdAsync(orderLine.ArticleId.Value);
            if (article != null)
            {
                article.Status = ArticleStatus.Beschikbaar;
                await _articleService.UpdateAsync(article);
            }
        }

        TempData["Success"] = "Artikel succesvol teruggebracht.";
        return RedirectToAction("Return", new { orderId });
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
                    article.Status = ArticleStatus.Beschikbaar;
                    await _articleService.UpdateAsync(article);
                }
            }
        }

        TempData["Success"] = "Alle artikelen zijn teruggebracht.";
        return RedirectToAction("Overview");
    }

    public async Task CleanupEmptyOrders()
    {
        var orders = await _orderService.GetAllAsync();
        var orderLines = await _orderLineService.GetAllAsync();

        foreach (var order in orders)
        {
            bool hasLines = orderLines.Any(ol => ol.OrderId == order.Id);
            if (!hasLines)
            {
                await _orderService.DeleteAsync(order);
            }
        }
    }

    public async Task CleanupCompletedOrders()
    {
        var orders = await _orderService.GetAllAsync();
        var orderLines = await _orderLineService.GetAllAsync();

        foreach (var order in orders)
        {
            var lines = orderLines.Where(ol => ol.OrderId == order.Id).ToList();

            if (lines.Any() && lines.All(l => l.ReturnedAt.HasValue))
            {
                // Eerst alle orderlines verwijderen
                foreach (var line in lines)
                {
                    await _orderLineService.DeleteAsync(line);
                }

                // Dan order verwijderen
                await _orderService.DeleteAsync(order);
            }
        }
    }


}
