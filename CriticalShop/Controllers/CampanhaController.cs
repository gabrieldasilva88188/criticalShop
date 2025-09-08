using Microsoft.AspNetCore.Mvc;

namespace CriticalShop.Controllers
{
    public class CampanhaController : Controller
    {
        private static bool _campanhaAtiva = false;

        public IActionResult Index()
        {
            ViewBag.CampanhaAtiva = _campanhaAtiva;
            return View();
        }

        [HttpPost]
        public IActionResult ToggleCampanha(bool campanhaAtiva)
        {
            _campanhaAtiva = campanhaAtiva;
            ViewBag.CampanhaAtiva = _campanhaAtiva;
            return View("Index");
        }

        public static bool CampanhaAtiva => _campanhaAtiva;
    }
}
