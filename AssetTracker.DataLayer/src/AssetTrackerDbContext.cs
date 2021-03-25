using Microsoft.EntityFrameworkCore;

using MP1.AssetTracker.Model;
using System;

namespace MP1.AssetTracker.DataLayer
{
    public class AssetTrackerDbContext : DbContext
    {
        public string ConnectionString { get; private set; }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Office> Offices { get; set; }

        public AssetTrackerDbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public AssetTrackerDbContext() : base() {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ConnectionString = "Server = (localdb)\\MSSQLLocalDB; Database = AssetTrackerDB; Integrated Security = True";
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Custom settings to avoid nvarchar(max) 
            modelBuilder.Entity<Office>(
                entityBuilder =>
                    {
                        entityBuilder.Property(o => o.Location).HasColumnType("nvarchar(128)").IsRequired();
                        entityBuilder.Property(o => o.Culture).HasColumnType("nvarchar(16)").IsRequired();
                    }
                );

            modelBuilder.Entity<Asset>(
                entityBuilder =>
                    {
                        entityBuilder.Property(a => a.ModelName).HasColumnType("nvarchar(128)").IsRequired();
                        entityBuilder.Property(a => a.PurchaseDate).HasColumnType("datetime").IsRequired();
                        entityBuilder.Property(a => a.ExpiryDate).HasColumnType("datetime").IsRequired();
                    }
                );

            modelBuilder.Entity<Computer>(
                entityBuilder =>
                    {
                        entityBuilder.Property(c => c.OperatingSystem).HasColumnType("nvarchar(128)").IsRequired();
                        entityBuilder.Property(c => c.RAM).HasColumnType("nvarchar(128)").IsRequired();
                        entityBuilder.Property(c => c.Processor).HasColumnType("nvarchar(128)").IsRequired();
                    }
                );

            modelBuilder.Entity<Cellphone>(
                entityBuilder =>
                    {
                        entityBuilder.Property(c => c.PhoneOperator).HasColumnType("nvarchar(128)").IsRequired();
                        entityBuilder.Property(c => c.PhoneNumber).HasColumnType("nvarchar(32)").IsRequired();
                    }
                );

            // Must ignore as EFC cannot handle CultureInfo
            modelBuilder.Entity<Office>().Ignore(o => o.OfficeLocalCulture);

            // Needed to make the abstract class Asset work with EFC
            modelBuilder.Entity<Computer>().HasBaseType(typeof(Asset)).HasDiscriminator<string>("AssetType").HasValue("Computer");
            modelBuilder.Entity<Cellphone>().HasBaseType(typeof(Asset)).HasDiscriminator<string>("AssetType").HasValue("Cellphone");
            modelBuilder.Entity<Asset>().HasBaseType(null as Type);
        }
    }
}
