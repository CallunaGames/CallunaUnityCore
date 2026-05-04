using System;
using System.Collections.Generic;

namespace Calluna
{
    public class AccumulatingFloatValue : AccumulatingValue<float>
    {
        private readonly Mode _mode;

        public AccumulatingFloatValue(Mode mode = Mode.Sum)
        {
            _mode = mode;
        }

        protected override float CalculateValue(IEnumerable<float> values)
        {
            return _mode switch
            {
                Mode.Sum      => EnumerableUtility.Sum(values),
                Mode.Multiply => EnumerableUtility.Product(values),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public enum Mode
        {
            Sum = 1,
            Multiply = 2,
        }
    }
}
