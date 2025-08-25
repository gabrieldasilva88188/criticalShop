using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CriticalShop.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public int? ParentId { get; set; }
        public Categoria? Parent { get; set; }
        public ICollection<Categoria>? SubCategorias { get; set; }
        public ICollection<Produto>? Produtos { get; set; }
    }
}