
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MP1.AssetTracker
{
    /// <summary>
    /// Class responsible for converting between different currencies, and also
    /// for formatting strings of prices correctly.
    /// 
    /// Note that the values are hard-coded for now.
    /// </summary>
    public static class CurrencyConverter
    {
        /// <summary>
        /// Contains details on how to convert prices in usd to other currencies.
        /// Units are printed as "SEK" or "EUR" and so on, to avoid a problem with the
        /// console not being able to display symbols for certain currencies (EUR and JPY).
        /// </summary>
        private static Dictionary<string, double> CurrencyConversionTable =
            new Dictionary<string, double>() 
            {
                { "ja-JP", 104.663 },  // jpy
                { "se-SE", 8.48 },     // sek
                { "fr-FR", 0.84 }      // eur
            };

        private static Dictionary<string, string> CurrencySymbolTable =
            new Dictionary<string, string>()
            {
                { "ja-JP", "JPY" },
                { "se-SE", "SEK" },
                { "fr-FR", "EUR" }
            };

        public static string PriceToString(double usd, CultureInfo country)
        {
            if(country == null)
            {
                throw new NullReferenceException("No culture information provided.");
            }    
            if(! CurrencyConversionTable.ContainsKey(country.Name))
            {
                throw new ArgumentException("Could not find currency for: " + country.Name);
            }

            return $"{CurrencySymbolTable[country.Name]} {(usd * CurrencyConversionTable[country.Name])}";
                
        }
    }
} 