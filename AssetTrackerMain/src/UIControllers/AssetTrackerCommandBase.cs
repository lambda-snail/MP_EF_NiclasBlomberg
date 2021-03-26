using MPEF.AssetTracker.Model;
using SCLI.Core;

namespace MPEF.AssetTracker.Main.UIControllers
{
    public class AssetTrackerCommandBase
    {
        protected IAssetRepository Assets { get; }
        protected IOfficeRepository Offices { get; }
        protected IConsoleOutput OutputHandle { get; set; }
        protected IUserInput InputHandle { get; set; }

        public AssetTrackerCommandBase(IConsoleOutput output, IUserInput inputHandle, IAssetRepository assetRepo, IOfficeRepository officeRepo)
        {
            Assets = assetRepo;
            Offices = officeRepo;
            OutputHandle = output;
            InputHandle = inputHandle;
        }
    }
}
