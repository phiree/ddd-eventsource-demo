using ddd_column.Events;
using ddd_column.Framework;

namespace ddd_column.ReadModel
{
    public class CalculationView
        : IEventHandler<CalculationAdded>
            , IEventHandler<CalculationRemoved>
            , IEventHandler<CalculationOperandChanged>
            , IEventHandler<CalculationOperatorChanged>
    {
        private readonly IRepository<CalculationDTO> _repository;
        public CalculationView(IRepository<CalculationDTO> repository)
        {
            _repository = repository;
        }

        public void Handle(CalculationAdded @event)
        {
            _repository.Save(new CalculationDTO(@event.CalculationId)
                {
                    Operand = @event.Operand,
                    Operator = @event.Operator,
                    ColumnId = @event.Id
            });
        }

        public void Handle(CalculationRemoved @event)
        {
            _repository.Remove(@event.CalculationId);
        }

        public void Handle(CalculationOperandChanged @event)
        {
            CalculationDTO calculation = _repository.Get(@event.CalculationId);
            calculation.Operand = @event.Operand;
            _repository.Save(calculation);
        }

        public void Handle(CalculationOperatorChanged @event)
        {
            CalculationDTO calculation = _repository.Get(@event.CalculationId);
            calculation.Operator = @event.Operator;
            _repository.Save(calculation);
        }
    }
}