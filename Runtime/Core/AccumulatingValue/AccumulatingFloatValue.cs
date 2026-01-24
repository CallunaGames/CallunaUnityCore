using System.Collections.Generic;
using System.Linq;

namespace Calluna
{
    public class AccumulatingFloatValue : AccumulatingValue<float>
    {
        protected override float CalculateValue(IEnumerable<float> values)
        {
            return values.Sum(v => v);
        }
    }
}