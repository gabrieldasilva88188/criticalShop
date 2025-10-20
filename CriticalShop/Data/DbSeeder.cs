using CriticalShop.Models;

namespace CriticalShop.Data
{
    public static class DbSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            // Verificar se já existem categorias
            if (!context.Categorias.Any())
            {
                var categorias = new List<Categoria>
                {
                    new Categoria { Nome = "Jogos de Tabuleiro" },
                    new Categoria { Nome = "RPG" },
                    new Categoria { Nome = "Miniaturas" },
                    new Categoria { Nome = "Dados" },
                    new Categoria { Nome = "Livros" },
                    new Categoria { Nome = "Acessórios" }
                };

                context.Categorias.AddRange(categorias);
                context.SaveChanges();
            }

            // Verificar se já existem descontos
            if (!context.Descontos.Any())
            {
                var descontos = new List<Desconto>
                {
                    new Desconto { Valor = 10 },
                    new Desconto { Valor = 15 },
                    new Desconto { Valor = 20 },
                    new Desconto { Valor = 25 },
                    new Desconto { Valor = 30 }
                };

                context.Descontos.AddRange(descontos);
                context.SaveChanges();
            }

            // Verificar se já existe o admin padrão
            if (!context.Admins.Any())
            {
                var adminPadrao = new Admin
                {
                    Email = "admin",
                    Senha = "admin123", // Em produção, isso deveria ser hasheado
                    Nome = "Administrador",
                    IsSuperAdmin = true,
                    Ativo = true,
                    DataCadastro = DateTime.Now
                };

                context.Admins.Add(adminPadrao);
                context.SaveChanges();
            }
        }
    }
}
