using System;
using System.Collections.Generic;
using ddd_column.Framework;

namespace ddd_column.Domain
{
    public class ColumnSnapshot : ISnapshot<Column>
    {
        public ColumnSnapshot(Guid id, int version, int schemaVersion)
        {
            Id = id;
            Version = version;
            SchemaVersion = schemaVersion;
        }

        public DataType DataType { get; set; }

        public bool IsPrimary { get; set; }

        public List<CalculationSnapshot> Calculations { get; set; }

        public Guid Id { get; private set; }

        public int Version { get; private set; }
        public int SchemaVersion { get; private set; }
    }
}