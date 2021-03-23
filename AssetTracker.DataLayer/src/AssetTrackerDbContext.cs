using Microsoft.EntityFrameworkCore;

using MP1.AssetTracker.Model;
using System;

namespace MP1.AssetTracker.DataLayer
{
    public class AssetTrackerDbContext : DbContext
    {
        //public string ConnectionString { get; }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Office> Offices { get; set; }


        public AssetTrackerDbContext() : base()
        {
            //ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server = (localdb)\\MSSQLLocalDB; Database = EfBloggy; Integrated Security = True"
                );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Office>(
                entityBuilder =>
                    {
                        entityBuilder.Property(o => o.Location).HasColumnType("nvarchar(128)").IsRequired();
                        entityBuilder.Property(o => o.Culture).HasColumnType("nvarchar(16)").IsRequired(); // Waste a few bytes just to be safe - still better than the default nvarchar(max)
                    }
                );

            modelBuilder.Entity<Office>().Ignore(o => o.OfficeLocalCulture); // Must ignore as EFC cannot handle CultureInfo

            //modelBuilder.Entity<Asset>().HasDiscriminator<string>("AssetType").HasValue("");
            modelBuilder.Entity<Computer>().HasBaseType(typeof(Asset)).HasDiscriminator<string>("AssetType").HasValue("Computer");
            modelBuilder.Entity<Cellphone>().HasBaseType(typeof(Asset)).HasDiscriminator<string>("AssetType").HasValue("Cellphone");
            modelBuilder.Entity<Asset>().HasBaseType(null as Type);
        }
    }
}
