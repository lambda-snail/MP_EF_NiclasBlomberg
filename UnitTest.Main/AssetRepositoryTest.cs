using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

using MP1.AssetTracker;

namespace UnitTest.Main
{
    internal class AssetDatabaseMock : IFileManager
    {
        public string File { get; }

        public AssetDatabaseMock(string file)
        {
            File = file;
        }

        public StreamReader GetStreamReader(string path)
        {
            return new StreamReader(File);
        }
    }

    public class AssetRepositoryTest
    {
        private AssetRepository GetTestRepository(string file)
        {
            return new AssetRepository(new AssetDatabaseMock(file));
        }

        private Dictionary<string, string> GetAssetParams_NoErrors()
        {
            return new()
            {
                ["Type"] = "Computer",
                ["PurchaseDate"] = "1990-01-01",
                ["ExpiryDate"] = "1990-01-01",
                ["Price"] = "145", // Avoid decmals, since they can be '.' or ',' deending on environment
                ["ModelName"] = "XXX",
                ["OfficeID"] = "1",
                ["OS"] = "win",
                ["RAM"] = "128TB",
                ["Processor"] = "MIPS" // :)
            };
        }

        /// <summary>
        /// Test to see if data is read if the data contains no errors.
        /// </summary>
        [Fact]
        public void TestReadData_DataWithoutErrors()
        {
            AssetRepository repo = GetTestRepository("TestData_NoErrorsInData.txt");

            Asset a = repo.GetAsset(1);
            Computer c = a as Computer;

            Assert.Equal( 2, repo.GetAssets().Count() );
            Assert.IsType<Computer>(a);

            Assert.Equal(1, c.AssetID);
            Assert.Equal(DateTime.Parse("2017 - 08 - 15"), c.PurchaseDate);
            Assert.Equal(DateTime.Parse("2020 - 08 - 15"), c.ExpiryDate);
            Assert.Equal(1500.0, c.Price, 4);
            Assert.Equal("MacBook", c.ModelName);
            Assert.Equal(1, c.OfficeID);
            Assert.Equal("macOS", c.OperatingSystem);
            Assert.Equal("8GB", c.RAM);
            Assert.Equal("PowerPC", c.Processor);
        }

        [Fact]
        public void TestReadData_DataWithErrors()
        {
            Action sameIdAction = () => GetTestRepository("TestData_SameID.txt");
            Action wrongDateAction = () => GetTestRepository("TestData_WrongDate.txt");

            Assert.Throws<ArgumentException>(sameIdAction);
            Assert.Throws<FormatException>(wrongDateAction);
        }

        [Fact]
        public void TestGetAsset()
        {
            AssetRepository repo = GetTestRepository("TestData_NoErrorsInData.txt");

            Asset found = repo.GetAsset(1);
            Asset shouldBeNull = repo.GetAsset(3);

            Assert.Null(shouldBeNull);
            Assert.NotNull(found);
            Assert.Equal("MacBook", found.ModelName);
        }

        [Fact]
        public void TestAddAsset_NoErrors()
        {
            AssetRepository repo = GetTestRepository("TestData_NoErrorsInData.txt");
            int initialRepoSize = repo.GetAssets().Count();

            Dictionary<string, string> param = GetAssetParams_NoErrors();

            repo.AddAsset(param);

            Assert.Equal(initialRepoSize + 1, repo.GetAssets().Count());
        }

        [Fact]
        public void TestAddAsset_NonExisingType()
        {
            Action testNonexistingType =
                () =>
                {
                    AssetRepository repo = GetTestRepository("TestData_NoErrorsInData.txt");
                    Dictionary<string, string> param = GetAssetParams_NoErrors();
                    param["Type"] = "SomeWeirdTypeThatDoesNotExist";
                    repo.AddAsset(param);
                };

            Assert.Throws<InvalidDataException>(testNonexistingType);
        }
    }
}
