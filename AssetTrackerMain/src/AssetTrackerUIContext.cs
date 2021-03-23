using System;
using System.Collections.Generic;
using System.Linq;

using SCLI.Core;

namespace MP1.AssetTracker
{
    /// <summary>
    /// Implements the following commands in the ui:
    /// list - to lsit all assets in the system.
    /// add  - to add a new asset to the system.
    /// </summary>
    public class AssetTrackerUIContext : Context
    {
        public IAssetRepository assets { get; private set; }

        public IOfficeRepository offices { get; private set; }

        public AssetTrackerUIContext(IConsoleOutput output, IConsoleInput input, IAssetRepository r, IOfficeRepository o)
            : base(output, input, "")
        {
            assets = r;
            offices = o;
        }

        /// <summary>
        /// The 'list' command implementation.
        /// </summary>
        public bool ListAllAssetsCommand(string cmdName, string[] cmdArgs)
        {
            int count = assets.GetAssets().Count();

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


            foreach (Asset a in assets.GetAssets()
                                       .OrderBy(a => a.OfficeID )
                                       .ThenBy(a => (a is Computer) ? 1 : 2) // Maybe a bit too "hacky"?
                                       .ThenBy(a => a.PurchaseDate))
            {
                IConsoleOutput.Color color = IConsoleOutput.Color.WHITE;
                if(a.ExpiryDate < DateTime.Now || a.ExpiryDate < DateTime.Now.AddMonths(3)) // Passed expiry date or 3 months left
                {
                    color = IConsoleOutput.Color.RED; // red
                }
                else if(a.ExpiryDate < DateTime.Now.AddMonths(6))
                {
                    color = IConsoleOutput.Color.YELLOW; // red
                }

                OutputHandle.PutMessage(
                    a.AssetID.ToString().PadRight(idPad) + 
                    a.ModelName.PadRight(pad) +
                    a.PurchaseDate.ToShortDateString().PadRight(pad) +
                    a.ExpiryDate.ToShortDateString().PadRight(pad) +
                    //a.Price.ToString().PadRight(pricePad) +
                    CurrencyConverter.PriceToString(a.Price, offices.GetOffice(a.OfficeID).OfficeLocalCulture).PadRight(pricePad) +
                    offices.GetOffice(a.OfficeID).ToString().PadRight(pad),
                    color,
                    newLine:false);

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

        /// <summary>
        /// Implementation of the 'add' command. Current usage:
        ///  add Computer
        ///  add Cellphone
        /// </summary>
        public bool AddAssetCommand(string cmdName, string[] cmdArgs)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            // Type of asset
            string type;
            if(cmdArgs.Length > 0 && cmdArgs[0].ToLower() == "computer")
            {
                type = "Computer";
            }
            else if (cmdArgs.Length > 0 && cmdArgs[0].ToLower() == "cellphone")
            {
                type = "Cellphone";
            }
            else
            {
                OutputHandle.PutMessage("Usage: 'add computer' or 'add cellphone'.", IConsoleOutput.Color.YELLOW);
                OutputHandle.PutMessage("Note that these are the only assets that are implemented in this iteration.");
                return false;
            }

            parameters["Type"] = type;
            
            // Logic to get location
            OutputHandle.PutMessage("In which country is the office located?");
            Office office = offices.GetOffice(InputHandle.ReadLine());
            while(office == null)
            {
                OutputHandle.PutMessage("Unknown country, please try again.", IConsoleOutput.Color.RED);
                OutputHandle.PutMessage("Available options: ");
                foreach(Office o in offices.GetOffices())
                {
                    OutputHandle.PutMessage(o.Location);
                }
                office = offices.GetOffice(InputHandle.ReadLine());
            }

            OutputHandle.PutMessage($"Location set: {office}.");
            OutputHandle.PutMessage("");
            parameters["OfficeID"] = office.OfficeID.ToString();

            // Purchase date
            OutputHandle.PutMessage("Enter date of purchase. The date should be in local time.");
            DateTime purchaseDate;
            while(!DateTime.TryParse(InputHandle.ReadLine(), out purchaseDate))
            {
                OutputHandle.PutMessage("Please enter a valid date.", IConsoleOutput.Color.RED);
            }

            parameters["PurchaseDate"] = purchaseDate.ToString();

            // Expiry date
            DateTime expiryDate = purchaseDate.AddYears(3);
            OutputHandle.PutMessage($"The expiry date has been calculated to: {expiryDate.ToShortDateString()} (local time).");
            parameters["ExpiryDate"] = expiryDate.ToString();

            // Price
            double price = -1;
            OutputHandle.PutMessage("Enter purchase price of asset, in USD.");
            while(!double.TryParse(InputHandle.ReadLine(), out price) || price < 0)
            {
                if (price < 0)
                {
                    OutputHandle.PutMessage("Price must not be less than 0.", IConsoleOutput.Color.RED);
                }
                else
                {
                    OutputHandle.PutMessage("Please enter a valid number.", IConsoleOutput.Color.RED);
                }
            }

            parameters["Price"] = price.ToString();

            // Model name
            OutputHandle.PutMessage("Enter model name.");
            string name = InputHandle.ReadLine();
            while(string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            {
                OutputHandle.PutMessage("Name is blank. Please enter a valid model name.", IConsoleOutput.Color.RED);
                name = InputHandle.ReadLine();
            }

            parameters["ModelName"] = name;

            // Read type-specific data
            if(type == "Computer")
            {
                // OS
                OutputHandle.PutMessage("Please enter the OS of the computer.");
                string os = InputHandle.ReadLine().Trim();
                while (string.IsNullOrEmpty(os))
                {
                    OutputHandle.PutMessage("Name cannot be blank.", IConsoleOutput.Color.RED);
                    os = InputHandle.ReadLine().Trim();
                }

                parameters["OS"] = os;

                // RAM
                OutputHandle.PutMessage("Please enter the amount of RAM in the computer.");
                string RAM = InputHandle.ReadLine().Trim();
                while (string.IsNullOrEmpty(RAM))
                {
                    OutputHandle.PutMessage("Name cannot be blank.", IConsoleOutput.Color.RED);
                    RAM = InputHandle.ReadLine().Trim();
                }

                parameters["RAM"] = RAM;

                // Processor
                OutputHandle.PutMessage("Please enter the processor.");
                string processor = InputHandle.ReadLine().Trim();
                while (string.IsNullOrEmpty(processor))
                {
                    OutputHandle.PutMessage("Name cannot be blank.", IConsoleOutput.Color.RED);
                    processor = InputHandle.ReadLine().Trim();
                }

                parameters["Processor"] = processor;
            }
            else // Cellphone
            {
                // Operator
                OutputHandle.PutMessage("Please enter the name of the phone operator.");
                string phoneOperator = InputHandle.ReadLine().Trim();
                while(string.IsNullOrEmpty(phoneOperator))
                {
                    OutputHandle.PutMessage("Name cannot be blank.", IConsoleOutput.Color.RED);
                    phoneOperator = InputHandle.ReadLine().Trim();
                }

                parameters["PhoneOperator"] = phoneOperator;

                // Phone number
                OutputHandle.PutMessage("Please enter the phone number.");
                string phoneNumber = InputHandle.ReadLine().Trim();
                while (string.IsNullOrEmpty(phoneNumber))
                {
                    OutputHandle.PutMessage("Name cannot be blank.", IConsoleOutput.Color.RED);
                    phoneNumber = InputHandle.ReadLine().Trim();
                }

                parameters["PhoneNumber"] = phoneNumber;
            }

            try
            {
                assets.AddAsset(parameters);
            }
            catch(Exception e)
            {
                OutputHandle.PutMessage(e.Message);
            }

            OutputHandle.PutMessage("Asset added to the system successfully.");

            return true;
        }
    }
}
