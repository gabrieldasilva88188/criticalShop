using Microsoft.AspNetCore.Mvc;
using CriticalShop.Services;

namespace CriticalShop.Controllers
{
    public class PagamentoController : Controller
    {
        private readonly AuthService _authService;

        public PagamentoController(AuthService authService)
        {
            _authService = authService;
        }

        // GET: Pagamento
        public IActionResult Index()
        {
            // Exigir autenticação do usuário para acessar o fluxo de pagamento
            if (!_authService.IsUsuarioLoggedIn() && !_authService.IsAdminLoggedIn())
            {
                TempData["ErrorMessage"] = "Você precisa estar logado para finalizar a compra.";
                return RedirectToAction("LoginUsuario", "Auth");
            }

            // Por enquanto apenas exibir a página de simulação
            return View();
        }

        // POST: Pagamento/Simular
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SimularPagamento()
        {
            if (!_authService.IsUsuarioLoggedIn() && !_authService.IsAdminLoggedIn())
            {
                TempData["ErrorMessage"] = "Sessão expirada. Faça login novamente.";
                return RedirectToAction("LoginUsuario", "Auth");
            }

            // Em próximos passos integraremos com Mercado Pago (sandbox)
            TempData["SuccessMessage"] = "Pagamento simulado com sucesso (sandbox).";
            return RedirectToAction("Index", "Home");
        }
    }
}
