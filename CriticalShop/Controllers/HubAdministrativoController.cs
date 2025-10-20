using Microsoft.AspNetCore.Mvc;
using CriticalShop.Services;
using CriticalShop.Data;
using CriticalShop.Models;
using Microsoft.EntityFrameworkCore;

namespace CriticalShop.Controllers
{
    public class HubAdministrativoController : Controller
    {
        private readonly AuthService _authService;
        private readonly AppDbContext _context;
        private readonly ILogger<HubAdministrativoController> _logger;

        public HubAdministrativoController(AuthService authService, AppDbContext context, ILogger<HubAdministrativoController> logger)
        {
            _authService = authService;
            _context = context;
            _logger = logger;
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

        // GET: Hub Administrativo
        public IActionResult Index()
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            try
            {
                var dashboard = new
                {
                    TotalProdutos = _context.Produtos.Count(),
                    TotalCategorias = _context.Categorias.Count(),
                    TotalUsuarios = _context.Usuarios.Count(),
                    TotalAdmins = _context.Admins.Count(),
                    TotalCarrinhos = _context.Carrinhos.Count(),
                    AdminNome = _authService.GetAdminNome()
                };

                return View(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dashboard administrativo");
                TempData["ErrorMessage"] = "Erro ao carregar dados do dashboard.";
                return View();
            }
        }

        // GET: Gerenciar Produtos
        public async Task<IActionResult> Produtos()
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            try
            {
                var produtos = await _context.Produtos
                    .Include(p => p.Categoria)
                    .Include(p => p.Desconto)
                    .OrderBy(p => p.Nome)
                    .ToListAsync();

                return View(produtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar produtos no hub administrativo");
                TempData["ErrorMessage"] = "Erro ao carregar produtos.";
                return View(new List<Produto>());
            }
        }

        // GET: Gerenciar Categorias
        public async Task<IActionResult> Categorias()
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            try
            {
                var categorias = await _context.Categorias
                    .Include(c => c.SubCategorias)
                    .Include(c => c.Produtos)
                    .OrderBy(c => c.Nome)
                    .ToListAsync();

                return View(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar categorias no hub administrativo");
                TempData["ErrorMessage"] = "Erro ao carregar categorias.";
                return View(new List<Categoria>());
            }
        }

        // GET: Gerenciar Usuarios
        public async Task<IActionResult> Usuarios()
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            try
            {
                var usuarios = await _context.Usuarios
                    .OrderBy(u => u.Nome)
                    .ToListAsync();

                return View(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar usuários no hub administrativo");
                TempData["ErrorMessage"] = "Erro ao carregar usuários.";
                return View(new List<Usuario>());
            }
        }

        // GET: Gerenciar Admins
        public async Task<IActionResult> Admins()
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            // Apenas super admins podem gerenciar outros admins
            if (!_authService.IsSuperAdmin())
            {
                TempData["ErrorMessage"] = "Apenas super administradores podem gerenciar outros admins.";
                return RedirectToAction("Index");
            }

            try
            {
                var admins = await _context.Admins
                    .OrderBy(a => a.Nome)
                    .ToListAsync();

                return View(admins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar admins no hub administrativo");
                TempData["ErrorMessage"] = "Erro ao carregar administradores.";
                return View(new List<Admin>());
            }
        }

        // GET: Gerenciar Carrinhos
        public async Task<IActionResult> Carrinhos()
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            try
            {
                var carrinhos = await _context.Carrinhos
                    .Include(c => c.Usuario)
                    .Include(c => c.Itens)
                        .ThenInclude(i => i.Produto)
                    .OrderByDescending(c => c.DataCriacao)
                    .ToListAsync();

                return View(carrinhos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar carrinhos no hub administrativo");
                TempData["ErrorMessage"] = "Erro ao carregar carrinhos.";
                return View(new List<Carrinho>());
            }
        }

        // GET: Estatísticas
        public async Task<IActionResult> Estatisticas()
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            try
            {
                var estatisticas = new
                {
                    TotalProdutos = _context.Produtos.Count(),
                    TotalCategorias = _context.Categorias.Count(),
                    TotalUsuarios = _context.Usuarios.Count(),
                    TotalAdmins = _context.Admins.Count(),
                    TotalCarrinhos = _context.Carrinhos.Count(),
                    TotalItensCarrinho = _context.ItensCarrinho.Count(),
                    ProdutosPorCategoria = await _context.Produtos
                        .GroupBy(p => p.Categoria.Nome)
                        .Select(g => new { Categoria = g.Key, Quantidade = g.Count() })
                        .ToListAsync(),
                    UsuariosAtivos = _context.Usuarios.Count(u => u.Ativo),
                    AdminsAtivos = _context.Admins.Count(a => a.Ativo)
                };

                return View(estatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar estatísticas no hub administrativo");
                TempData["ErrorMessage"] = "Erro ao carregar estatísticas.";
                return View();
            }
        }
    }
}
