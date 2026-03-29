using System.Collections.Generic;

namespace Calluna
{
    public static class EnumerableUtility
    {
        public static int Sum(IEnumerable<int> values)
        {
            int total = 0;
            foreach (int v in values) total += v;
            return total;
        }

        public static float Sum(IEnumerable<float> values)
        {
            float total = 0f;
            foreach (float v in values) total += v;
            return total;
        }

        // Seeds with the first element so a single-element sequence returns that value unchanged.
        public static float Product(IEnumerable<float> values)
        {
            float result = 0f;
            bool isFirst = true;
            foreach (float v in values)
            {
                if (isFirst) { result = v; isFirst = false; }
                else result *= v;
            }
            return result;
        }

        public static bool Any(IEnumerable<bool> values)
        {
            foreach (bool v in values) { if (v) return true; }
            return false;
        }

        public static bool All(IEnumerable<bool> values)
        {
            foreach (bool v in values) { if (!v) return false; }
            return true;
        }
    }
}
