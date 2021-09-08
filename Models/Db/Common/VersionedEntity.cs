using System;

namespace Models.Db.Common
{
    public class VersionedEntity : IdEntity
    {
        public uint Version { get; set; }

        public DateTime DateTime { get; set; }
    }
}