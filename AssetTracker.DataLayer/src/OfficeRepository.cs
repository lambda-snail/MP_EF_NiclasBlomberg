
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MPEF.AssetTracker.Model;

namespace MPEF.AssetTracker.DataLayer
{
    /// <summary>
    /// A class that abstracts the storage and retreival of Office objects.
    /// Currently, the offices are hard-coded for simplicity.
    /// </summary>
    public class OfficeRepository : IOfficeRepository
    {
        private AssetTrackerDbContext _db; // Also tracks offices despite the name :)

        public OfficeRepository(AssetTrackerDbContext database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("OfficeRepository: No database specified");
            }
            _db = database;
        }

        public void AddOffice(Office office)
        {
            if(office == null)
                throw new ArgumentNullException("Argument null: AddOffice");

            _db.Add(office);
            _db.SaveChanges();
        }

        /// <summary>
        /// Retreive the Office with the specified id. If no such Office exists, the method returns null.
        /// </summary>
        public Office GetOffice(int id)
        {
            return _db.Offices.Find(id);
        }

        /// <summary>
        /// Retreive the Office in the specified country. Note that this assumes one office per country, and
        /// will fail if there are more offices in the database for the same country -- Office management will be added
        /// in a future iteration :)
        /// </summary>
        public Office GetOffice(string country)
        {
            return _db.Offices.Where(o => o.Location.ToLower() == country.ToLower()).Single();
        }

        public void UpdateOffice(Office office)
        {
            _db.Update(office);
            _db.SaveChanges();
        }

        public IEnumerable<Office> GetOffices()
        {
            return _db.Offices.ToList();
        }

        public void DeleteOffice(int id)
        {
            var office = _db.Offices.Find(id);
            if(office != null)
            {
                _db.Remove(office);
                _db.SaveChanges();
            }
        }
    }
}
