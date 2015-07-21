using ddd_column.Events;
using ddd_column.Framework;

namespace ddd_column.ReadModel
{
    public class ColumnView
        : IEventHandler<ColumnCreated>
        , IEventHandler<ColumnRenamed>
        , IEventHandler<ColumnDataTypeChanged>
        , IEventHandler<ColumnMadePrimary>
        , IEventHandler<ColumnPrimaryCleared>
    {
        private readonly IReadRepository<ColumnDTO> _repository;

        public ColumnView(IReadRepository<ColumnDTO> repository)
        {
            _repository = repository;
        }

        public void Handle(ColumnCreated @event)
        {
            _repository.Save(new ColumnDTO(@event.Id)
                {
                    Name = @event.Name,
                    DataType = @event.Type
                });
        }

        public void Handle(ColumnRenamed @event)
        {
            var column = _repository.Get(@event.Id);
            column.Name = @event.Name;
            _repository.Save(column);
        }

        public void Handle(ColumnDataTypeChanged @event)
        {
            var column = _repository.Get(@event.Id);
            column.DataType = @event.DataType;
            _repository.Save(column);
        }

        public void Handle(ColumnMadePrimary @event)
        {
            var column = _repository.Get(@event.Id);
            column.IsPrimary = true;
            _repository.Save(column);
        }

        public void Handle(ColumnPrimaryCleared @event)
        {
            var column = _repository.Get(@event.Id);
            column.IsPrimary = false;
            _repository.Save(column);
        }
    }
}