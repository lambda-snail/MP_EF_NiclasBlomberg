using MPEF.AssetTracker.Model;
using SCLI.Core;
using System;
using System.Linq;

namespace MPEF.AssetTracker.Main.UIControllers
{
    class ListAssetsCommand : AssetTrackerCommandBase
    {
        public ListAssetsCommand(IConsoleOutput outputHandle, IUserInput inputHandle, IAssetRepository assetRepo, IOfficeRepository officeRepo)
            : base(outputHandle, inputHandle, assetRepo, officeRepo) { }

        /// <summary>
        /// The 'list' command implementation.
        /// </summary>
        public bool ListAllAssetsCommand(string cmdName, string[] cmdArgs)
        {
            int count = Assets.GetAssets().Count();

            if (count == 0)
            {
                OutputHandle.PutMessage("No assets in database.", IConsoleOutput.Color.GREEN);
            }

            int pad = 16;
            int pricePad = pad;
            int idPad = pad / 3;
            OutputHandle.PutMessage($"Listing all assets. There are currently {count} assets being tracked.");
            OutputHandle.PutMessage(
                "Id".PadRight(idPad) +
                "Model".PadRight(pad) +
                "Purchase Date".PadRight(pad) +
                "Expiry Date".PadRight(pad) +
                "Price".PadRight(pricePad) +
                "Office Location".PadRight(pad) +
                "Other info ...".PadRight(pad));


            foreach (Asset a in Assets.GetAssets()
                                       .OrderBy(a => a.OfficeID)
                                       .ThenBy(a => (a is Computer) ? 1 : 2) // Maybe a bit too "hacky"?
                                       .ThenBy(a => a.PurchaseDate).Take(20))
            {
                IConsoleOutput.Color color = IConsoleOutput.Color.WHITE;
                if (a.ExpiryDate < DateTime.Now || a.ExpiryDate < DateTime.Now.AddMonths(3)) // Passed expiry date or 3 months left
                {
                    color = IConsoleOutput.Color.RED; // red
                }
                else if (a.ExpiryDate < DateTime.Now.AddMonths(6))
                {
                    color = IConsoleOutput.Color.YELLOW; // red
                }

                OutputHandle.PutMessage(
                    a.AssetID.ToString().PadRight(idPad) +
                    a.ModelName.PadRight(pad) +
                    a.PurchaseDate.ToShortDateString().PadRight(pad) +
                    a.ExpiryDate.ToShortDateString().PadRight(pad) +
                    //a.Price.ToString().PadRight(pricePad) +
                    CurrencyConverter.PriceToString(a.Price, Offices.GetOffice(a.OfficeID).OfficeLocalCulture).PadRight(pricePad) +
                    Offices.GetOffice(a.OfficeID).ToString().PadRight(pad),
                    color,
                    newLine: false);

                switch (a)
                {
                    case Computer computer:
                        OutputHandle.PutMessage(
                            computer.OperatingSystem.PadRight(pad) +
                            computer.RAM.PadRight(pad) +
                            computer.Processor.PadRight(pad),
                            color);
                        break;
                    case Cellphone phone:
                        OutputHandle.PutMessage(
                            phone.PhoneOperator.PadRight(pad) +
                            phone.PhoneNumber.PadRight(pad),
                            color);
                        break;
                }
            }

            return true;
        }
    }
}
