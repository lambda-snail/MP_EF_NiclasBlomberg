using System;
using Xunit;

using MP1.AssetTracker;

namespace UnitTest.Main
{
    public class CurrencyConverterTestcs
    {
        [Fact]
        public void TestConversion_WithoutErrors()
        {
            CurrencyConverter.PriceToString(123.0, new System.Globalization.CultureInfo("se-SE"));
            CurrencyConverter.PriceToString(123.0, new System.Globalization.CultureInfo("ja-JP"));
            CurrencyConverter.PriceToString(123.0, new System.Globalization.CultureInfo("fr-FR"));
        }

        [Fact]
        public void TestConversion_WithErrors()
        {
            Action nullCultureInfo = () =>
                CurrencyConverter.PriceToString(123.0, null);

            Action cultureNotInSystem = () =>
                CurrencyConverter.PriceToString(123.0, new System.Globalization.CultureInfo("fi-FI"));

            Assert.Throws<ArgumentException>(cultureNotInSystem);
            Assert.Throws<NullReferenceException>(nullCultureInfo);
        }
    }
}
