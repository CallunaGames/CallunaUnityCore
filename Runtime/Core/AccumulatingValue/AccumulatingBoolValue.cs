using System;
using System.Collections.Generic;

namespace Calluna
{
    public class AccumulatingBoolValue : AccumulatingValue<bool>
    {
        private Mode _mode;

        public AccumulatingBoolValue(Mode mode = Mode.Any)
        {
            _mode = mode;
        }

        protected override bool CalculateValue(IEnumerable<bool> values)
        {
            return _mode switch
            {
                Mode.Any => EnumerableUtility.Any(values),
                Mode.All => EnumerableUtility.All(values),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public enum Mode
        {
            Any = 1,
            All = 2,
        }
    }
}
