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

        // GET: Detalhes do Usuario
        public async Task<IActionResult> DetalhesUsuario(int id)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Carrinhos)
                    .Include(u => u.Avaliacoes)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuário não encontrado.";
                    return RedirectToAction("Usuarios");
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do usuário");
                TempData["ErrorMessage"] = "Erro ao carregar detalhes do usuário.";
                return RedirectToAction("Usuarios");
            }
        }

        // GET: Editar Usuario
        public async Task<IActionResult> EditarUsuario(int id)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuário não encontrado.";
                    return RedirectToAction("Usuarios");
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar usuário para edição");
                TempData["ErrorMessage"] = "Erro ao carregar usuário.";
                return RedirectToAction("Usuarios");
            }
        }

        // POST: Editar Usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(int id, Usuario usuario)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            if (id != usuario.Id)
            {
                TempData["ErrorMessage"] = "ID inválido.";
                return RedirectToAction("Usuarios");
            }

            try
            {
                // Remover validação de propriedades de navegação
                ModelState.Remove("Carrinhos");
                ModelState.Remove("Avaliacoes");

                if (ModelState.IsValid)
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Usuário atualizado com sucesso!";
                    return RedirectToAction("Usuarios");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Usuarios.Any(u => u.Id == id))
                {
                    TempData["ErrorMessage"] = "Usuário não encontrado.";
                    return RedirectToAction("Usuarios");
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário");
                TempData["ErrorMessage"] = "Erro ao atualizar usuário.";
            }

            return View(usuario);
        }

        // GET: Excluir Usuario
        public async Task<IActionResult> ExcluirUsuario(int id)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Carrinhos)
                    .Include(u => u.Avaliacoes)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuário não encontrado.";
                    return RedirectToAction("Usuarios");
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar usuário para exclusão");
                TempData["ErrorMessage"] = "Erro ao carregar usuário.";
                return RedirectToAction("Usuarios");
            }
        }

        // POST: Excluir Usuario
        [HttpPost, ActionName("ExcluirUsuario")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirUsuarioConfirmado(int id)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario != null)
                {
                    _context.Usuarios.Remove(usuario);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Usuário excluído com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Usuário não encontrado.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuário");
                TempData["ErrorMessage"] = "Erro ao excluir usuário. Verifique se há dados relacionados.";
            }

            return RedirectToAction("Usuarios");
        }

        // GET: Detalhes do Admin
        public async Task<IActionResult> DetalhesAdmin(int id)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            if (!_authService.IsSuperAdmin())
            {
                TempData["ErrorMessage"] = "Apenas super administradores podem visualizar detalhes de admins.";
                return RedirectToAction("Index");
            }

            try
            {
                var admin = await _context.Admins.FindAsync(id);
                if (admin == null)
                {
                    TempData["ErrorMessage"] = "Administrador não encontrado.";
                    return RedirectToAction("Admins");
                }

                return View(admin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do admin");
                TempData["ErrorMessage"] = "Erro ao carregar detalhes do administrador.";
                return RedirectToAction("Admins");
            }
        }

        // GET: Editar Admin
        public async Task<IActionResult> EditarAdmin(int id)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            if (!_authService.IsSuperAdmin())
            {
                TempData["ErrorMessage"] = "Apenas super administradores podem editar admins.";
                return RedirectToAction("Index");
            }

            try
            {
                var admin = await _context.Admins.FindAsync(id);
                if (admin == null)
                {
                    TempData["ErrorMessage"] = "Administrador não encontrado.";
                    return RedirectToAction("Admins");
                }

                return View(admin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar admin para edição");
                TempData["ErrorMessage"] = "Erro ao carregar administrador.";
                return RedirectToAction("Admins");
            }
        }

        // POST: Editar Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarAdmin(int id, Admin admin)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            if (!_authService.IsSuperAdmin())
            {
                TempData["ErrorMessage"] = "Apenas super administradores podem editar admins.";
                return RedirectToAction("Index");
            }

            if (id != admin.Id)
            {
                TempData["ErrorMessage"] = "ID inválido.";
                return RedirectToAction("Admins");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Administrador atualizado com sucesso!";
                    return RedirectToAction("Admins");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Admins.Any(a => a.Id == id))
                {
                    TempData["ErrorMessage"] = "Administrador não encontrado.";
                    return RedirectToAction("Admins");
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar admin");
                TempData["ErrorMessage"] = "Erro ao atualizar administrador.";
            }

            return View(admin);
        }

        // GET: Excluir Admin
        public async Task<IActionResult> ExcluirAdmin(int id)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            if (!_authService.IsSuperAdmin())
            {
                TempData["ErrorMessage"] = "Apenas super administradores podem excluir admins.";
                return RedirectToAction("Index");
            }

            try
            {
                var admin = await _context.Admins.FindAsync(id);
                if (admin == null)
                {
                    TempData["ErrorMessage"] = "Administrador não encontrado.";
                    return RedirectToAction("Admins");
                }

                // Não permitir excluir a si mesmo
                if (admin.Id == _authService.GetAdminId())
                {
                    TempData["ErrorMessage"] = "Você não pode excluir sua própria conta.";
                    return RedirectToAction("Admins");
                }

                return View(admin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar admin para exclusão");
                TempData["ErrorMessage"] = "Erro ao carregar administrador.";
                return RedirectToAction("Admins");
            }
        }

        // POST: Excluir Admin
        [HttpPost, ActionName("ExcluirAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirAdminConfirmado(int id)
        {
            if (!VerificarAcessoAdmin())
            {
                return RedirectToAction("LoginAdmin", "Auth");
            }

            if (!_authService.IsSuperAdmin())
            {
                TempData["ErrorMessage"] = "Apenas super administradores podem excluir admins.";
                return RedirectToAction("Index");
            }

            try
            {
                var admin = await _context.Admins.FindAsync(id);
                if (admin != null)
                {
                    // Não permitir excluir a si mesmo
                    if (admin.Id == _authService.GetAdminId())
                    {
                        TempData["ErrorMessage"] = "Você não pode excluir sua própria conta.";
                        return RedirectToAction("Admins");
                    }

                    _context.Admins.Remove(admin);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Administrador excluído com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Administrador não encontrado.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir admin");
                TempData["ErrorMessage"] = "Erro ao excluir administrador.";
            }

            return RedirectToAction("Admins");
        }
    }
}
