
using SCLI.Core;
using MPEF.AssetTracker.DataLayer;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

using MPEF.AssetTracker.Model;
using MPEF.AssetTracker.Main.UIControllers;

namespace MPEF.AssetTracker
{
    public class Program
    {
        private static AssetTrackerDbContext _database;

        //public static string ConnectionString = "Server = (localdb)\\MSSQLLocalDB; Database = AssetTracker.DB; Integrated Security = True";
        static void Main(string[] args)
        {
            // EF Core stuff
            //_database = new AssetTrackerDbContext();
            var factory = new AssetTrackerContextFactory();

            _database = factory.CreateDbContext(null);
            _database.Database.EnsureCreated();
            LoadMockData();

            // UI stuff
            SCLIMain ui = new SCLIMain();

            AssetTrackerMainMenu main = new AssetTrackerMainMenu(ui,ui, new AssetRepository(_database), new OfficeRepository(_database));
            ui.PushContext(main);

            ui.PutMessage("Welcome to the AssetTracker miniproject. Type 'help' to see available commands.");
            ui.Run();
        }

        public static void LoadMockData()
        {
            if (_database.Assets.Count() == 0)
            {
                string jsonComputers = System.IO.File.ReadAllText(System.IO.Path.Join("DevelopmentResources", "MOCK_DATA_COMPUTER.json"));
                string jsonCellphones = System.IO.File.ReadAllText(System.IO.Path.Join("DevelopmentResources", "MOCK_DATA_CELLPHONE.json"));

                List<Computer> computerList = JsonSerializer.Deserialize<List<Computer>>(jsonComputers);
                List<Cellphone> cellphoneList = JsonSerializer.Deserialize<List<Cellphone>>(jsonCellphones);

                _database.AddRange(computerList);
                _database.AddRange(cellphoneList);
            }

            if (_database.Offices.Count() == 0)
            {
                _database.Add(new Office("Japan", "ja-JP"));
                _database.Add(new Office("Sweden", "se-SE"));
                _database.Add(new Office("France", "fr-FR"));
            }

            _database.SaveChanges();
        }
    }
}
