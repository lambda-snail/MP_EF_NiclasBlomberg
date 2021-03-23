using Microsoft.EntityFrameworkCore;

using MP1.AssetTracker.Model;

namespace MP1.AssetTracker.DataLayer
{
    public class AssetTrackerDbContext : DbContext
    {
        public string ConnectionString { get; }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Office> Offices { get; set; }


        public AssetTrackerDbContext(string connectionString) : base()
        {
            ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
              "Server = (localdb)\\MSSQLLocalDB; Database = EfBloggy; Integrated Security = True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Office>(
                entityBuilder =>
                    {
                        entityBuilder.Property(o => o.Location).HasColumnType("nvarchar(128)");
                        entityBuilder.Property(o => o.Culture).HasColumnType("nvarchar(16)"); // Waste a few bytes just to be safe - still better than the default nvarchar(max)
                    }
                );

            //modelBuilder.Entity<Asset>(
            //    entityBuilder =>
        }
    }
}
