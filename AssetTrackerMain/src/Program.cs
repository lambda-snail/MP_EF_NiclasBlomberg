
using SCLI.Core;

namespace MP1.AssetTracker
{
    public class Program
    {
        static void Main(string[] args)
        {
            SCLIMain ui = new SCLIMain();
            AssetTrackerUIContext c = new AssetTrackerUIContext(ui, ui, new AssetRepository(), new OfficeRepository());

            c.AddCommand("list", c.ListAllAssetsCommand);
            c.AddCommand("add", c.AddAssetCommand);
            ui.PushContext(c);

            ui.PutMessage("Welcome to the AssetTracker miniproject. Type 'help' to see available commands.");
            ui.Run();
        }       
    }
}
