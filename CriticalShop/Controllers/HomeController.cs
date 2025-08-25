using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CriticalShop.Models;
using System.Collections.Generic;

namespace CriticalShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var produtos = new List<Produto>
        {
            new Produto { Id = 1, Nome = "Dados Poliédricos", Preco = 49.90, Nota = 4.5, CategoriaId = 1 },
            new Produto { Id = 2, Nome = "Manual do Jogador", Preco = 99.90, Nota = 4.8, CategoriaId = 1 },
            new Produto { Id = 3, Nome = "Miniatura Dragão", Preco = 79.90, Nota = 4.2, CategoriaId = 2 }
        };

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
