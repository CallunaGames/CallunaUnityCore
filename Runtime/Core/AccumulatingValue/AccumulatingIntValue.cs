using System.Collections.Generic;

namespace Calluna
{
    public class AccumulatingIntValue : AccumulatingValue<int>
    {
        protected override int CalculateValue(IEnumerable<int> values) => EnumerableUtility.Sum(values);
    }
}
