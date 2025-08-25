using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CriticalShop.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public double Preco { get; set; }
        public string? Img { get; set; }
        public double Nota { get; set; }

        // Relacionamentos
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        public int? DescontoId { get; set; }
        public Desconto? Desconto { get; set; }

        public ICollection<Variacao> Variacoes { get; set; } = new List<Variacao>();
        public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
    }
}


