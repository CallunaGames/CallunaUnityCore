using System.Collections.Generic;

namespace Calluna
{
    public class AccumulatingIntValue : AccumulatingValue<int>
    {
        public AccumulatingIntValue()
        {
            Recalculate();
        }

        protected override int CalculateValue(IReadOnlyList<int> values)
        {
            int sum = 0;
            for (int i = 0; i < values.Count; i++)
                sum += values[i];
            return sum;
        }
    }
}
