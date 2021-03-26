using MPEF.AssetTracker.Model;
using SCLI.Core;
using System;
using System.Linq;

namespace MPEF.AssetTracker.Main.UIControllers
{
    class ReportGenerationCommand : AssetTrackerCommandBase
    {
        public ReportGenerationCommand(IConsoleOutput outputHandle, IUserInput inputHandle, IAssetRepository assetRepo, IOfficeRepository officeRepo)
            : base(outputHandle, inputHandle, assetRepo, officeRepo) { }

        /// <summary>
        /// Generates some basic reports about the system.
        /// <returns></returns>
        public bool GenerateReport(string cmdName, string[] cmdArgs)
        {
            int assetCount = Assets.GetAssets().Count();
            
            int numExpiredAssets = Assets.GetAssets()
                                   .Where(a => DateTime.Now < a.ExpiryDate)
                                   .Select(a => a)
                                   .Count();

            int numAssetsOlderThanFiveYears = Assets.GetAssets()
                                              .Where(a => DateTime.Now.Subtract(TimeSpan.FromDays(365 * 5)) < a.PurchaseDate)
                                              .Select(a => a)
                                              .Count();

            OutputHandle.PutMessage("Statistics:");
            OutputHandle.PutMessage($"A total of {assetCount} assets are being tracked.");
            OutputHandle.PutMessage($"{assetCount} assets are past their exiry date.");
            OutputHandle.PutMessage($"{numAssetsOlderThanFiveYears} assets are olde than 5 years.");

            return true;
        }

    } 
}
