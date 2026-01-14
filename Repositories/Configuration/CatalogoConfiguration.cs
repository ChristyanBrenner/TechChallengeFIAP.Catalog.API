using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories
{
    public class CatalogoConfiguration : IEntityTypeConfiguration<Jogo>
    {
        public void Configure(EntityTypeBuilder<Jogo> builder)
        {
            builder.ToTable("Jogo");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(p => p.DataCriacao).HasColumnType("DATETIME").IsRequired();
            builder.Property(p => p.Nome).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.Genero).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.Preco).HasColumnType("DECIMAL(18,2)").IsRequired();
            builder.Property(p => p.NomeNormalizado).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.GeneroNormalizado).HasColumnType("VARCHAR(100)").IsRequired();
            builder.HasIndex(p => new { p.NomeNormalizado, p.GeneroNormalizado }).IsUnique().HasDatabaseName("UX_Jogo_Nome_Genero");
        }
    }
}
