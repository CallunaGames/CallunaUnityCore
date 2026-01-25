using System.Collections.Generic;
using System.Linq;

namespace Calluna
{
    public class AccumulatingIntValue : AccumulatingValue<int>
    {
        protected override int CalculateValue(IEnumerable<int> values)
        {
            return values.Sum(v => v);
        }
    }
}