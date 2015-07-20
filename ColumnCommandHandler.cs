using System;

namespace ddd_column
{
    public class ColumnCommandHandler
        : ICommandHandler<CreateColumn>
        , ICommandHandler<RenameColumn>
        , ICommandHandler<ChangeColumnDataType>
        , ICommandHandler<MakeColumnPrimary>
        , ICommandHandler<ClearColumnPrimary>
    {
        private readonly IRepository<Column> _columnRepository;
        private readonly IEventBus _bus;

        public ColumnCommandHandler(IRepository<Column> columnRepository, IEventBus bus)
        {
            _columnRepository = columnRepository;
            _bus = bus;
        }

        public void Apply(CreateColumn createColumn)
        {
            Column column = new Column(_bus, createColumn.Id, createColumn.Name, createColumn.DataType);
            _columnRepository.Save(column);
        }

        public void Apply(RenameColumn command)
        {
            Column column = _columnRepository.Get(command.Id);
            column.Rename(command.Name);
            _columnRepository.Save(column);
        }

        public void Apply(ChangeColumnDataType command)
        {
            Column column = _columnRepository.Get(command.Id);
            column.ChangeDataType(command.DataType);
            _columnRepository.Save(column);
        }

        public void Apply(MakeColumnPrimary command)
        {
            Column column = _columnRepository.Get(command.Id);
            column.MakePrimary();
            _columnRepository.Save(column);
        }

        public void Apply(ClearColumnPrimary command)
        {
            Column column = _columnRepository.Get(command.Id);
            column.ClearPrimary();
            _columnRepository.Save(column);
        }
    }
}