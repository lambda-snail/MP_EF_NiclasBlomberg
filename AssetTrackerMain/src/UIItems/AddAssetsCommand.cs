using MPEF.AssetTracker.Model;
using SCLI.Core;
using System;
using System.Collections.Generic;

namespace MPEF.AssetTracker.Main.UIItems
{
    class AddAssetsCommand : AssetTrackerCommandBase
    {
        public AddAssetsCommand(IConsoleOutput outputHandle, IUserInput inputHandle, IAssetRepository assetRepo, IOfficeRepository officeRepo)
            : base(outputHandle, inputHandle, assetRepo, officeRepo) { }

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
            if (cmdArgs.Length > 0 && cmdArgs[0].ToLower() == "computer")
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
            Office office = Offices.GetOffice(InputHandle.GetEditableInputWithDefaultText());
            while (office == null)
            {
                OutputHandle.PutMessage("Unknown country, please try again.", IConsoleOutput.Color.RED);
                OutputHandle.PutMessage("Available options: ");
                foreach (Office o in Offices.GetOffices())
                {
                    OutputHandle.PutMessage(o.Location);
                }
                office = Offices.GetOffice(InputHandle.GetEditableInputWithDefaultText());
            }

            OutputHandle.PutMessage($"Location set: {office}.");
            OutputHandle.PutMessage("");
            parameters["OfficeID"] = office.OfficeID.ToString();

            // Purchase date
            OutputHandle.PutMessage("Enter date of purchase. The date should be in local time.");
            DateTime purchaseDate;
            while (!DateTime.TryParse(InputHandle.GetEditableInputWithDefaultText(), out purchaseDate))
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
            while (!double.TryParse(InputHandle.GetEditableInputWithDefaultText(), out price) || price < 0)
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
            string name = InputHandle.GetEditableInputWithDefaultText();
            while (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            {
                OutputHandle.PutMessage("Name is blank. Please enter a valid model name.", IConsoleOutput.Color.RED);
                name = InputHandle.GetEditableInputWithDefaultText();
            }

            parameters["ModelName"] = name;

            // Read type-specific data
            if (type == "Computer")
            {
                // OS
                OutputHandle.PutMessage("Please enter the OS of the computer.");
                string os = InputHandle.GetEditableInputWithDefaultText().Trim();
                while (string.IsNullOrEmpty(os))
                {
                    OutputHandle.PutMessage("Name cannot be blank.", IConsoleOutput.Color.RED);
                    os = InputHandle.GetEditableInputWithDefaultText().Trim();
                }

                parameters["OS"] = os;

                // RAM
                OutputHandle.PutMessage("Please enter the amount of RAM in the computer.");
                string RAM = InputHandle.GetEditableInputWithDefaultText().Trim();
                while (string.IsNullOrEmpty(RAM))
                {
                    OutputHandle.PutMessage("Name cannot be blank.", IConsoleOutput.Color.RED);
                    RAM = InputHandle.GetEditableInputWithDefaultText().Trim();
                }

                parameters["RAM"] = RAM;

                // Processor
                OutputHandle.PutMessage("Please enter the processor.");
                string processor = InputHandle.GetEditableInputWithDefaultText().Trim();
                while (string.IsNullOrEmpty(processor))
                {
                    OutputHandle.PutMessage("Name cannot be blank.", IConsoleOutput.Color.RED);
                    processor = InputHandle.GetEditableInputWithDefaultText().Trim();
                }

                parameters["Processor"] = processor;
            }
            else // Cellphone
            {
                // Operator
                OutputHandle.PutMessage("Please enter the name of the phone operator.");
                string phoneOperator = InputHandle.GetEditableInputWithDefaultText().Trim();
                while (string.IsNullOrEmpty(phoneOperator))
                {
                    OutputHandle.PutMessage("Name cannot be blank.", IConsoleOutput.Color.RED);
                    phoneOperator = InputHandle.GetEditableInputWithDefaultText().Trim();
                }

                parameters["PhoneOperator"] = phoneOperator;

                // Phone number
                OutputHandle.PutMessage("Please enter the phone number.");
                string phoneNumber = InputHandle.GetEditableInputWithDefaultText().Trim();
                while (string.IsNullOrEmpty(phoneNumber))
                {
                    OutputHandle.PutMessage("Name cannot be blank.", IConsoleOutput.Color.RED);
                    phoneNumber = InputHandle.GetEditableInputWithDefaultText().Trim();
                }

                parameters["PhoneNumber"] = phoneNumber;
            }

            try
            {
                Assets.AddAsset(parameters);
            }
            catch (Exception e)
            {
                OutputHandle.PutMessage(e.Message);
            }

            OutputHandle.PutMessage("Asset added to the system successfully.");

            return true;
        }
    }
}
