using System.ComponentModel.DataAnnotations;

namespace CriticalShop.Models
{
    public class ItemCarrinho
    {
        public int Id { get; set; }
        
        [Required]
        public int CarrinhoId { get; set; }
        public Carrinho? Carrinho { get; set; }
        
        [Required]
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }
        
        [Required]
        public double PrecoUnitario { get; set; }
        
        public double PrecoFinal { get; set; }
        
        public DateTime DataAdicao { get; set; } = DateTime.Now;
        
        // Propriedades calculadas
        public double Subtotal => PrecoFinal * Quantidade;
        public double Economia => (PrecoUnitario - PrecoFinal) * Quantidade;
    }
}

