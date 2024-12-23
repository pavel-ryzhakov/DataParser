using Microsoft.EntityFrameworkCore;

namespace DataParser;

public class PassportsDbContext : DbContext
{ 
    public PassportsDbContext(DbContextOptions<PassportsDbContext> options) : base(options)
    {
    }

    public DbSet<PassportSeries> PassportSeries { get; set; }
    public DbSet<PassportNumber> PassportNumbers { get; set; }
    public DbSet<Changelog> Changelogs { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PassportsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}