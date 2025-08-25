using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CriticalShop.Models
{
    public class Avaliacao
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public int ProdutoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public DateTime Data { get; set; } = DateTime.Now;

        public Usuario Usuario { get; set; } = null!;
        public Produto Produto { get; set; } = null!;
    }
}