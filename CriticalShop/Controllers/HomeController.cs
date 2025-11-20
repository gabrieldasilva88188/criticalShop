using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CriticalShop.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

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

    public async Task<IActionResult> Index(string searchString, string categoria)
    {
        var query = _context.Produtos
            .Include(p => p.Desconto)
            .Include(p => p.Categoria)
            .AsQueryable();

        // Aplicar filtro de busca
        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(p => 
                p.Nome.Contains(searchString) || 
                (p.Descricao != null && p.Descricao.Contains(searchString))
            );
        }

        // Aplicar filtro de categoria
        if (!string.IsNullOrEmpty(categoria))
        {
            query = query.Where(p => p.Categoria != null && p.Categoria.Nome == categoria);
        }

        ViewBag.CampanhaAtiva = CriticalShop.Controllers.CampanhaController.CampanhaAtiva;
        ViewBag.SearchString = searchString;
        ViewBag.CategoriaSelecionada = categoria;

        var produtos = await query.ToListAsync();
        return View(produtos);
    }

    // GET: Home/DetalhesProduto/5
    public async Task<IActionResult> DetalhesProduto(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var produto = await _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.Desconto)
            .Include(p => p.Avaliacoes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto == null)
        {
            return NotFound();
        }

        ViewBag.CampanhaAtiva = CriticalShop.Controllers.CampanhaController.CampanhaAtiva;
        return View(produto);
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
