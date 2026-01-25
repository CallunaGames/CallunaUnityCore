using System;
using System.Collections.Generic;
using System.Linq;

namespace Calluna
{
    public class AccumulatingBoolValue : AccumulatingValue<bool>
    {
        private Mode _mode;

        public AccumulatingBoolValue()
        {
            _mode = Mode.Any;
        }

        public AccumulatingBoolValue(Mode mode)
        {
            _mode = mode;
        }

        protected override bool CalculateValue(IEnumerable<bool> values)
        {
            return _mode switch
            {
                Mode.Any => values.Any(v => v),
                Mode.All => values.All(v => v),
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