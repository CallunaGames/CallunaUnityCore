using System;
using System.Collections.Generic;

namespace Calluna
{
    public class AccumulatingFloatValue : AccumulatingValue<float>
    {
        private Mode _mode;

        public AccumulatingFloatValue(Mode mode = Mode.AddUp)
        {
            _mode = mode;
        }

        protected override float CalculateValue(IEnumerable<float> values)
        {
            return _mode switch
            {
                Mode.AddUp   => EnumerableUtility.Sum(values),
                Mode.Multiply => EnumerableUtility.Product(values),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public enum Mode
        {
            AddUp = 1,
            Multiply = 2,
        }
    }
}
