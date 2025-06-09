using EagleBankApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EagleBankApi.Data;

public sealed class EagleBankDbContext(DbContextOptions<EagleBankDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasMaxLength(64);

            entity.OwnsOne(u => u.Address, a =>
            {
                a.Property(p => p.Line1).IsRequired().HasMaxLength(100);
                a.Property(p => p.Town).IsRequired().HasMaxLength(50);
                a.Property(p => p.County).IsRequired().HasMaxLength(50);
                a.Property(p => p.Postcode).IsRequired().HasMaxLength(20);
            });

            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
        });
    }
}
