using System;
using System.Collections.Generic;
using ddd_column.Domain;
using ddd_column.Framework;

namespace ddd_column.ReadModel
{
    public class ColumnDTO : IKeyedObject
    {
        public ColumnDTO(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
        public string Name { get; set; }
        public DataType DataType { get; set; }
        public bool IsPrimary { get; set; }

        public List<Guid> Calculations { get; set; } = new List<Guid>();
    }
}