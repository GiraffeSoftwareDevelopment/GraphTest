using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphUtility
{
    class HaltonSequence
    {
        private HaltonSequence() { }
        public static float VanDerCorputSequence(Int32 n, Int32 cn)
        {
            float q = 0.0f;
            float bk = (1.0f / cn);
            while (n > 0)
            {
                q += (n % cn) * bk;
                n /= cn;
                bk /= cn;
            }
            return (q);
        }
        public static void Get(List<float> result, Int32 n, Int32 dimension)
        {
            Debug.Assert(dimension < 10);
            Int32[] primes = new Int32[] { 2, 3, 5, 7, 9, 11, 13, 17, 19, 23 };
            for (Int32 index1 = 0; index1 < dimension; index1++)
            {
                result.Add(HaltonSequence.VanDerCorputSequence(n, primes[index1]));
            }
        }
        public static void Build(Int32 count, List<GPoint> points)
        {
            HaltonSequence.Build(count, 1.0f, 1.0f, points);
        }
        public static void Build(Int32 count, float width, float height, List<GPoint> points)
        {
            Random rand = new Random();
            Int32 start = rand.Next(20, 256);
            Int32 dimension = 2;
            List<float> hs = new List<float>();
            for (Int32 index1 = 0; index1 < count; index1++)
            {
                hs.Clear();
                HaltonSequence.Get(hs, start + index1, dimension);
                hs[0] *= width;
                hs[1] *= height;
                points.Add(new GPoint(hs[0], hs[1]));
            }
        }
    }
}
