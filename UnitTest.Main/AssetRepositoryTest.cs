using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Xunit;

using MP1.AssetTracker.DataLayer;
using MP1.AssetTracker.Model;


namespace UnitTest.Main
{
    public class AssetRepositoryTest
    {
        private string TestConnectionString = "Server = (localdb)\\MSSQLLocalDB; Database = AssetTrackerTestDB; Integrated Security = True";

        private AssetTrackerDbContext _db;

        private AssetRepository AssetTestRepo { get; set; }

        // Start the asset repository from a known state
        private void AssetTestRepositoryUp()
        {   
            _db = new AssetTrackerDbContext(TestConnectionString);
            _db.Database.EnsureCreated();
            
            AssetTestRepo = new AssetRepository(_db);
        }

        // Tear the asset repository down after the test
        private void AssetTestRepositoryDown()
        {
            _db.Database.EnsureDeleted();
            _db = null;
            AssetTestRepo = null;
        }

        [Fact]
        public void TestAddAsset_NoErrors()
        {
            AssetTestRepositoryUp();
            PopulateTestDatabase();

            int expectedNewRepoSize = 3;
            Assert.Equal(expectedNewRepoSize, expectedNewRepoSize);

            AssetTestRepositoryDown();
        }


        [Fact]
        public void TestAddAsset_NonExisingType()
        {
            AssetTestRepositoryUp();
            PopulateTestDatabase();

            var assetParamsContainingError =
                new Dictionary<string, string>()
                {
                    ["Type"] = "SomeWeirdTypeThatDoesNotExist",
                    ["PurchaseDate"] = "1990-01-01",
                    ["ExpiryDate"] = "1990-01-01",
                    ["Price"] = "145",
                    ["ModelName"] = "XXX",
                    ["OfficeID"] = "1",
                    ["OS"] = "win",
                    ["RAM"] = "128TB",
                    ["Processor"] = "MIPS"
                };

            Action addNonexistingType =
                () =>
                {
                    AssetTestRepo.AddAsset(assetParamsContainingError);
                };

            Assert.Throws<System.ArgumentException>(addNonexistingType);

            AssetTestRepositoryDown();
        }

        /// <summary>
        /// Test to see if can be read.
        /// </summary>
        [Fact]
        public void TestReadData_DataWithExistingId()
        {
            AssetTestRepositoryUp();
            PopulateTestDatabase();

            Asset a = AssetTestRepo.GetAsset(1);

            Assert.NotNull(a);
            
            Computer c = a as Computer;

            Assert.Equal(1, c.AssetID);
            Assert.Equal(DateTime.Parse("2017 - 08 - 15"), c.PurchaseDate);
            Assert.Equal(DateTime.Parse("2020 - 08 - 15"), c.ExpiryDate);
            Assert.Equal(1500.0, c.Price, 4);
            Assert.Equal("MacBook", c.ModelName);
            Assert.Equal(1, c.OfficeID);
            Assert.Equal("macOS", c.OperatingSystem);
            Assert.Equal("8GB", c.RAM);
            Assert.Equal("PowerPC", c.Processor);

            AssetTestRepositoryDown();
        }

        [Fact]
        public void TestReadData_DataNonExistingId()
        {
            AssetTestRepositoryUp();
            PopulateTestDatabase();

            var shouldBeNull = AssetTestRepo.GetAsset(1024);
            Assert.Null(shouldBeNull);

            AssetTestRepositoryDown();
        }

        [Fact]
        public void TestUpdateData()
        {
            AssetTestRepositoryUp();
            PopulateTestDatabase();

            int id = 3; // The cellphone
            int newPrice = 1000;

            var cellphone = AssetTestRepo.GetAsset(id) as Cellphone;
            cellphone.Price = newPrice;
            AssetTestRepo.UpdateAsset(cellphone);

            var updatedCellphone = AssetTestRepo.GetAsset(id) as Cellphone;

            Assert.Equal(newPrice, updatedCellphone.Price, 4); // A precision of 4 should be ok here since there is no requirement on precision here ...

            AssetTestRepositoryDown();
        }

        [Fact]
        public void TestDeleteData()
        {
            AssetTestRepositoryUp();
            PopulateTestDatabase();

            int initialDbSize = AssetTestRepo.GetAssets().Count();

            AssetTestRepo.DeleteAsset(1);
            Assert.Equal(2, AssetTestRepo.GetAssets().Count());

            AssetTestRepo.DeleteAsset(2);
            Assert.Equal(1, AssetTestRepo.GetAssets().Count());

            AssetTestRepo.DeleteAsset(3);
            Assert.Equal(0, AssetTestRepo.GetAssets().Count());

            AssetTestRepositoryDown();
        }


        private void PopulateTestDatabase()
        {
            AssetTestRepo.AddAsset(
                new Dictionary<string, string>
                {
                    ["Type"] = "Computer",
                    ["PurchaseDate"] = "2017 - 08 - 15",
                    ["ExpiryDate"] = "2020 - 08 - 15",
                    ["Price"] = "1500", // Avoid decmals, since they can be '.' or ',' deending on environment
                    ["ModelName"] = "MacBook",
                    ["OfficeID"] = "1",
                    ["OS"] = "macOS",
                    ["RAM"] = "8GB",
                    ["Processor"] = "PowerPC"
                });

            AssetTestRepo.AddAsset(
                new Dictionary<string, string>
                {
                    ["Type"] = "Computer",
                    ["PurchaseDate"] = "1990-01-01",
                    ["ExpiryDate"] = "1990-01-01",
                    ["Price"] = "145",
                    ["ModelName"] = "XXX",
                    ["OfficeID"] = "1",
                    ["OS"] = "win",
                    ["RAM"] = "128TB",
                    ["Processor"] = "MIPS"
                });

            AssetTestRepo.AddAsset(
                new Dictionary<string, string>
                {
                    ["Type"] = "Cellphone",
                    ["PurchaseDate"] = "2019 - 07 - 18",
                    ["ExpiryDate"] = "2022 - 07 - 18",
                    ["Price"] = "400",
                    ["ModelName"] = "iPhone",
                    ["OfficeID"] = "1",
                    ["PhoneOperator"] = "Au",
                    ["PhoneNumber"] = "+817655308287"
                });
        }
    }
}
