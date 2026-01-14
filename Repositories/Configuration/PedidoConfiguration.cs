using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.Configuration
{
    public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedido");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(p => p.DataCriacao).HasColumnType("DATETIME").IsRequired();
            builder.Property(p => p.DataCompra).HasColumnType("DATETIME").IsRequired();
            builder.Property(p => p.UsuarioId).HasColumnType("INT").IsRequired();
            builder.Property(p => p.JogoId).HasColumnType("INT").IsRequired();
            builder.Property(p => p.NomeJogo).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.PrecoPago).HasColumnType("DECIMAL(18,2)").IsRequired();
        }
    }
}
