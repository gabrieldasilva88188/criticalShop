using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CriticalShop.Models
{
    public class Variacao
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal Preco { get; set; }

        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;
    }
}