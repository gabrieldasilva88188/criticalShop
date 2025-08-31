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
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento Produto - Categoria
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento Produto - Desconto
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Desconto)
                .WithMany()
                .HasForeignKey(p => p.DescontoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relacionamento Produto - Variacao (Cascade Delete)
            modelBuilder.Entity<Variacao>()
                .HasOne(v => v.Produto)
                .WithMany(p => p.Variacoes)
                .HasForeignKey(v => v.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento Avaliação - Produto (Cascade Delete)
            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Produto)
                .WithMany(p => p.Avaliacoes)
                .HasForeignKey(a => a.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento Avaliação - Usuario
            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurações adicionais para Produto
            modelBuilder.Entity<Produto>()
                .Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Preco)
                .HasColumnType("decimal(18,2)");


            // Configurações para Categoria
            modelBuilder.Entity<Categoria>()
                .Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(50);

            base.OnModelCreating(modelBuilder);
        }
    }
}
