using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var user = modelBuilder.Entity<User>();

        user.ToTable("Users");

        // Primary Key
        user.HasKey(x => x.Id);

        // Name
        user.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Age
        user.Property(x => x.Age)
            .IsRequired();

        // City
        user.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(100);

        // State
        user.Property(x => x.State)
            .IsRequired()
            .HasMaxLength(100);

        // Pincode
        user.Property(x => x.Pincode)
            .IsRequired()
            .HasMaxLength(10);

        // Created date
        user.Property(x => x.CreatedAtUtc)
            .IsRequired();
    }
}
