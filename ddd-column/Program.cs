using System;
using System.Linq;
using ddd_column.Commands;
using ddd_column.Domain;
using ddd_column.Events;
using ddd_column.Framework;
using ddd_column.ReadModel;
using Serilog;

namespace ddd_column
{
    class Program
    {
        private static IReadRepository<ColumnDTO> _columnReadRepository;
        private static IReadRepository<CalculationDTO> _calculationReadRepository;
        private static IEventStore _eventStore;
        private static ILogger _log;

        static void Main(string[] args)
        {
            _log = Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            EventBus bus = new EventBus();
            _eventStore = new MemoryEventStore(bus);
            EventSourcedRepository<Column> eventSourcedRepository = new EventSourcedRepository<Column>(((id, events) => new Column(id, events)), _eventStore);

            ColumnCommandHandler commandHandler = new ColumnCommandHandler(eventSourcedRepository);
            _columnReadRepository = new MemoryReadRepository<ColumnDTO>();
            _calculationReadRepository = new MemoryReadRepository<CalculationDTO>();
            ColumnView columnView = new ColumnView(_columnReadRepository);
            CalculationView calculationView = new CalculationView(_calculationReadRepository);

            bus.Subscribe<IEvent>(ev => _log.Information("New Event: {@Event}", ev));

            bus.Subscribe<ColumnCreated>(columnView.Handle);
            bus.Subscribe<ColumnRenamed>(columnView.Handle);
            bus.Subscribe<ColumnDataTypeChanged>(columnView.Handle);
            bus.Subscribe<ColumnMadePrimary>(columnView.Handle);
            bus.Subscribe<ColumnPrimaryCleared>(columnView.Handle);
            bus.Subscribe<CalculationAdded>(columnView.Handle);
            bus.Subscribe<CalculationRemoved>(columnView.Handle);

            bus.Subscribe<CalculationAdded>(calculationView.Handle);
            bus.Subscribe<CalculationRemoved>(calculationView.Handle);
            bus.Subscribe<CalculationOperandChanged>(calculationView.Handle);
            bus.Subscribe<CalculationOperatorChanged>(calculationView.Handle);

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

            Guid calculationId = Guid.NewGuid();
            commandHandler.Apply(new AddCalculation
            {
                Id = id,
                CalculationId = calculationId,
                Operator = Operator.Multiply,
                Operand = 3
            });

            commandHandler.Apply(new ChangeOperand
            {
                Id = id,
                CalculationId = calculationId,
                Operand = 2
            });

            commandHandler.Apply(new ChangeOperator
            {
                Id = id,
                CalculationId = calculationId,
                Operator = Operator.Divide,
            });

            commandHandler.Apply(new AddCalculation
            {
                Id = id,
                CalculationId = Guid.NewGuid(),
                Operator = Operator.Multiply,
                Operand = 3
            });


            commandHandler.Apply(new AddCalculation
            {
                Id = id,
                CalculationId = Guid.NewGuid(),
                Operator = Operator.Add,
                Operand = 98
            });

            ShowReadModel(id);
        }

        private static void ShowReadModel(Guid id)
        {
            var column = _columnReadRepository.Get(id);
            Console.WriteLine("SQL:");
            Console.WriteLine("  CREATE COLUMN `{0}` ({1}){2};", column.Name, column.DataType, column.IsPrimary ? " PRIMARY KEY" : "");

            if (column.Calculations.Any())
            {
                Console.WriteLine();
                Console.WriteLine("Calculations:");
                Console.Write("  Initial Value");

                foreach (var calcId in column.Calculations)
                {
                    var calc = _calculationReadRepository.Get(calcId);
                    var operatorAsString = calc.Operator == Operator.Add
                        ? "+"
                        : calc.Operator == Operator.Divide
                            ? "/"
                            : calc.Operator == Operator.Multiply
                                ? "*"
                                : "-";
                    Console.Write(" {0} {1}", operatorAsString, calc.Operand);
                }
                Console.WriteLine();
            }
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
