using EagleBankApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EagleBankApi.Infrastructure.Data;

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

        //generated code
        modelBuilder.Entity<Account>(entity =>
        {
            // Primary Key
            entity.HasKey(e => e.AccountNumber);

            // AccountNumber configuration
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(8) // Matches regex pattern length ^01\d{6}$
                .IsFixedLength() // Optimize for fixed-length account numbers
                .IsRequired();

            // SortCode configuration
            entity.Property(e => e.SortCode)
                .HasMaxLength(8) // "10-10-10" is 8 chars
                .HasDefaultValue("10-10-10")
                .IsRequired();

            // Name configuration
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();

            // AccountType configuration (enum as string)
            entity.Property(e => e.AccountType)
                .HasConversion<string>()
                .HasMaxLength(20) // Longest enum value length
                .IsRequired();

            // Balance configuration
            entity.Property(e => e.Balance)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m)
                .IsRequired();

            // Currency configuration
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("GBP")
                .IsRequired();

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

        // If using SQLite, add this to ensure proper column types
        if (Database.IsSqlite())
        {
            modelBuilder.Entity<Account>()
                .Property(e => e.Balance)
                .HasConversion<double>(); // SQLite doesn't support decimal natively
        }

        modelBuilder.Entity<Transaction>(entity =>
        {
            // Primary Key
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id)
                .HasMaxLength(64) // Matches User.Id configuration
                .IsRequired();

            // Required fields with constraints
            entity.Property(t => t.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)"); // Ensures 2 decimal places for currency

            entity.Property(t => t.Currency)
                .IsRequired()
                .HasMaxLength(3); // "GBP" is 3 chars

            entity.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(10); // "deposit" (7) or "withdrawal" (9)

            entity.Property(t => t.Reference)
                .HasMaxLength(100); // Optional, but limit length if provided

            // Foreign Key relationships (if needed)
            entity.Property(t => t.UserId)
                .IsRequired()
                .HasMaxLength(64); // Matches User.Id length

            entity.Property(t => t.AccountNumber)
                .IsRequired()
                .HasMaxLength(8) // Matches ^01\d{6}$ (8 chars total)
                .IsFixedLength(); // Ensures exact length (no padding)

            // Timestamps
            entity.Property(t => t.CreatedTimestamp)
                .IsRequired()
                .HasColumnType("datetime2"); // Precise timestamp

            entity.HasIndex(t => t.AccountNumber);
            entity.HasIndex(t => t.UserId);
        });
    }
}
