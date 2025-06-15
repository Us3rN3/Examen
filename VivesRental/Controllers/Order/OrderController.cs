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

    public async Task<IActionResult> Create(Guid customerId)
    {
        var customer = await _customerService.FindByIdAsync(customerId);
        if (customer == null)
        {
            TempData["Error"] = "Klant niet gevonden.";
            return RedirectToAction(nameof(Start));
        }

        var products = await _productService.GetAllAsync();
        var articles = await _articleService.GetAllAsync();

        var availableArticles = articles
            .Where(a => a.Status == ArticleStatus.Beschikbaar)
            .ToList();

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
        .Where(p => p.AvailableCount > 0)
        .ToList();

        ViewBag.Customer = customer;
        ViewBag.CustomerId = customerId;

        return View("SelectProducts", model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid customerId, Dictionary<Guid, int> quantities)
    {
        if (quantities == null || !quantities.Any(q => q.Value > 0))
        {
            TempData["Error"] = "Selecteer minstens één product.";
            return RedirectToAction(nameof(Create), new { customerId });
        }

        var customer = await _customerService.FindByIdAsync(customerId);
        if (customer == null)
        {
            TempData["Error"] = "Klant niet gevonden.";
            return RedirectToAction(nameof(Start));
        }

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
        var articles = await _articleService.GetAllAsync();

        foreach (var entry in quantities.Where(q => q.Value > 0))
        {
            var productId = entry.Key;
            var amount = entry.Value;

            var availableArticles = articles
                .Where(a => a.ProductId == productId && a.Status == ArticleStatus.Beschikbaar)
                .Take(amount)
                .ToList();

            if (availableArticles.Count < amount)
            {
                TempData["Error"] = "Niet genoeg artikelen beschikbaar voor een van de producten.";
                await _orderService.DeleteAsync(order);
                return RedirectToAction(nameof(Create), new { customerId });
            }

            var product = await _productService.FindByIdAsync(productId);

            foreach (var article in availableArticles)
            {
                var orderLine = new OrderLine
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ArticleId = article.Id,
                    ProductName = product.Name,
                    ProductDescription = product.Description,
                    RentedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddDays(7)
                };

                article.Status = ArticleStatus.Verhuurd;

                await _orderLineService.AddAsync(orderLine);
                await _articleService.UpdateAsync(article);
            }
        }

        TempData["Success"] = "Order succesvol aangemaakt.";
        return RedirectToAction("Overview");
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
