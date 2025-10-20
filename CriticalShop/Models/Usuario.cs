using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CriticalShop.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(14, ErrorMessage = "CPF deve ter 11 dígitos")]
        [Display(Name = "CPF")]
        public string Cpf { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;
        
        [Display(Name = "Endereço")]
        public string? Endereco { get; set; }
        
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }
        
        [Display(Name = "Data de Nascimento")]
        public DateTime? DataNascimento { get; set; }
        
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public bool Ativo { get; set; } = true;
        
        // Relacionamentos
        public ICollection<Carrinho>? Carrinhos { get; set; }
        public ICollection<Avaliacao>? Avaliacoes { get; set; }
    }
}