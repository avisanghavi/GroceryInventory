using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GroceryInventory.Web.Models;
using GroceryInventory.Core.Models;
using GroceryInventory.Web.Services;

namespace GroceryInventory.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApiService _apiService;

    public HomeController(ILogger<HomeController> logger, ApiService apiService)
    {
        _logger = logger;
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _apiService.GetGroceryItemsAsync();
        var suppliers = await _apiService.GetSuppliersAsync();
        var orders = await _apiService.GetOrdersAsync();

        var viewModel = new DashboardViewModel
        {
            GroceryItems = items,
            Suppliers = suppliers,
            Orders = orders
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class DashboardViewModel
{
    public IEnumerable<GroceryItem> GroceryItems { get; set; } = Array.Empty<GroceryItem>();
    public IEnumerable<Supplier> Suppliers { get; set; } = Array.Empty<Supplier>();
    public IEnumerable<Order> Orders { get; set; } = Array.Empty<Order>();
}
