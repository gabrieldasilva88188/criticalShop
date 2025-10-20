using Microsoft.AspNetCore.Mvc;
using CriticalShop.Services;
using CriticalShop.Models;

namespace CriticalShop.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // GET: Login Admin
        public IActionResult LoginAdmin()
        {
            if (_authService.IsAdminLoggedIn())
            {
                return RedirectToAction("Index", "HubAdministrativo");
            }
            return View();
        }

        // POST: Login Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAdmin(string email, string senha)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                {
                    TempData["ErrorMessage"] = "Email e senha são obrigatórios.";
                    return View();
                }

                var admin = await _authService.LoginAdminAsync(email, senha);
                if (admin != null)
                {
                    TempData["SuccessMessage"] = $"Bem-vindo, {admin.Nome}!";
                    return RedirectToAction("Index", "HubAdministrativo");
                }
                else
                {
                    TempData["ErrorMessage"] = "Email ou senha incorretos.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no login do admin");
                TempData["ErrorMessage"] = "Erro interno. Tente novamente.";
                return View();
            }
        }

        // GET: Login Usuario
        public IActionResult LoginUsuario()
        {
            if (_authService.IsUsuarioLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Login Usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginUsuario(string email, string senha)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                {
                    TempData["ErrorMessage"] = "Email e senha são obrigatórios.";
                    return View();
                }

                var usuario = await _authService.LoginUsuarioAsync(email, senha);
                if (usuario != null)
                {
                    TempData["SuccessMessage"] = $"Bem-vindo, {usuario.Nome}!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Email ou senha incorretos.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no login do usuário");
                TempData["ErrorMessage"] = "Erro interno. Tente novamente.";
                return View();
            }
        }

        // GET: Cadastro Usuario
        public IActionResult CadastroUsuario()
        {
            if (_authService.IsUsuarioLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Cadastro Usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CadastroUsuario(Usuario usuario)
        {
            try
            {
                // Remover erros de validação das propriedades de navegação
                ModelState.Remove("Carrinhos");
                ModelState.Remove("Avaliacoes");

                if (ModelState.IsValid)
                {
                    var sucesso = await _authService.CadastrarUsuarioAsync(usuario);
                    if (sucesso)
                    {
                        TempData["SuccessMessage"] = "Cadastro realizado com sucesso! Faça login para continuar.";
                        return RedirectToAction("LoginUsuario");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Email ou CPF já cadastrado.";
                    }
                }
                else
                {
                    // Log dos erros de validação
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("Erros de validação no cadastro: {Errors}", string.Join(", ", errors));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no cadastro do usuário");
                TempData["ErrorMessage"] = $"Erro interno: {ex.Message}";
            }

            return View(usuario);
        }

        // GET: Cadastro Admin (apenas para super admins)
        public IActionResult CadastroAdmin()
        {
            if (!_authService.IsAdminLoggedIn() || !_authService.IsSuperAdmin())
            {
                TempData["ErrorMessage"] = "Acesso negado.";
                return RedirectToAction("LoginAdmin");
            }
            return View();
        }

        // POST: Cadastro Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CadastroAdmin(Admin admin)
        {
            try
            {
                if (!_authService.IsAdminLoggedIn() || !_authService.IsSuperAdmin())
                {
                    TempData["ErrorMessage"] = "Acesso negado.";
                    return RedirectToAction("LoginAdmin");
                }

                if (ModelState.IsValid)
                {
                    var sucesso = await _authService.CadastrarAdminAsync(admin);
                    if (sucesso)
                    {
                        TempData["SuccessMessage"] = "Administrador cadastrado com sucesso!";
                        return RedirectToAction("Index", "HubAdministrativo");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Email já cadastrado.";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no cadastro do admin");
                TempData["ErrorMessage"] = "Erro interno. Tente novamente.";
            }

            return View(admin);
        }

        // Logout
        public IActionResult Logout()
        {
            _authService.Logout();
            TempData["SuccessMessage"] = "Logout realizado com sucesso!";
            return RedirectToAction("Index", "Home");
        }
    }
}
