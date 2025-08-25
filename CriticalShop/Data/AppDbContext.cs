using Microsoft.EntityFrameworkCore;
using CriticalShop.Models;

namespace CriticalShop.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Variacao> Variacoes { get; set; }
        public DbSet<Desconto> Descontos { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relacionamento Categoria Pai/Filho
            modelBuilder.Entity<Categoria>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.SubCategorias)
                .HasForeignKey(c => c.ParentId);

            // Relacionamento Produto - Categoria
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.CategoriaId);

            // Relacionamento Produto - Variacao
            modelBuilder.Entity<Variacao>()
                .HasOne(v => v.Produto)
                .WithMany(p => p.Variacoes)
                .HasForeignKey(v => v.ProdutoId);

            // Relacionamento Avaliação - Produto
            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Produto)
                .WithMany(p => p.Avaliacoes)
                .HasForeignKey(a => a.ProdutoId);

            // Relacionamento Avaliação - Usuario
            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
