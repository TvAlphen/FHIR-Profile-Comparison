using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileComparisonMethod
{
    public static class StringToInteger
    {
        public static int MaxInt(ElementDefinition one)
        {
            int max1 = 0;
            if (one.Max == "*") max1 = int.MaxValue;
            else max1 = int.Parse(one.Max);
            return max1;
        }
    }
}
