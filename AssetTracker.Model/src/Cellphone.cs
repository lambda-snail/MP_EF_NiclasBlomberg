using System;
using System.Collections.Generic;
using System.Text;

namespace MPEF.AssetTracker.Model
{
    public class Cellphone : Asset
    {
        public string PhoneOperator { get; set; }
        public string PhoneNumber { get; set; }

        public Cellphone() : base() { }

        public Cellphone(DateTime purchaseDate, DateTime expiryDate,
                        double price, string modelName, int officeID,
                        string phoneOperator, string number) :
            base(purchaseDate, expiryDate, price, modelName, officeID)
        {
            ValidateStringProperty(phoneOperator, "PhoneOperator");
            PhoneOperator = phoneOperator;

            ValidateStringProperty(number, "PhoneNumber");
            PhoneNumber = number;
        }
    }
}
