using System;

namespace ddd_column
{
    public class ColumnCreated : IEvent
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public DataType Type { get; set; }

        public ColumnCreated(Guid id, string name, DataType type)
        {
            Id = id;
            Name = name;
            Type = type;
        }
    }

    public class ColumnRenamed : IEvent
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public ColumnRenamed(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class ColumnPrimaryCleared : IEvent
    {
        public Guid Id { get; set; }

        public ColumnPrimaryCleared(Guid id)
        {
            Id = id;
        }
    }

    public class ColumnMadePrimary : IEvent
    {
        public Guid Id { get; set; }

        public ColumnMadePrimary(Guid id)
        {
            Id = id;
        }
    }

    public class ColumnDataTypeChanged : IEvent
    {
        public Guid Id { get; set; }
        public DataType DataType { get; set; }

        public ColumnDataTypeChanged(Guid id, DataType dataType)
        {
            Id = id;
            DataType = dataType;
        }
    }
}