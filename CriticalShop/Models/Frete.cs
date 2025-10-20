using System.ComponentModel.DataAnnotations;

namespace CriticalShop.Models
{
    public class FreteOpcao
    {
        public string Servico { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public double Valor { get; set; }
        public int PrazoEntrega { get; set; }
        public string Observacao { get; set; } = string.Empty;
    }

    public class FreteRequest
    {
        [Required(ErrorMessage = "O CEP de destino é obrigatório")]
        [StringLength(9, MinimumLength = 8, ErrorMessage = "CEP inválido")]
        public string CepDestino { get; set; } = string.Empty;
    }

    public class FreteResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public List<FreteOpcao> Opcoes { get; set; } = new List<FreteOpcao>();
        public string CepDestino { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }

    public class EnderecoViaCep
    {
        public string Cep { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Localidade { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public string Ibge { get; set; } = string.Empty;
        public string Gia { get; set; } = string.Empty;
        public string Ddd { get; set; } = string.Empty;
        public string Siafi { get; set; } = string.Empty;
        public bool Erro { get; set; }
    }
}
