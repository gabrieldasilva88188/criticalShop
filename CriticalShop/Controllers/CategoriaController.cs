using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CriticalShop.Models;
using CriticalShop.Data;
using CriticalShop.Services;

namespace CriticalShop.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;

        public CategoriaController(AppDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // Verificar se admin está logado
        private bool VerificarAcessoAdmin()
        {
            if (!_authService.IsAdminLoggedIn())
            {
                TempData["ErrorMessage"] = "Acesso negado. Faça login como administrador.";
                return false;
            }
            return true;
        }

        // GET: Categoria
        public async Task<IActionResult> Index()
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            var categorias = await _context.Categorias.Include(c => c.SubCategorias).ToListAsync();
            return View("~/Views/HubAdministrativo/Categoria/Index.cshtml", categorias);
        }

        // GET: Categoria/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            if (id == null) return NotFound();
            var categoria = await _context.Categorias.Include(c => c.SubCategorias).FirstOrDefaultAsync(m => m.Id == id);
            if (categoria == null) return NotFound();
            return View("~/Views/HubAdministrativo/Categoria/Details.cshtml", categoria);
        }

        // GET: Categoria/Create
        public IActionResult Create()
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            ViewBag.Categorias = _context.Categorias.ToList(); // Para selecionar categoria pai
            return View("~/Views/HubAdministrativo/Categoria/Create.cshtml");
        }

        // POST: Categoria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,ParentId")] Categoria categoria)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            if (ModelState.IsValid)
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categorias = _context.Categorias.ToList();
            return View("~/Views/HubAdministrativo/Categoria/Create.cshtml", categoria);
        }

        // GET: Categoria/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            if (id == null) return NotFound();
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();
            ViewBag.Categorias = _context.Categorias.Where(c => c.Id != id).ToList();
            return View("~/Views/HubAdministrativo/Categoria/Edit.cshtml", categoria);
        }

        // POST: Categoria/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,ParentId")] Categoria categoria)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            if (id != categoria.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categorias = _context.Categorias.Where(c => c.Id != id).ToList();
            return View("~/Views/HubAdministrativo/Categoria/Edit.cshtml", categoria);
        }

        // GET: Categoria/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            if (id == null) return NotFound();
            var categoria = await _context.Categorias.Include(c => c.SubCategorias).FirstOrDefaultAsync(m => m.Id == id);
            if (categoria == null) return NotFound();
            return View("~/Views/HubAdministrativo/Categoria/Delete.cshtml", categoria);
        }

        // POST: Categoria/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
