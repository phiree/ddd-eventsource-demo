using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ddd_column.Commands;
using ddd_column.Domain;
using ddd_column.Framework;
using ddd_column.ReadModel;

namespace ddd_column
{
    public class Results
    {
        public Results(TimeSpan elapsed, int successCount, int failureCount, int conflictCount)
        {
            SuccessCount = successCount;
            Elapsed = elapsed;
            FailureCount = failureCount;
            ConflictCount = conflictCount;
        }

        public TimeSpan Elapsed { get; private set; }
        public int SuccessCount { get; private set; }
        public int FailureCount { get; private set; }
        public int ConflictCount { get; private set; }
        public int Total { get { return SuccessCount + FailureCount + ConflictCount; } }

        public int CommandsPerSecond
        {
            get
            {
                return (int)(Total / Elapsed.TotalSeconds);
            }
        }

    }

    class RandomCommandRunner
    {
        private readonly Dictionary<Guid, bool> _potentialColumns;
        private readonly ColumnCommandHandler _columnCommandHandler;
        private readonly IReadRepository<ColumnDTO> _columnRepository;
        private readonly IReadOnlyList<Func<Guid, ICommand>> _commandFactories;

        public RandomCommandRunner(IEnumerable<Guid> potentialColumnIds, ColumnCommandHandler columnCommandHandler, IReadRepository<ColumnDTO> columnRepository)
        {
            _potentialColumns = potentialColumnIds.ToDictionary(id => id, id => false);
            _columnCommandHandler = columnCommandHandler;
            _columnRepository = columnRepository;

            _commandFactories = new List<Func<Guid, ICommand>>
        {
                RenameColumn,
                ChangeDataType,
                MakePrimary,
                ClearPrimary,
                AddCalculation,
                ChangeOperator,
                ChangeOperand,
                RemoveCalculation
            };
        }


        public Results RunSomeCommands(int commandCount)
        {
            int numSuccess = 0;
            int numFailures = 0;
            int numConcurrencyFailures = 0;

            var watch = new Stopwatch();
            watch.Start();

            Enumerable.Range(0, commandCount)
                .AsParallel().WithDegreeOfParallelism(4)
                .ForAll(_ =>
                {
                    try
                    {
                        RunACommand();
                        Interlocked.Increment(ref numSuccess);
                    }
                    catch (DBConcurrencyException e)
                    {
                        Interlocked.Increment(ref numConcurrencyFailures);
                    }
                    catch (Exception)
                    {
                        Interlocked.Increment(ref numFailures);
                    }
                });

            watch.Stop();

            return new Results(watch.Elapsed, numSuccess, numFailures, numConcurrencyFailures);
        }

        private void RunACommand()
        {
            Guid columnId = Some.ElementIn(_potentialColumns.Keys.ToList());

            ICommand command = _potentialColumns[columnId]
                ? CreateModificationCommand(columnId)
                : CreateColumn(columnId);

            _columnCommandHandler.Apply((dynamic)command);
            _potentialColumns[columnId] = true;
        }

        private ICommand CreateModificationCommand(Guid columnId)
        {
            Func<Guid, ICommand> factory = Some.ElementIn(_commandFactories);
            return factory(columnId);
        }

        private static CreateColumn CreateColumn(Guid columnId)
        {
            return new CreateColumn()
                {
                    Id = columnId,
                    Name = Some.AlphanumericString(),
                    DataType = Some.Enum<DataType>()
                };
        }

        private static ICommand ClearPrimary(Guid columnId)
        {
            return new ClearColumnPrimary { Id = columnId };
        }

        private static ICommand MakePrimary(Guid columnId)
        {
            return new MakeColumnPrimary { Id = columnId };
        }

        private static ICommand ChangeDataType(Guid columnId)
        {
            return new ChangeColumnDataType
            {
                Id = columnId,
                DataType = Some.Enum<DataType>()
            };
        }

        private static ICommand RenameColumn(Guid columnId)
        {
            return new RenameColumn
            {
                Id = columnId,
                Name = Some.AlphanumericString()
            };
        }

        private static ICommand AddCalculation(Guid columnId)
        {
            return new AddCalculation
            {
                Id = columnId,
                CalculationId = Guid.NewGuid(),
                Operand = Some.Integer(),
                Operator = Some.Enum<Operator>()
            };
        }

        private ICommand ChangeOperand(Guid columnId)
        {
            //ColumnDTO column = _columnRepository.Get(columnId);
            //
            //if (!column.Calculations.Any() || Some.RandomInteger(100) < 10)
            //    return AddOrRemoveCalculation(column);

            return new ChangeOperand
            {
                Id = columnId,
                CalculationId = Guid.NewGuid(),// Some.ElementIn(column.Calculations),
                Operand = Some.Integer(),
            };
        }

        private ICommand ChangeOperator(Guid columnId)
        {
            //ColumnDTO column = _columnRepository.Get(columnId);
            //
            //if (!column.Calculations.Any() || Some.RandomInteger(100) < 10)
            //    return AddOrRemoveCalculation(column);

            return new ChangeOperator
            {
                Id = columnId,
                CalculationId = Guid.NewGuid(),//Some.ElementIn(column.Calculations),
                Operator = Some.Enum<Operator>()
            };
        }

        private ICommand RemoveCalculation(Guid columnId)
        {
            //ColumnDTO column = _columnRepository.Get(columnId);
            //
            //if (!column.Calculations.Any())
            //    return AddCalculation(columnId);

            return new RemoveCalculation
            {
                Id = columnId,
                CalculationId = Guid.NewGuid()//Some.ElementIn(column.Calculations)
            };
        }


        private ICommand AddOrRemoveCalculation(ColumnDTO column)
        {
            if (!column.Calculations.Any() || Some.RandomInteger(100) < 50 && column.Calculations.Count < 5)
                return AddCalculation(column.Id);
            else
                return RemoveCalculation(column.Id);
        }
    }
}
