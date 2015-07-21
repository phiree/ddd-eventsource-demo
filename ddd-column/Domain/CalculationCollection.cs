using System;
using System.Collections.ObjectModel;

namespace ddd_column.Domain
{
    public class CalculationCollection : KeyedCollection<Guid, Calculation>
    {
        protected override Guid GetKeyForItem(Calculation item)
        {
            return item.Id;
        }
    }
}