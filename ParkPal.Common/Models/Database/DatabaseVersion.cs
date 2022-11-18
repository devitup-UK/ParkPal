using System;

namespace ParkPal.Common.Models.Database
{
    public class DatabaseVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }

        public DatabaseVersion(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        public int Concat()
        {
            return int.Parse(Major.ToString() + Minor.ToString() + Revision.ToString()); 
        }

        public string Print()
        {
            return $"{Major}.{Minor}.{Revision}";
        }
    }
}