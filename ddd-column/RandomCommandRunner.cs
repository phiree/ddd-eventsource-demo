using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ddd_column.Commands;
using ddd_column.Domain;
using ddd_column.Framework;

namespace ddd_column
{
    public class Results
    {
        public Results(TimeSpan elapsed, int failureCount, int successCount)
        {
            Elapsed = elapsed;
            FailureCount = failureCount;
            SuccessCount = successCount;
        }

        public TimeSpan Elapsed { get; private set; }
        public int FailureCount { get; private set; }
        public int SuccessCount { get; private set; }
        public int Total { get { return FailureCount + SuccessCount; } }

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

        private List<Func<Guid, ICommand>> _commandFactories = new List<Func<Guid, ICommand>>()
            {
                RenameColumn,
                ChangeDataType,
                MakePrimary,
                ClearPrimary
            };

        public RandomCommandRunner(IEnumerable<Guid> potentialColumnIds, ColumnCommandHandler columnCommandHandler)
        {
            _potentialColumns = potentialColumnIds.ToDictionary(id => id, id => false);
            _columnCommandHandler = columnCommandHandler;

            //foreach (var id in _potentialColumns.Keys)
            //    _columnCommandHandler.Apply(CreateColumn(id));
        }

        public Results RunSomeCommands(int commandCount)
        {
            int numFailures=0;

            var watch = new Stopwatch();
            watch.Start();

            for (var i = 0; i < commandCount; i++)
            {
                try
                {
                    RunACommand();
                }
                catch (Exception)
                {
                    numFailures++;
                }
            }

            watch.Stop();

            return new Results(watch.Elapsed, numFailures, commandCount - numFailures);
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
    }
}
