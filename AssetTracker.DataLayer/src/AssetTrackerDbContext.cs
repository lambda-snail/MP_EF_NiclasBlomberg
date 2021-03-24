using Microsoft.EntityFrameworkCore;

using MP1.AssetTracker.Model;
using System;

namespace MP1.AssetTracker.DataLayer
{
    public class AssetTrackerDbContext : DbContext
    {
        public string ConnectionString { get; }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Office> Offices { get; set; }

        public AssetTrackerDbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public AssetTrackerDbContext() : base()
        {
            ConnectionString = "Server = (localdb)\\MSSQLLocalDB; Database = AssetTrackerDB; Integrated Security = True";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
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

            // Must ignore as EFC cannot handle CultureInfo
            modelBuilder.Entity<Office>().Ignore(o => o.OfficeLocalCulture);

            // Needed to make the abstract class Asset work with EFC
            modelBuilder.Entity<Computer>().HasBaseType(typeof(Asset)).HasDiscriminator<string>("AssetType").HasValue("Computer");
            modelBuilder.Entity<Cellphone>().HasBaseType(typeof(Asset)).HasDiscriminator<string>("AssetType").HasValue("Cellphone");
            modelBuilder.Entity<Asset>().HasBaseType(null as Type);
        }
    }
}
