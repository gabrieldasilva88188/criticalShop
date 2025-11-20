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
        
        [Display(Name = "Descrição Detalhada")]
        [StringLength(5000, ErrorMessage = "A descrição não pode exceder 5000 caracteres")]
        public string? Descricao { get; set; }
        
        [Display(Name = "Material")]
        [StringLength(200, ErrorMessage = "O material não pode exceder 200 caracteres")]
        public string? Material { get; set; }
        
        [Display(Name = "Quantidade/Conteúdo")]
        [StringLength(100, ErrorMessage = "O conteúdo não pode exceder 100 caracteres")]
        public string? Conteudo { get; set; }
        
        // Informações para cálculo de frete (opcionais)
        [Display(Name = "Peso (kg)")]
        [Range(0, 30, ErrorMessage = "O peso deve estar entre 0 e 30 kg")]
        public double? Peso { get; set; }
        
        [Display(Name = "Altura (cm)")]
        [Range(0, 105, ErrorMessage = "A altura deve estar entre 0 e 105 cm")]
        public int? Altura { get; set; }
        
        [Display(Name = "Largura (cm)")]
        [Range(0, 105, ErrorMessage = "A largura deve estar entre 0 e 105 cm")]
        public int? Largura { get; set; }
        
        [Display(Name = "Comprimento (cm)")]
        [Range(0, 105, ErrorMessage = "O comprimento deve estar entre 0 e 105 cm")]
        public int? Comprimento { get; set; }

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


