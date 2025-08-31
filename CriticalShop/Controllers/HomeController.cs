using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CriticalShop.Models;
using System.Collections.Generic;

namespace CriticalShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly CriticalShop.Data.AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, CriticalShop.Data.AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
    var produtos = _context.Produtos.ToList();
    return View(produtos);
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
