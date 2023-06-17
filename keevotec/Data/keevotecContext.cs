using keevotec.Models;
using Microsoft.EntityFrameworkCore;

namespace keevotec.Data;

public class KeevotecContext : DbContext
{
    public KeevotecContext(DbContextOptions<KeevotecContext> options)
	: base(options)
    {
    }
    public DbSet<Produto> Produtos { get; set; } = null!;
    public DbSet<Movimento> Movimentos { get; set; } = null!;
}
