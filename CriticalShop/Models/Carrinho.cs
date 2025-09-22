using System.ComponentModel.DataAnnotations;

namespace CriticalShop.Models
{
    public class Carrinho
    {
        public int Id { get; set; }
        
        [Required]
        public string SessionId { get; set; } = string.Empty;
        
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataAtualizacao { get; set; }
        
        public ICollection<ItemCarrinho> Itens { get; set; } = new List<ItemCarrinho>();
        
        // Propriedades calculadas
        public int TotalItens => Itens.Sum(i => i.Quantidade);
        public double Total => Itens.Sum(i => i.PrecoUnitario * i.Quantidade);
        public double TotalComDesconto => Itens.Sum(i => i.PrecoFinal * i.Quantidade);
    }
}

