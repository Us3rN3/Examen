using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VivesRental.Data;
using VivesRental.Models.Dashboard;
using VivesRental.Domains.DataDB;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly RentalDbContext _context;

    public HomeController(ILogger<HomeController> logger, RentalDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var model = new DashboardViewModel
        {
            TotalCustomers = _context.Customers.Count(),
            TotalProducts = _context.Products.Count(),
            TotalArticles = _context.Articles.Count(),
            ActiveOrders = _context.Orders.Count(),
            ActiveReservations = _context.ArticleReservations.Count(r => r.FromDateTime <= DateTime.Today && r.UntilDateTime >= DateTime.Today)
        };


        return View(model);
    }
}

