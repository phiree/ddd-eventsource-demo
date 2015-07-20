
using System;

namespace ddd_column
{
    class Program
    {
        private static IReadRepository<ColumnDTO> _readRepository;

        static void Main(string[] args)
        {
            EventBus realBus = new EventBus();
            UnitOfWorkEventBus unitOfWorkBus = new UnitOfWorkEventBus(realBus);

            MemoryRepository<Column> repository = new MemoryRepository<Column>();
            UnitOfWorkRepository<Column> unitOfWorkRepository = new UnitOfWorkRepository<Column>(repository, unitOfWorkBus);

            ColumnCommandHandler commandHandler = new ColumnCommandHandler(unitOfWorkRepository, unitOfWorkBus);
            _readRepository = new MemoryReadRepository<ColumnDTO>();
            ColumnView columnView = new ColumnView(_readRepository);

            realBus.Subscribe<ColumnCreated>(columnView.Handle);
            realBus.Subscribe<ColumnRenamed>(columnView.Handle);
            realBus.Subscribe<ColumnDataTypeChanged>(columnView.Handle);
            realBus.Subscribe<ColumnMadePrimary>(columnView.Handle);
            realBus.Subscribe<ColumnPrimaryCleared>(columnView.Handle);

            PerformSomeActions(commandHandler);
        }

        private static void ShowReadModel(Guid id)
        {
            var column = _readRepository.Get(id);
            Console.WriteLine("CREATE COLUMN `{0}` ({1}){2};", column.Name, column.DataType, column.IsPrimary ? " PRIMARY KEY" : "");
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
        }
    }
}
