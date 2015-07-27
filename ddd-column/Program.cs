using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ddd_column.Commands;
using ddd_column.Domain;
using ddd_column.Events;
using ddd_column.Framework;
using ddd_column.ReadModel;

namespace ddd_column
{
    class Program
    {
        private static IReadRepository<ColumnDTO> _columnReadRepository;
        private static IReadRepository<CalculationDTO> _calculationReadRepository;
        private static IEventStore _eventStore;

        private const int CommandsPerProfileBatch = 5000;
        private const int ColumnCount = 100;
        private const int NumProfileIterations = 50;
        private const int EventsPerSnapshot = 10;

        static void Main(string[] args)
        {
            EventBus bus = new EventBus();
            _eventStore = new MemoryEventStore(bus);

            IEventSourcedRepository<Column> eventSourcedRepository = true
                ? CreateNonSnapshottingRepository()
                : CreateSnapshottingRepository(EventsPerSnapshot);

            ColumnCommandHandler commandHandler = new ColumnCommandHandler(eventSourcedRepository);
            _columnReadRepository = new MemoryReadRepository<ColumnDTO>();
            _calculationReadRepository = new MemoryReadRepository<CalculationDTO>();
            ColumnView columnView = new ColumnView(_columnReadRepository);
            CalculationView calculationView = new CalculationView(_calculationReadRepository);

            //bus.Subscribe<IEvent>(ev => _log.Information("New Event: {@Event}", ev));

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
            Thread.Sleep(100);
            ShowReadModel();

            PerformLotsOfActions(commandHandler);
        }

        private static EventSourcedRepository<Column> CreateNonSnapshottingRepository()
        {
            return new EventSourcedRepository<Column>(((id, events) => new Column(id, events)), _eventStore);
        }

        private static IEventSourcedRepository<Column> CreateSnapshottingRepository(int eventsPerSnapshot)
        {
            Func<Guid, IEnumerable<IEvent>, Column> createColumn = ((id, events) => new Column(id, events));
            MemoryReadRepository<ColumnSnapshot> snapshotRepository = new MemoryReadRepository<ColumnSnapshot>();
            return new SnapshottingEventSourcedRepository<Column, ColumnSnapshot>(createColumn, _eventStore, snapshotRepository, Column.Snapshotter, eventsPerSnapshot);
        }

        private static void PerformLotsOfActions(ColumnCommandHandler commandHandler)
        {
            var randomRunner = new RandomCommandRunner(Enumerable.Range(1, ColumnCount).Select(i => Some.Guid()), commandHandler, _columnReadRepository);

            for (var i = 0; i < NumProfileIterations; i++)
            {
                var results = randomRunner.RunSomeCommands(CommandsPerProfileBatch);
                Console.WriteLine("{0} commands: {1} succeeded, {2} failed, {3} conflicts", results.Total, results.SuccessCount, results.FailureCount, results.ConflictCount);
                Console.WriteLine("  {0} commands per second", results.CommandsPerSecond);
                Console.WriteLine();
            }
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
        }

        private static void ShowReadModel()
        {
            foreach (var column in _columnReadRepository.All.ToList())
            {
                Console.WriteLine("SQL for {0}", column.Id);
                Console.WriteLine("  CREATE COLUMN `{0}` ({1}){2};", column.Name, column.DataType, column.IsPrimary
                    ? " PRIMARY KEY"
                    : "");

                var calculations = column.Calculations.ToList();
                if (calculations.Any())
                {
                    Console.WriteLine();
                    Console.WriteLine("Calculations:");
                    Console.Write("  Initial Value");

                    foreach (var calcId in calculations.ToList())
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
        }
    }
}
