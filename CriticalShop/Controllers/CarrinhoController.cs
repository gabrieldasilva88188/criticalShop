using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CriticalShop.Models;
using CriticalShop.Data;
using CriticalShop.Services;

namespace CriticalShop.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CarrinhoController> _logger;
        private readonly FreteService _freteService;

        public CarrinhoController(AppDbContext context, ILogger<CarrinhoController> logger, FreteService freteService)
        {
            _context = context;
            _logger = logger;
            _freteService = freteService;
        }

        public class AddToCartRequest
        {
            public int ProdutoId { get; set; }
            public int Quantidade { get; set; } = 1;
        }

        public class UpdateQuantityRequest
        {
            public int ItemId { get; set; }
            public int NovaQuantidade { get; set; }
        }

        public class RemoveItemRequest
        {
            public int ItemId { get; set; }
        }

        // GET: Carrinho
        public async Task<IActionResult> Index()
        {
            var sessionId = GetOrCreateSessionId();
            var carrinho = await ObterCarrinhoAsync(sessionId);
            
            return View(carrinho);
        }

        // POST: Carrinho/Adicionar
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Adicionar([FromBody] AddToCartRequest request)
        {
            try
            {
                var produto = await _context.Produtos
                    .Include(p => p.Desconto)
                    .FirstOrDefaultAsync(p => p.Id == request.ProdutoId);

                if (produto == null)
                {
                    return Json(new { success = false, message = "Produto não encontrado" });
                }

                var sessionId = GetOrCreateSessionId();
                var carrinho = await ObterOuCriarCarrinhoAsync(sessionId);

                // Verificar se o produto já está no carrinho
                var itemExistente = carrinho.Itens.FirstOrDefault(i => i.ProdutoId == request.ProdutoId);

                if (itemExistente != null)
                {
                    itemExistente.Quantidade += request.Quantidade;
                }
                else
                {
                    var precoFinal = CalcularPrecoFinal(produto);
                    
                    var novoItem = new ItemCarrinho
                    {
                        CarrinhoId = carrinho.Id,
                        ProdutoId = request.ProdutoId,
                        Quantidade = request.Quantidade,
                        PrecoUnitario = produto.Preco,
                        PrecoFinal = precoFinal
                    };

                    carrinho.Itens.Add(novoItem);
                }

                carrinho.DataAtualizacao = DateTime.Now;
                await _context.SaveChangesAsync();

                var totalItens = carrinho.TotalItens;
                var total = carrinho.TotalComDesconto;

                return Json(new 
                { 
                    success = true, 
                    message = "Produto adicionado ao carrinho!",
                    totalItens = totalItens,
                    total = total.ToString("C")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar produto ao carrinho");
                return Json(new { success = false, message = $"Erro ao adicionar produto ao carrinho: {ex.Message}" });
            }
        }

        // POST: Carrinho/AtualizarQuantidade
        [HttpPost]
        public async Task<IActionResult> AtualizarQuantidade([FromBody] UpdateQuantityRequest request)
        {
            try
            {
                if (request.NovaQuantidade <= 0)
                {
                    return await RemoverItem(new RemoveItemRequest { ItemId = request.ItemId });
                }

                var item = await _context.ItensCarrinho
                    .Include(i => i.Carrinho)
                    .FirstOrDefaultAsync(i => i.Id == request.ItemId);

                if (item == null)
                {
                    return Json(new { success = false, message = "Item não encontrado" });
                }

                item.Quantidade = request.NovaQuantidade;
                if (item.Carrinho != null)
                {
                    item.Carrinho.DataAtualizacao = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                var subtotal = item.Subtotal;
                var totalCarrinho = item.Carrinho?.TotalComDesconto ?? 0;
                var totalItens = item.Carrinho?.TotalItens ?? 0;

                return Json(new 
                { 
                    success = true,
                    subtotal = subtotal.ToString("C"),
                    total = totalCarrinho.ToString("C"),
                    totalItens = totalItens
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar quantidade do item");
                return Json(new { success = false, message = "Erro ao atualizar quantidade" });
            }
        }

        // POST: Carrinho/RemoverItem
        [HttpPost]
        public async Task<IActionResult> RemoverItem([FromBody] RemoveItemRequest request)
        {
            try
            {
                var item = await _context.ItensCarrinho
                    .Include(i => i.Carrinho)
                    .FirstOrDefaultAsync(i => i.Id == request.ItemId);

                if (item == null)
                {
                    return Json(new { success = false, message = "Item não encontrado" });
                }

                var carrinhoId = item.CarrinhoId;
                _context.ItensCarrinho.Remove(item);
                
                var carrinho = await _context.Carrinhos.FindAsync(carrinhoId);
                if (carrinho != null)
                {
                    carrinho.DataAtualizacao = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                // Recalcular totais
                carrinho = await ObterCarrinhoAsync(carrinho.SessionId);
                var totalItens = carrinho?.TotalItens ?? 0;
                var total = carrinho?.TotalComDesconto ?? 0;

                return Json(new 
                { 
                    success = true, 
                    message = "Item removido do carrinho",
                    totalItens = totalItens,
                    total = total.ToString("C")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover item do carrinho");
                return Json(new { success = false, message = "Erro ao remover item" });
            }
        }

        // POST: Carrinho/Limpar
        [HttpPost]
        public async Task<IActionResult> Limpar()
        {
            try
            {
                var sessionId = GetOrCreateSessionId();
                var carrinho = await ObterCarrinhoAsync(sessionId);

                if (carrinho != null)
                {
                    _context.ItensCarrinho.RemoveRange(carrinho.Itens);
                    carrinho.DataAtualizacao = DateTime.Now;
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Carrinho limpo com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao limpar carrinho");
                return Json(new { success = false, message = "Erro ao limpar carrinho" });
            }
        }

        // GET: Carrinho/Contador
        public async Task<IActionResult> Contador()
        {
            var sessionId = GetOrCreateSessionId();
            var carrinho = await ObterCarrinhoAsync(sessionId);
            
            return Json(new { totalItens = carrinho?.TotalItens ?? 0 });
        }

        // POST: Carrinho/CalcularFrete
        [HttpPost]
        public async Task<IActionResult> CalcularFrete([FromBody] FreteRequest request)
        {
            try
            {
                if (!_freteService.ValidarCep(request.CepDestino))
                {
                    return Json(new { sucesso = false, mensagem = "CEP inválido" });
                }

                var sessionId = GetOrCreateSessionId();
                var carrinho = await ObterCarrinhoAsync(sessionId);

                if (carrinho == null || !carrinho.Itens.Any())
                {
                    return Json(new { sucesso = false, mensagem = "Carrinho vazio" });
                }

                var resultado = await _freteService.CalcularFreteAsync(request.CepDestino, carrinho.Itens.ToList());

                if (!resultado.Sucesso)
                {
                    return Json(new { sucesso = false, mensagem = resultado.Mensagem });
                }

                return Json(new
                {
                    sucesso = true,
                    mensagem = resultado.Mensagem,
                    cepDestino = resultado.CepDestino,
                    cidade = resultado.Cidade,
                    estado = resultado.Estado,
                    opcoes = resultado.Opcoes.Select(o => new
                    {
                        servico = o.Servico,
                        nome = o.Nome,
                        valor = o.Valor,
                        valorFormatado = o.Valor.ToString("C"),
                        prazoEntrega = o.PrazoEntrega,
                        observacao = o.Observacao
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular frete");
                return Json(new { sucesso = false, mensagem = "Erro ao calcular frete. Tente novamente." });
            }
        }

        // Métodos auxiliares
        private string GetOrCreateSessionId()
        {
            var sessionId = HttpContext.Session.GetString("SessionId");
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("SessionId", sessionId);
            }
            return sessionId;
        }

        private async Task<Carrinho?> ObterCarrinhoAsync(string sessionId)
        {
            return await _context.Carrinhos
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Produto)
                        .ThenInclude(p => p.Categoria)
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Produto)
                        .ThenInclude(p => p.Desconto)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);
        }

        private async Task<Carrinho> ObterOuCriarCarrinhoAsync(string sessionId)
        {
            var carrinho = await ObterCarrinhoAsync(sessionId);
            
            if (carrinho == null)
            {
                carrinho = new Carrinho
                {
                    SessionId = sessionId,
                    DataCriacao = DateTime.Now
                };
                _context.Carrinhos.Add(carrinho);
                await _context.SaveChangesAsync();
            }

            return carrinho;
        }

        private double CalcularPrecoFinal(Produto produto)
        {
            var precoOriginal = produto.Preco;
            var precoComDesconto = produto.Desconto != null ? 
                precoOriginal * (1 - produto.Desconto.Valor / 100.0) : precoOriginal;
            
            // Aplicar campanha se ativa
            var campanhaAtiva = CampanhaController.CampanhaAtiva;
            var precoFinal = campanhaAtiva ? precoComDesconto * 0.5 : precoComDesconto;
            
            return precoFinal;
        }
    }
}
