using System;
using System.Collections.Generic;
using System.Text;

namespace MPEF.AssetTracker.Model
{
    public class Computer : Asset
    {
        public string OperatingSystem { get; set; }
        public string RAM { get; set; }
        public string Processor { get; set; }

        public Computer() : base() { }

        public Computer(DateTime purchaseDate, DateTime expiryDate,
                        double price, string modelName, int officeID,
                        string operatingSystem, string ram, string processor) :
            base(purchaseDate, expiryDate, price, modelName, officeID)
        {
            ValidateStringProperty(operatingSystem, "OperatingSystem");
            OperatingSystem = operatingSystem;

            ValidateStringProperty(ram, "RAM");
            RAM = ram;

            ValidateStringProperty(processor, "Processor");
            Processor = processor;
        }
    }
}
