using MPEF.AssetTracker.Model;
using SCLI.Core;
using System;
using System.Linq;

namespace MPEF.AssetTracker.Main.UIControllers
{
    public class DeleteAssetsCommand : AssetTrackerCommandBase
    {
        public DeleteAssetsCommand(IConsoleOutput outputHandle, IUserInput inputHandle, IAssetRepository assetRepo, IOfficeRepository officeRepo)
            : base(outputHandle, inputHandle, assetRepo, officeRepo) { }

        public bool DeleteAssetCommand(string cmdName, string[] cmdArgs)
        {
            OutputHandle.PutMessage("Enter the id of the asset you wish to delete.");

            int id = 0;
            while (!int.TryParse(InputHandle.GetEditableInputWithDefaultText(), out id))
            {
                OutputHandle.PutMessage("Please enter a valid number.", IConsoleOutput.Color.YELLOW);
            }

            if (Assets.GetAsset(id) == null)
            {
                OutputHandle.PutMessage("The provided id does not exist in the system.", IConsoleOutput.Color.YELLOW);
                return false;
            }

            Assets.DeleteAsset(id);

            return true;
        }
    }
}
