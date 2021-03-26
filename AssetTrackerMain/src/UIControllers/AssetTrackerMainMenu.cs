using System;
using System.Collections.Generic;
using System.Linq;

using SCLI.Core;
using MPEF.AssetTracker.Model;
using System.Reflection;

namespace MPEF.AssetTracker.Main.UIControllers
{
    /// <summary>
    /// This class is responsible for providing the context for the following commands:
    /// add  - to add a new asset to the system.
    /// list - to list all assets in the system.
    /// update - updates an asset
    /// delete - deletes an asset
    /// 
    /// These commands are implemented in their own classes to keep responsibilities separate.
    /// </summary>
    public class AssetTrackerMainMenu : Context
    {
        public IAssetRepository Assets { get; }
        public IOfficeRepository Offices { get; }

        // CRUD operations
        private AddAssetsCommand AddAssetCommand { get; set; }
        private ListAssetsCommand ListAssetsCommand { get; set; }
        private UpdateAssetsCommand UpdateAssetsCommand { get; set; }
        private DeleteAssetsCommand DeleteAssetsCommand { get; set; }

        // Other operations
        private ReportGenerationCommand ReportGenerator { get; set; }

        public AssetTrackerMainMenu(IConsoleOutput outputHandle, IUserInput inputHandle, IAssetRepository assetRepo, IOfficeRepository officeRepo)
            : base(outputHandle, inputHandle, "")
        {
            Assets = assetRepo;
            Offices = officeRepo;

            AddAssetCommand = new AddAssetsCommand(outputHandle, inputHandle, assetRepo, officeRepo);
            ListAssetsCommand = new ListAssetsCommand(outputHandle, inputHandle, assetRepo, officeRepo);
            UpdateAssetsCommand = new UpdateAssetsCommand(outputHandle, inputHandle, assetRepo, officeRepo);
            DeleteAssetsCommand = new DeleteAssetsCommand(outputHandle, inputHandle, assetRepo, officeRepo);

            ReportGenerator = new ReportGenerationCommand(outputHandle, inputHandle, assetRepo, officeRepo);

            AddCommand("add", AddAssetCommand.AddAssetCommand);
            AddCommand("list", ListAssetsCommand.ListAllAssetsCommand);
            AddCommand("update", UpdateAssetsCommand.UpdateAssetCommand);
            AddCommand("delete", DeleteAssetsCommand.DeleteAssetCommand);

            AddCommand("reports", ReportGenerator.GenerateReport);
        }
    }
}
