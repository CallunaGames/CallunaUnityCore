using System;
using System.Collections.Generic;

namespace Calluna
{
    public class AccumulatingBoolValue : AccumulatingValue<bool>
    {
        private readonly Mode _mode;

        /// <summary>Without parts, <see cref="Mode.Any"/> is false and <see cref="Mode.All"/> is true.</summary>
        public AccumulatingBoolValue(Mode mode = Mode.Any)
        {
            _mode = mode;
            Recalculate();
        }

        protected override bool CalculateValue(IReadOnlyList<bool> values)
        {
            switch (_mode)
            {
                case Mode.Any:
                    for (int i = 0; i < values.Count; i++)
                        if (values[i]) return true;
                    return false;
                case Mode.All:
                    for (int i = 0; i < values.Count; i++)
                        if (!values[i]) return false;
                    return true;
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
