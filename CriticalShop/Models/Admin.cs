using System.ComponentModel.DataAnnotations;

namespace CriticalShop.Models
{
    public class Admin
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;
        
        [Display(Name = "Nome")]
        public string? Nome { get; set; }
        
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public bool Ativo { get; set; } = true;
        public bool IsSuperAdmin { get; set; } = false;
    }
}
