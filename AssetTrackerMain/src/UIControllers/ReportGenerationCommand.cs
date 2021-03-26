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
            int assetCount = Assets.Count;

            int numExpiredAssets = Assets.GetAssets(a => a.ExpiryDate < DateTime.Now ).Count();

            int numAssetsOlderThanFiveYears = Assets.GetAssets(a => a.PurchaseDate.AddYears(5) < DateTime.Now).Count();

            int numAssetsExpireThreeMonths = Assets.GetAssets(a => a.ExpiryDate > DateTime.Now && a.ExpiryDate < DateTime.Now.AddMonths(3)).Count();

            int numAssetsExpireSixMonths = Assets.GetAssets(a =>
                                                !(a.ExpiryDate < DateTime.Now || a.ExpiryDate < DateTime.Now.AddMonths(3)) &&
                                                a.ExpiryDate < DateTime.Now.AddMonths(6)).Count();


            OutputHandle.PutMessage("Statistics:");
            OutputHandle.PutMessage($"A total of {assetCount} assets are being tracked.");
            OutputHandle.PutMessage($"{numExpiredAssets} assets are past their exiry date.");
            OutputHandle.PutMessage($"{numAssetsExpireThreeMonths} assets have expired or will expire within 3 months.");
            OutputHandle.PutMessage($"{numAssetsExpireSixMonths} assets will expire within 6 months.");
            OutputHandle.PutMessage($"{numAssetsOlderThanFiveYears} assets are older than 5 years.");

            return true;
        }

    } 
}
