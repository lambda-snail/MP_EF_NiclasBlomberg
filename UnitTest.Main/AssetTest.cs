using System;
using Xunit;
using MPEF.AssetTracker.Model;

namespace UnitTest.Main
{
    public class AssetTest
    {
        [Fact]
        public void CreateAsset()
        {
            int id = 0;
            DateTime purchaseDate = DateTime.Parse("2020-01-01");
            DateTime expiryDate = DateTime.Parse("2020-01-01");
            double price = 100;
            string modelName = "xxx";
            int officeID = 1;

            string OS = "win";
            string RAM = "16GB";
            string Processor = "Intel";

            Action negativePrice = () =>
                {
                    price = -100;
                    new Computer(purchaseDate, expiryDate, price, modelName, officeID, OS, RAM, Processor);
                };

            Action emptyStringProperty = () =>
                {
                    modelName = "";
                    price = 100; // This was changed in previous action
                    new Computer(purchaseDate, expiryDate, price, modelName, officeID, OS, RAM, Processor);
                };

            Assert.Throws<ArgumentException>(negativePrice);
            Assert.Throws<ArgumentNullException>(emptyStringProperty);
        }
    }
}
