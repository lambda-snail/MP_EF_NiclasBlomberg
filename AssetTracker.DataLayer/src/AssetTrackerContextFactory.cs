
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MPEF.AssetTracker.DataLayer
{
    /// <summary>
    /// Responsible for creating the DbContext. This class is neccessary since the
    /// unit tests and EF Core require different constructors to create the DbContext.
    /// </summary>
    public class AssetTrackerContextFactory : IDesignTimeDbContextFactory<AssetTrackerDbContext>
    {
        public string ConnectionString = "Server = (localdb)\\MSSQLLocalDB; Database = AssetTrackerDB; Integrated Security = True";
        public AssetTrackerDbContext CreateDbContext(string[] args)
        {
            return new AssetTrackerDbContext(ConnectionString);
        }
    }
}
