using Faturamento.Api.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Faturamento.Api.Infra
{
    public sealed class EFContexto : DbContext
    {
        public EFContexto(DbContextOptions<EFContexto> configuracao)
            : base(configuracao)
        { }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PedidoMap());
            modelBuilder.ApplyConfiguration(new PagamentoMap());
        }
    }

    public sealed class PedidoMap : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedidos");
            builder.HasKey("Id");
            builder
                .Property(p => p.Status)
                .HasConversion(new EnumToStringConverter<Pedido.EStatus>());
            builder
                 .HasMany(c => c.Itens)
                 .WithOne()
                 .HasForeignKey("PedidoId")
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public sealed class PedidoItemMap : IEntityTypeConfiguration<Pedido.Item>
    {
        public void Configure(EntityTypeBuilder<Pedido.Item> builder)
        {
            builder.ToTable("PedidoItens");
            builder.HasKey(p => p.Id);
        }
    }

    public sealed class PagamentoMap : IEntityTypeConfiguration<Pagamento>
    {
        public void Configure(EntityTypeBuilder<Pagamento> builder)
        {
            builder.ToTable("Pagamentos");
            builder.HasKey("Id");
            builder
                .Property(p => p.Status)
                .HasConversion(new EnumToStringConverter<Pagamento.EStatus>());
            builder
                .HasOne(typeof(Pedido))
                .WithMany()
                .HasForeignKey("PedidoId");
        }
    }
}
