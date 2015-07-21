using ddd_column.Commands;
using ddd_column.Framework;

namespace ddd_column.Domain
{
    public class ColumnCommandHandler
        : ICommandHandler<CreateColumn>
        , ICommandHandler<RenameColumn>
        , ICommandHandler<ChangeColumnDataType>
        , ICommandHandler<MakeColumnPrimary>
        , ICommandHandler<ClearColumnPrimary>
        , ICommandHandler<AddCalculation>
        , ICommandHandler<RemoveCalculation>
        , ICommandHandler<ChangeOperator>
        , ICommandHandler<ChangeOperand>
    {
        private readonly IEventSourcedRepository<Column> _columnRepository;

        public ColumnCommandHandler(IEventSourcedRepository<Column> columnRepository)
        {
            _columnRepository = columnRepository;
        }

        public void Apply(CreateColumn createColumn)
        {
            Column column = new Column(createColumn.Id, createColumn.Name, createColumn.DataType);
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

        public void Apply(AddCalculation command)
        {
            Column column = _columnRepository.Get(command.Id);
            column.AddCalculation(command.CalculationId, command.Operator, command.Operand);
            _columnRepository.Save(column);
        }

        public void Apply(RemoveCalculation command)
        {
            Column column = _columnRepository.Get(command.Id);
            column.RemoveCalculation(command.CalculationId);
            _columnRepository.Save(column);
        }

        public void Apply(ChangeOperator command)
        {
            Column column = _columnRepository.Get(command.Id);
            column.ChangeOperator(command.CalculationId, command.Operator);
            _columnRepository.Save(column);
        }

        public void Apply(ChangeOperand command)
        {
            Column column = _columnRepository.Get(command.Id);
            column.ChangeOperand(command.CalculationId, command.Operand);
            _columnRepository.Save(column);
        }
    }
}