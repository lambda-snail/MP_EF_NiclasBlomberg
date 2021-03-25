using System;
using System.Collections.Generic;
using System.Linq;

using SCLI.Core;
using MP1.AssetTracker.Model;
using System.Reflection;

namespace MP1.AssetTracker.Main.UIItems
{
    /// <summary>
    /// Implements the following commands in the ui:
    /// add  - to add a new asset to the system.
    /// list - to lsit all assets in the system.
    /// </summary>
    public class AssetTrackerMainMenu : Context
    {
        protected IAssetRepository Assets { get; private set; }
        protected IOfficeRepository Offices { get; private set; }

        public AssetTrackerMainMenu(IConsoleOutput output, IUserInput inputHandle, IAssetRepository assetRepo, IOfficeRepository officeRepo)
            : base(output, inputHandle, "")
        {
            Assets = assetRepo;
            Offices = officeRepo;

            AddCommand("list", ListAllAssetsCommand);
            AddCommand("add", AddAssetCommand); 
            AddCommand("update", UpdateAssetCommand);
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

        public bool UpdateAssetCommand(string cmdName, string[] cmdArgs)
        {
            OutputHandle.PutMessage("Enter the id of the asset you wish to modify.");

            int id = 0;
            while( ! int.TryParse(InputHandle.GetEditableInputWithDefaultText(), out id) )
            {
                OutputHandle.PutMessage("Please enter a valid number.", IConsoleOutput.Color.YELLOW);
            }

            Asset changeTarget = Assets.GetAsset(id);

            // We will make changes to this object, and then transfer them to the original object
            // after validation. If the user entered something wrong, we can revert to the old state
            // by simply discarding this object.
            // This method seems to be the simplest when using the reflection-based approach, since we
            // do not know the order in which the properties will be presented to the user.
            Asset tmpCopy = changeTarget.ShallowCopy();

            PropertyInfo[] properties = tmpCopy.GetType().GetProperties();
            foreach(var property in properties)
            {
                if (property.Name != "AssetID") // We don't want the user changing the id!
                {
                    OutputHandle.PutMessage("Enter a new '" + property.Name + "' or press return to keep the existing value.");

                    // This could probably be written nicer without code duplication ...
                    if(property.PropertyType ==  typeof(string))
                    {
                        string newValue = InputHandle.GetEditableInputWithDefaultText(property.GetValue(changeTarget).ToString());
                        while (string.IsNullOrEmpty(newValue))
                        {
                            OutputHandle.PutMessage("Please enter a valid string.", IConsoleOutput.Color.YELLOW);
                        }
                        property.SetValue(tmpCopy, newValue);
                    } 
                    else if(property.PropertyType == typeof(int))
                    {
                        int newValue = 0;
                        while ( ! int.TryParse(
                                    InputHandle.GetEditableInputWithDefaultText(property.GetValue(changeTarget).ToString()),
                                    out newValue))
                        {
                            OutputHandle.PutMessage("Please enter a valid number.", IConsoleOutput.Color.YELLOW);
                        }
                        property.SetValue(tmpCopy, newValue);
                    }
                    else if(property.PropertyType == typeof(double))
                    {
                        double newValue = 0;
                        while ( ! double.TryParse(
                                    InputHandle.GetEditableInputWithDefaultText(property.GetValue(changeTarget).ToString()),
                                    out newValue))
                        {
                            OutputHandle.PutMessage("Please enter a valid number.", IConsoleOutput.Color.YELLOW);
                        }
                        property.SetValue(tmpCopy, newValue);
                    }
                    else if (property.PropertyType == typeof(DateTime))
                    {
                        DateTime newValue;
                        while ( ! DateTime.TryParse(
                                    InputHandle.GetEditableInputWithDefaultText(property.GetValue(changeTarget).ToString()),
                                    out newValue))
                        {
                            OutputHandle.PutMessage("Please enter a valid Date.", IConsoleOutput.Color.YELLOW);
                        }
                        property.SetValue(tmpCopy, newValue);
                    } else
                    {
                        throw new InvalidOperationException("Unknown type error: " + property.Name);
                    }

                }
            }

            // Now check that the dates and prices are correct
            if (tmpCopy.Price < 0)
            {
                OutputHandle.PutMessage("Error: The price must not be negative.", IConsoleOutput.Color.RED);
                return false;
            }
            else if (tmpCopy.ExpiryDate < tmpCopy.PurchaseDate)
            {
                OutputHandle.PutMessage("Error: Expiry date cannot come after purchase date.", IConsoleOutput.Color.RED);
                return false;
            }
            else if (DateTime.Now < tmpCopy.PurchaseDate)
            {
                OutputHandle.PutMessage("Error: The specified purchase date is in the future. The asset tracker is not a list of things to buy!", IConsoleOutput.Color.RED);
                return false;
            }
            else if (Offices.GetOffice(tmpCopy.OfficeID) == null)
            {
                OutputHandle.PutMessage($"Error: An office with the specified id does not exist: {tmpCopy.OfficeID}.", IConsoleOutput.Color.RED);
                return false;
            }
            else
            // If we get here things should be OK, we can now save changes
            {
                foreach (var property in properties)
                {
                    if (property.Name != "AssetID")
                        property.SetValue(changeTarget, property.GetValue(tmpCopy));
                }

                Assets.UpdateAsset(changeTarget);
                OutputHandle.PutMessage("Changes saved.");
                return true;
            }
        }
    }
}
