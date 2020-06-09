using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class HaltonSequence
{
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
        Int32[] primes = new Int32[]{ 2, 3, 5, 7, 9, 11, 13, 17, 19, 23 };
        for (Int32 index1 = 0; index1 < dimension; index1++)
        {
            result.Add(HaltonSequence.VanDerCorputSequence(n, primes[index1]));
        }
    }
}
