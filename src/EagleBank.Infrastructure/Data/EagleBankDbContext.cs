using EagleBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EagleBank.Infrastructure.Data;

public sealed class EagleBankDbContext(DbContextOptions<EagleBankDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

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

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(a => a.AccountNumber);
            entity.Property(a => a.AccountNumber).HasMaxLength(8);
            entity.Property(a => a.SortCode).IsRequired().HasMaxLength(8);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.AccountType).IsRequired().HasConversion<string>();
            entity.Property(a => a.Currency).IsRequired().HasMaxLength(3);
            entity.Property(a => a.UserId).IsRequired().HasMaxLength(64).HasMaxLength(20);
            entity.Property(e => e.Balance).IsRequired().HasColumnType("decimal(18,2)");

            // Timestamps configuration
            entity.Property(e => e.CreatedTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP") // SQL Server syntax
                .ValueGeneratedOnAdd()
                .IsRequired();

            entity.Property(e => e.UpdatedTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate()
                .IsRequired();

            // UserId configuration
            entity.Property(e => e.UserId)
                .HasMaxLength(64) // usr- + alphanumeric (adjust as needed)
                .IsRequired();

            // Configure the User relationship to be query-only
            entity.HasOne(a => a.User)
                .WithMany() // No inverse navigation
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); // Makes the relationship optional for saves

            // Explicitly ignore User property during changes
            entity.Navigation(a => a.User)
                .AutoInclude(false) // Don't auto-include in queries
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            // Add indexes for performance
            entity.HasIndex(e => e.UserId);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).HasMaxLength(64);
            entity.Property(t => t.Amount).HasPrecision(18, 2);
            entity.Property(t => t.Currency).IsRequired().HasMaxLength(3);
            entity.Property(t => t.Type).IsRequired().HasMaxLength(20);
            entity.Property(t => t.Reference).HasMaxLength(100);
            entity.Property(t => t.UserId).IsRequired().HasMaxLength(64);
            entity.Property(t => t.AccountNumber).IsRequired().HasMaxLength(8);
            entity.Property(t => t.CreatedTimestamp)
                .IsRequired()
                .HasColumnType("datetime2");

            entity.HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.AccountNumber);

            // Configure the User relationship to be query-only
            entity.HasOne(a => a.Account)
                .WithMany() // No inverse navigation
                .HasForeignKey(a => a.AccountNumber)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); // Makes the relationship optional for saves

            // Explicitly ignore User property during changes
            entity.Navigation(a => a.Account)
                .AutoInclude(false) // Don't auto-include in queries
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            // Add indexes for performance
            entity.HasIndex(e => e.UserId);
        });
    }
}
