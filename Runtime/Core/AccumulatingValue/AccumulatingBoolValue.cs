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
            switch (_mode)
            {
                case Mode.Any:
                    return values.Any(v => v);
                case Mode.All:
                    return values.All(v => v);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public enum Mode
        {
            Any = 1,
            All = 2,
        }
    }
}