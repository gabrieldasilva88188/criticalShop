using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CriticalShop.Models;
using CriticalShop.Data;

namespace CriticalShop.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProdutoController> _logger;

        public ProdutoController(AppDbContext context, ILogger<ProdutoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Produto
        public async Task<IActionResult> Index()
        {
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
                _logger.LogError(ex, "Erro ao carregar produtos");
                TempData["ErrorMessage"] = "Erro ao carregar a lista de produtos.";
                return View(new List<Produto>());
            }
        }

        // GET: Produto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var produto = await _context.Produtos
                    .Include(p => p.Categoria)
                    .Include(p => p.Desconto)
                    .Include(p => p.Variacoes)
                    .Include(p => p.Avaliacoes)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (produto == null)
                {
                    return NotFound();
                }

                return View(produto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do produto {Id}", id);
                TempData["ErrorMessage"] = "Erro ao carregar os detalhes do produto.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Produto/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();
                var descontos = await _context.Descontos.OrderBy(d => d.Valor).ToListAsync();
                
                ViewBag.Categorias = categorias;
                ViewBag.Descontos = descontos;

                // Verificar se há categorias disponíveis
                if (!categorias.Any())
                {
                    TempData["ErrorMessage"] = "Não há categorias cadastradas. É necessário criar categorias antes de adicionar produtos.";
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados para criação de produto");
                TempData["ErrorMessage"] = "Erro ao carregar dados necessários.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Produto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Preco,Img,Nota,CategoriaId,DescontoId")] Produto produto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validação adicional para categoria
                    var categoria = await _context.Categorias.FindAsync(produto.CategoriaId);
                    if (categoria == null)
                    {
                        ModelState.AddModelError("CategoriaId", "Categoria selecionada não existe.");
                        ViewBag.Categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();
                        ViewBag.Descontos = await _context.Descontos.OrderBy(d => d.Valor).ToListAsync();
                        return View(produto);
                    }

                    // Validação adicional para desconto (se fornecido)
                    if (produto.DescontoId.HasValue)
                    {
                        var desconto = await _context.Descontos.FindAsync(produto.DescontoId.Value);
                        if (desconto == null)
                        {
                            ModelState.AddModelError("DescontoId", "Desconto selecionado não existe.");
                            ViewBag.Categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();
                            ViewBag.Descontos = await _context.Descontos.OrderBy(d => d.Valor).ToListAsync();
                            return View(produto);
                        }
                    }

                    _context.Add(produto);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Produto criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar produto");
                ModelState.AddModelError("", "Erro interno ao criar o produto. Tente novamente.");
            }

            ViewBag.Categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();
            ViewBag.Descontos = await _context.Descontos.OrderBy(d => d.Valor).ToListAsync();
            return View(produto);
        }

        // GET: Produto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var produto = await _context.Produtos.FindAsync(id);
                if (produto == null)
                {
                    return NotFound();
                }

                var categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();
                var descontos = await _context.Descontos.OrderBy(d => d.Valor).ToListAsync();
                
                ViewBag.Categorias = categorias;
                ViewBag.Descontos = descontos;

                // Verificar se há categorias disponíveis
                if (!categorias.Any())
                {
                    TempData["ErrorMessage"] = "Não há categorias cadastradas. É necessário criar categorias antes de editar produtos.";
                }

                return View(produto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar produto para edição {Id}", id);
                TempData["ErrorMessage"] = "Erro ao carregar dados para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Produto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Preco,Img,Nota,CategoriaId,DescontoId")] Produto produto)
        {
            if (id != produto.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Validação adicional para categoria
                    var categoria = await _context.Categorias.FindAsync(produto.CategoriaId);
                    if (categoria == null)
                    {
                        ModelState.AddModelError("CategoriaId", "Categoria selecionada não existe.");
                        ViewBag.Categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();
                        ViewBag.Descontos = await _context.Descontos.OrderBy(d => d.Valor).ToListAsync();
                        return View(produto);
                    }

                    // Validação adicional para desconto (se fornecido)
                    if (produto.DescontoId.HasValue)
                    {
                        var desconto = await _context.Descontos.FindAsync(produto.DescontoId.Value);
                        if (desconto == null)
                        {
                            ModelState.AddModelError("DescontoId", "Desconto selecionado não existe.");
                            ViewBag.Categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();
                            ViewBag.Descontos = await _context.Descontos.OrderBy(d => d.Valor).ToListAsync();
                            return View(produto);
                        }
                    }

                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Produto atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(produto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar produto {Id}", id);
                ModelState.AddModelError("", "Erro interno ao atualizar o produto. Tente novamente.");
            }

            ViewBag.Categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();
            ViewBag.Descontos = await _context.Descontos.OrderBy(d => d.Valor).ToListAsync();
            return View(produto);
        }

        // GET: Produto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var produto = await _context.Produtos
                    .Include(p => p.Categoria)
                    .Include(p => p.Desconto)
                    .Include(p => p.Variacoes)
                    .Include(p => p.Avaliacoes)
                    .FirstOrDefaultAsync(m => m.Id == id);
                
                if (produto == null)
                {
                    return NotFound();
                }

                return View(produto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar produto para exclusão {Id}", id);
                TempData["ErrorMessage"] = "Erro ao carregar dados para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Produto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var produto = await _context.Produtos
                    .Include(p => p.Variacoes)
                    .Include(p => p.Avaliacoes)
                    .FirstOrDefaultAsync(p => p.Id == id);
                
                if (produto != null)
                {
                    _context.Produtos.Remove(produto);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Produto excluído com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Produto não encontrado.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir produto {Id}", id);
                TempData["ErrorMessage"] = "Erro ao excluir o produto. Tente novamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
    }
}
