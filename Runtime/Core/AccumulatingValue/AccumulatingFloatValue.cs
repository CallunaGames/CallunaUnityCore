using System;
using System.Collections.Generic;

namespace Calluna
{
    public class AccumulatingFloatValue : AccumulatingValue<float>
    {
        private readonly Mode _mode;

        /// <summary>Without parts, <see cref="Mode.Sum"/> is 0 and <see cref="Mode.Multiply"/> is 1.</summary>
        public AccumulatingFloatValue(Mode mode = Mode.Sum)
        {
            _mode = mode;
            Recalculate();
        }

        protected override float CalculateValue(IReadOnlyList<float> values)
        {
            switch (_mode)
            {
                case Mode.Sum:
                    float sum = 0f;
                    for (int i = 0; i < values.Count; i++)
                        sum += values[i];
                    return sum;
                case Mode.Multiply:
                    float product = 1f;
                    for (int i = 0; i < values.Count; i++)
                        product *= values[i];
                    return product;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public enum Mode
        {
            Sum = 1,
            Multiply = 2,
        }
    }
}
