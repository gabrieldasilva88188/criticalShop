using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CriticalShop.Models
{
    public class Avaliacao
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public int ProdutoId { get; set; }
        
        [Required(ErrorMessage = "A nota é obrigatória")]
        [Range(1, 5, ErrorMessage = "A nota deve estar entre 1 e 5")]
        [Display(Name = "Nota")]
        public int Nota { get; set; }
        
        public string Descricao { get; set; } = string.Empty;
        public DateTime Data { get; set; } = DateTime.Now;

        public Usuario Usuario { get; set; } = null!;
        public Produto Produto { get; set; } = null!;
    }
}