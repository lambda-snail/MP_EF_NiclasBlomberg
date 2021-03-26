using MPEF.AssetTracker.Model;
using SCLI.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPEF.AssetTracker.Main.UIControllers
{
    class ListAssetsCommand : AssetTrackerCommandBase
    {
        // Constants determining padding
        static int Pad = 16;
        static int IndexdPad = Pad / 3; // Smaller numbers

        /// <summary>
        /// The 'list' command implementation.
        /// </summary>
        public ListAssetsCommand(IConsoleOutput outputHandle, IUserInput inputHandle, IAssetRepository assetRepo, IOfficeRepository officeRepo)
            : base(outputHandle, inputHandle, assetRepo, officeRepo) { }


        public bool ListAllAssetsCommand(string cmdName, string[] cmdArgs)
        {
            if (Assets.Count == 0)
            {
                OutputHandle.PutMessage("No assets in database.", IConsoleOutput.Color.GREEN);
                return true;
            }

            // Used when scrolling
            int pageSize = 20;
            int totalPageNum = Assets.Count / pageSize;
            int currentPageIndex = 0;

            // Header
            OutputHandle.PutMessage(
                "Id".PadRight(IndexdPad) +
                "Model".PadRight(Pad) +
                "Purchase Date".PadRight(Pad) +
                "Expiry Date".PadRight(Pad) +
                "Price".PadRight(Pad) +
                "Office Location".PadRight(Pad) +
                "Other info ...".PadRight(Pad));

            // Let user scroll up or down among the pages
            // Loop until user is finished
            string input = "";
            bool notDone = true;
            while(notDone)
            {
                ShowPage(pageSize, currentPageIndex);
                OutputHandle.PutMessage($"Displaying page {currentPageIndex + 1} of {totalPageNum}.");
                OutputHandle.PutMessage("Enter a number to go to that page. Type 'up' or 'down' to scroll up or down.");

                input = InputHandle.GetEditableInputWithDefaultText( currentPageIndex==0 ? "down" : "" ).ToLower();

                if(input == "down")
                {
                    if(currentPageIndex + 1 >= totalPageNum)
                    {
                        OutputHandle.PutMessage("No more assets. Aborting list.", IConsoleOutput.Color.GREEN);
                        return true;
                    } 
                    else
                    {
                        currentPageIndex++;
                    }
                }
                else if(input == "up")
                {
                    if(currentPageIndex == 0)
                    {
                        OutputHandle.PutMessage("You are already at the top page.", IConsoleOutput.Color.YELLOW);
                    }
                    else
                    {
                        currentPageIndex--;
                    }
                }
                else if(int.TryParse(input, out int userSelectedPage))
                {
                    if(userSelectedPage > 0 && userSelectedPage < totalPageNum)
                    {
                        currentPageIndex = userSelectedPage - 1; // index is zero-based, but index in ui is not
                    }
                    else
                    {
                        OutputHandle.PutMessage("Invalid page.", IConsoleOutput.Color.YELLOW);
                    }
                }
                else
                {
                    OutputHandle.PutMessage("Aborted.");
                    notDone = false;
                }
            }
            

            return true;
        }

        private void ShowPage(int pageSize, int pageIndex)
        {
            IEnumerable<Asset> page = Assets.GetAssetsPaged(pageSize, pageIndex);

            foreach (Asset a in page)
                                       //.OrderBy(a => a.OfficeID)
                                       //.ThenBy(a => (a is Computer) ? 1 : 2) // Maybe a bit too "hacky"?
                                       //.ThenBy(a => a.PurchaseDate))
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
                    a.AssetID.ToString().PadRight(IndexdPad) +
                    a.ModelName.PadRight(Pad) +
                    a.PurchaseDate.ToShortDateString().PadRight(Pad) +
                    a.ExpiryDate.ToShortDateString().PadRight(Pad) +
                    //a.Price.ToString().PadRight(pricePad) +
                    CurrencyConverter.PriceToString(a.Price, Offices.GetOffice(a.OfficeID).OfficeLocalCulture).PadRight(Pad) +
                    Offices.GetOffice(a.OfficeID).ToString().PadRight(Pad),
                    color,
                    newLine: false);

                switch (a)
                {
                    case Computer computer:
                        OutputHandle.PutMessage(
                            computer.OperatingSystem.PadRight(Pad) +
                            computer.RAM.PadRight(Pad) +
                            computer.Processor.PadRight(Pad),
                            color);
                        break;
                    case Cellphone phone:
                        OutputHandle.PutMessage(
                            phone.PhoneOperator.PadRight(Pad) +
                            phone.PhoneNumber.PadRight(Pad),
                            color);
                        break;
                }
            }
        }
    }
}
