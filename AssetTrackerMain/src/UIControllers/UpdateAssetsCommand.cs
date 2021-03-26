using MPEF.AssetTracker.Model;
using SCLI.Core;
using System;
using System.Reflection;

namespace MPEF.AssetTracker.Main.UIControllers
{
    public class UpdateAssetsCommand : AssetTrackerCommandBase
    {
        public UpdateAssetsCommand(IConsoleOutput outputHandle, IUserInput inputHandle, IAssetRepository assetRepo, IOfficeRepository officeRepo)
            : base(outputHandle, inputHandle, assetRepo, officeRepo) { }


        public bool UpdateAssetCommand(string cmdName, string[] cmdArgs)
        {
            OutputHandle.PutMessage("Enter the id of the asset you wish to modify.");

            int id = 0;
            while (!int.TryParse(InputHandle.GetEditableInputWithDefaultText(), out id))
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
            foreach (var property in properties)
            {
                if (property.Name != "AssetID") // We don't want the user changing the id!
                {
                    OutputHandle.PutMessage("Enter a new '" + property.Name + "' or press return to keep the existing value.");

                    // This could probably be written nicer without code duplication ...
                    if (property.PropertyType == typeof(string))
                    {
                        string newValue = InputHandle.GetEditableInputWithDefaultText(property.GetValue(changeTarget).ToString());
                        while (string.IsNullOrEmpty(newValue))
                        {
                            OutputHandle.PutMessage("Please enter a valid string.", IConsoleOutput.Color.YELLOW);
                        }
                        property.SetValue(tmpCopy, newValue);
                    }
                    else if (property.PropertyType == typeof(int))
                    {
                        int newValue = 0;
                        while (!int.TryParse(
                                    InputHandle.GetEditableInputWithDefaultText(property.GetValue(changeTarget).ToString()),
                                    out newValue))
                        {
                            OutputHandle.PutMessage("Please enter a valid number.", IConsoleOutput.Color.YELLOW);
                        }
                        property.SetValue(tmpCopy, newValue);
                    }
                    else if (property.PropertyType == typeof(double))
                    {
                        double newValue = 0;
                        while (!double.TryParse(
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
                        while (!DateTime.TryParse(
                                    InputHandle.GetEditableInputWithDefaultText(property.GetValue(changeTarget).ToString()),
                                    out newValue))
                        {
                            OutputHandle.PutMessage("Please enter a valid Date.", IConsoleOutput.Color.YELLOW);
                        }
                        property.SetValue(tmpCopy, newValue);
                    }
                    else
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
