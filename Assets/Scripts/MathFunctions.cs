using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class MathFunctions
    {
        public static float Normalize(float mainValue, float min, float max,
            float newMin, float newMax)
        {
            // formula taken from here
            //https://stats.stackexchange.com/a/281164/364135

            float temp = (mainValue - min) / (max - min);

            temp = temp * (newMax - newMin) + newMin;

            return temp;
        }
    }
}
