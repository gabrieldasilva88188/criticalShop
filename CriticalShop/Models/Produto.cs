using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CriticalShop.Models
{
    public class Produto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
        [Display(Name = "Nome do Produto")]
        public string Nome { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        [Display(Name = "Preço")]
        public double Preco { get; set; }
        
        [Display(Name = "Imagem do Produto")]
        public string? Img { get; set; }
        
        [Required(ErrorMessage = "A nota é obrigatória")]
        [Range(1, 5, ErrorMessage = "A nota deve estar entre 1 e 5")]
        [Display(Name = "Nota")]
        public double Nota { get; set; }

        // Relacionamentos
        [Required(ErrorMessage = "A categoria é obrigatória")]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        [Display(Name = "Desconto")]
        public int? DescontoId { get; set; }
        public Desconto? Desconto { get; set; }

        public ICollection<Variacao> Variacoes { get; set; } = new List<Variacao>();
        public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
    }
}


