
using SCLI.Core;
using MP1.AssetTracker.DataLayer;

namespace MP1.AssetTracker
{
    public class Program
    {
        //public static string ConnectionString = "Server = (localdb)\\MSSQLLocalDB; Database = EfBloggy; Integrated Security = True";
        static void Main(string[] args)
        {
            SCLIMain ui = new SCLIMain();

            AssetTrackerDbContext database = new AssetTrackerDbContext();
            AssetTrackerUIContext c = new AssetTrackerUIContext(ui, ui, new AssetRepository(database), new OfficeRepository(database));

            c.AddCommand("list", c.ListAllAssetsCommand);
            c.AddCommand("add", c.AddAssetCommand);
            ui.PushContext(c);

            ui.PutMessage("Welcome to the AssetTracker miniproject. Type 'help' to see available commands.");
            ui.Run();
        }       
    }
}
