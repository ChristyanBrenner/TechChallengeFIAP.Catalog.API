using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Jogo> Jogo { get; set; }
        public DbSet<Pedido> Pedido { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
