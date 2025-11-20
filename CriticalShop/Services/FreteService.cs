using CriticalShop.Models;
using System.Text.Json;

namespace CriticalShop.Services
{
    public class FreteService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<FreteService> _logger;
        private readonly IConfiguration _configuration;

        // CEP de origem (seu endereço/armazém)
        private const string CEP_ORIGEM = "01310-100"; // Exemplo: Av. Paulista, SP

        public FreteService(IHttpClientFactory httpClientFactory, ILogger<FreteService> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Calcula o frete para um CEP de destino baseado nos produtos do carrinho
        /// </summary>
        public async Task<FreteResponse> CalcularFreteAsync(string cepDestino, List<ItemCarrinho> itens)
        {
            try
            {
                // Validar e buscar informações do CEP
                var endereco = await BuscarCepAsync(cepDestino);
                if (endereco == null || endereco.Erro)
                {
                    return new FreteResponse
                    {
                        Sucesso = false,
                        Mensagem = "CEP não encontrado ou inválido"
                    };
                }

                // Calcular peso e dimensões totais
                var pesoTotal = itens.Sum(i => (i.Produto?.Peso ?? 0.5) * i.Quantidade);
                var volumeTotal = CalcularVolume(itens);

                // Calcular distância aproximada (simplificado por estado)
                var distancia = CalcularDistanciaPorEstado(endereco.Uf);

                // Calcular opções de frete
                var opcoes = new List<FreteOpcao>();

                // PAC (Mais econômico)
                var valorPac = CalcularValorFrete(pesoTotal, distancia, "PAC");
                var prazoPac = CalcularPrazoEntrega(distancia, "PAC");
                opcoes.Add(new FreteOpcao
                {
                    Servico = "PAC",
                    Nome = "PAC - Correios",
                    Valor = valorPac,
                    PrazoEntrega = prazoPac,
                    Observacao = "Entrega econômica"
                });

                // SEDEX (Mais rápido)
                var valorSedex = CalcularValorFrete(pesoTotal, distancia, "SEDEX");
                var prazoSedex = CalcularPrazoEntrega(distancia, "SEDEX");
                opcoes.Add(new FreteOpcao
                {
                    Servico = "SEDEX",
                    Nome = "SEDEX - Correios",
                    Valor = valorSedex,
                    PrazoEntrega = prazoSedex,
                    Observacao = "Entrega rápida"
                });

                // Frete Grátis (se valor total for acima de R$ 200)
                var valorTotal = itens.Sum(i => i.PrecoFinal * i.Quantidade);
                if (valorTotal >= 200)
                {
                    opcoes.Add(new FreteOpcao
                    {
                        Servico = "GRATIS",
                        Nome = "Frete Grátis",
                        Valor = 0,
                        PrazoEntrega = prazoPac + 2,
                        Observacao = "Compras acima de R$ 200,00"
                    });
                }

                return new FreteResponse
                {
                    Sucesso = true,
                    Mensagem = "Frete calculado com sucesso",
                    Opcoes = opcoes,
                    CepDestino = cepDestino,
                    Cidade = endereco.Localidade,
                    Estado = endereco.Uf
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular frete");
                return new FreteResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro ao calcular frete. Tente novamente."
                };
            }
        }

        /// <summary>
        /// Busca informações de endereço pelo CEP usando ViaCEP
        /// </summary>
        private async Task<EnderecoViaCep?> BuscarCepAsync(string cep)
        {
            try
            {
                // Remover formatação do CEP
                cep = cep.Replace("-", "").Replace(".", "").Trim();

                if (cep.Length != 8)
                {
                    return null;
                }

                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://viacep.com.br/ws/{cep}/json/");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var endereco = JsonSerializer.Deserialize<EnderecoViaCep>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return endereco;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar CEP");
                return null;
            }
        }

        /// <summary>
        /// Calcula volume total dos produtos
        /// </summary>
        private double CalcularVolume(List<ItemCarrinho> itens)
        {
            double volumeTotal = 0;
            foreach (var item in itens)
            {
                if (item.Produto != null)
                {
                    // Usar valores padrão se dimensões não estiverem definidas
                    var altura = item.Produto.Altura ?? 10;
                    var largura = item.Produto.Largura ?? 15;
                    var comprimento = item.Produto.Comprimento ?? 20;
                    
                    var volume = (altura * largura * comprimento) / 1000000.0; // m³
                    volumeTotal += volume * item.Quantidade;
                }
            }
            return volumeTotal;
        }

        /// <summary>
        /// Calcula distância aproximada baseada no estado
        /// </summary>
        private int CalcularDistanciaPorEstado(string uf)
        {
            // Distâncias aproximadas de São Paulo (em km)
            var distancias = new Dictionary<string, int>
            {
                { "SP", 50 },
                { "RJ", 430 },
                { "MG", 580 },
                { "ES", 880 },
                { "PR", 410 },
                { "SC", 710 },
                { "RS", 1130 },
                { "MS", 1010 },
                { "MT", 1690 },
                { "GO", 920 },
                { "DF", 1010 },
                { "BA", 1960 },
                { "SE", 2280 },
                { "AL", 2490 },
                { "PE", 2660 },
                { "PB", 2850 },
                { "RN", 3010 },
                { "CE", 3130 },
                { "PI", 2980 },
                { "MA", 3070 },
                { "TO", 1640 },
                { "PA", 3250 },
                { "AP", 3980 },
                { "AM", 3900 },
                { "RR", 4500 },
                { "RO", 3400 },
                { "AC", 3700 }
            };

            return distancias.ContainsKey(uf.ToUpper()) ? distancias[uf.ToUpper()] : 1000;
        }

        /// <summary>
        /// Calcula valor do frete baseado em peso, distância e tipo de serviço
        /// </summary>
        private double CalcularValorFrete(double peso, int distancia, string servico)
        {
            // Valores base
            double valorBase = servico == "SEDEX" ? 25.0 : 15.0;
            
            // Adicionar por peso (R$ 5 por kg adicional após 1kg)
            double adicionalPeso = peso > 1 ? (peso - 1) * 5.0 : 0;
            
            // Adicionar por distância (R$ 0.01 por km)
            double adicionalDistancia = distancia * 0.01;
            
            // Multiplicador do serviço
            double multiplicador = servico == "SEDEX" ? 1.8 : 1.0;
            
            var valorTotal = (valorBase + adicionalPeso + adicionalDistancia) * multiplicador;
            
            // Arredondar para 2 casas decimais
            return Math.Round(valorTotal, 2);
        }

        /// <summary>
        /// Calcula prazo de entrega baseado em distância e tipo de serviço
        /// </summary>
        private int CalcularPrazoEntrega(int distancia, string servico)
        {
            // Prazo base em dias úteis
            int prazoBase = servico == "SEDEX" ? 2 : 5;
            
            // Adicionar dias por faixa de distância
            int diasAdicionais = 0;
            if (distancia > 500) diasAdicionais += 2;
            if (distancia > 1000) diasAdicionais += 2;
            if (distancia > 2000) diasAdicionais += 3;
            if (distancia > 3000) diasAdicionais += 3;
            
            return prazoBase + diasAdicionais;
        }

        /// <summary>
        /// Valida formato do CEP
        /// </summary>
        public bool ValidarCep(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return false;

            cep = cep.Replace("-", "").Replace(".", "").Trim();
            return cep.Length == 8 && cep.All(char.IsDigit);
        }
    }
}
