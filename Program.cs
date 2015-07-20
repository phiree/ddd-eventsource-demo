using System;

namespace ddd_column
{
    class Program
    {
        private static IReadRepository<ColumnDTO> _readRepository;
        private static IEventStore _eventStore;

        static void Main(string[] args)
        {
            EventBus bus = new EventBus();
            _eventStore = new MemoryEventStore(bus);
            EventSourcedRepository<Column> eventSourcedRepository = new EventSourcedRepository<Column>(((id, events) => new Column(id, events)), _eventStore);

            ColumnCommandHandler commandHandler = new ColumnCommandHandler(eventSourcedRepository);
            _readRepository = new MemoryReadRepository<ColumnDTO>();
            ColumnView columnView = new ColumnView(_readRepository);

            bus.Subscribe<ColumnCreated>(columnView.Handle);
            bus.Subscribe<ColumnRenamed>(columnView.Handle);
            bus.Subscribe<ColumnDataTypeChanged>(columnView.Handle);
            bus.Subscribe<ColumnMadePrimary>(columnView.Handle);
            bus.Subscribe<ColumnPrimaryCleared>(columnView.Handle);

            PerformSomeActions(commandHandler);
        }

        private static void PerformSomeActions(ColumnCommandHandler commandHandler)
        {
            Guid id = Guid.NewGuid();
            commandHandler.Apply(new CreateColumn
                {
                    Id = id,
                    Name = "Column Name",
                    DataType = DataType.Text
                });

            commandHandler.Apply(new RenameColumn
                {
                    Id = id,
                    Name = "New Column Name"
                });

            commandHandler.Apply(new MakeColumnPrimary
                {
                    Id = id
                });

            commandHandler.Apply(new ChangeColumnDataType
                {
                    Id = id,
                    DataType = DataType.Date
                });

            commandHandler.Apply(new ChangeColumnDataType
            {
                Id = id,
                DataType = DataType.Number
            });

            ShowReadModel(id);

            Console.WriteLine("Events");
            Console.WriteLine("------");
            RenderEvents(id);
        }

        private static void ShowReadModel(Guid id)
        {
            var column = _readRepository.Get(id);
            Console.WriteLine("CREATE COLUMN `{0}` ({1}){2};", column.Name, column.DataType, column.IsPrimary ? " PRIMARY KEY" : "");
        }

        private static void RenderEvents(Guid id)
        {
            foreach (var e in _eventStore.EventsFor(id))
            {
                Console.WriteLine("Event {0}", e.GetType().Name);
            }
        }
    }
}
