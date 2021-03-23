
using System;
using System.Globalization;

namespace MP1.AssetTracker.Model
{

    public class Office
    {
        /// <summary>
        /// The country in which the office is located.
        /// </summary>
        public string Location { get; }

        /// <summary>
        /// The string describing the cultrue of the country in which the office resides.
        /// </summary>
        public string Culture { get; }

        public int OfficeID { get; }

        public CultureInfo OfficeLocalCulture { get; }

        public Office(string location, string culture)
        {
            ValidateStringProperty(location, "Location");
            Location = location;

            ValidateStringProperty(culture, "Culture");
            Culture = culture;

            OfficeLocalCulture = new CultureInfo(culture);
        }

        public override string ToString()
        {
            string retstr = "";
            switch (Location.ToLower())
            {
                case "japan":
                    retstr = "Tokyo";
                    break;
                case "sweden":
                    retstr = "Stockholm";
                    break;
                case "france":
                    retstr = "Paris";
                    break;
                default:
                    retstr = Location;
                    break;
            }

            return retstr;
        }

        private void ValidateStringProperty(string prop, string propName)
        {
            if(string.IsNullOrEmpty(prop) || string.IsNullOrWhiteSpace(prop))
            {
                throw new ArgumentNullException("Argument bland or null: " + propName);
            }
        }

    }
}
