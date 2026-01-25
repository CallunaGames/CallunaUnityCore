using System;
using System.Collections.Generic;
using System.Linq;

namespace Calluna
{
    public class AccumulatingFloatValue : AccumulatingValue<float>
    {
        private Mode _mode;

        public AccumulatingFloatValue()
        {
            _mode = Mode.AddUp;
        }

        public AccumulatingFloatValue(Mode mode)
        {
            _mode = mode;
        }
        
        protected override float CalculateValue(IEnumerable<float> values)
        {
            return _mode switch
            {
                Mode.AddUp => values.Sum(v => v),
                Mode.Multiply => Multiply(values),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private float Multiply(IEnumerable<float> values)
        {
            float accumulatedValue = 0;
            bool first = true;
            foreach (float value in values)
            {
                if (first)
                {
                    accumulatedValue = value;
                    first = false;
                    continue;
                }

                accumulatedValue *= value;
            }

            return accumulatedValue;
        }

        public enum Mode
        {
            AddUp = 1,
            Multiply = 2,
        }
    }
}