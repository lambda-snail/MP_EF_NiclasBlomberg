
using System;

namespace MP1.AssetTracker.Model
{
    public abstract class Asset
    {
        public int AssetID { get; private set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public double Price { get; set; }
        public string ModelName { get; set; }
        public int OfficeID { get; set; }

        public Asset() : base() { }

        public Asset(DateTime purchaseDate, DateTime expiryDate, double price, string modelName, int officeID)
        {
            ValidateDates(purchaseDate, expiryDate);
            PurchaseDate = purchaseDate;
            ExpiryDate = expiryDate;

            ValidatePrice(price);
            Price = price;

            ValidateStringProperty(modelName, "ModelName");
            ModelName = modelName;

            OfficeID = officeID;
        }

        /// <summary>
        /// Validate the DateTime objects used by the class. Note that it is not allowed to create
        /// assets puhcased in the future, but expired assets can still be created.
        /// </summary>
        protected void ValidateDates(DateTime purchase, DateTime expiry)
        {
            if (purchase == null)
            {
                throw new ArgumentNullException("Purhcase date cannot be empty.");
            }
            else if (expiry == null)
            {
                throw new ArgumentNullException("Expiry date cannot be empty.");
            } else if (purchase > DateTime.Now)
            {
                throw new InvalidOperationException("Cannot create Asset purchased in the future: " + purchase.ToString());
            }    
        }

        protected void ValidatePrice(double price)
        {
            if (price < 0)
            {
                throw new ArgumentException("Price cannot be negative: " + price);
            }
        }

        protected void ValidateStringProperty(string property, string propertyName)
        {
            if (string.IsNullOrEmpty(property) || string.IsNullOrWhiteSpace(property))
            {
                throw new ArgumentNullException("Property cannot be null or empty: " + propertyName);
            }
        }

        protected void ValidateLocation(Office location)
        {
            _ = location ?? throw new ArgumentNullException("Location null.");
        }
    }
}
