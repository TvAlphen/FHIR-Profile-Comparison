using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileComparisonMethod
{
    class CompareBase
    {
        public static double DistanceBase(ElementDefinition.BaseComponent base1, ElementDefinition.BaseComponent base2, double weight)
        {
            double distance = 0;
            if (base1 == null && base2 == null) { return 0; }
            if ((base1 == null && base2 != null) || (base2 == null && base1 != null))
            {
                Program.LogAspectDifference(weight, "Base");
                return weight;
            }
            if (base1.Path != base2.Path)
            {
                distance += AspectWeights.WEIGHT_BASE_PATH;
                Program.LogAspectDifference((weight * distance), "Base");
                return (weight * distance);
            }
            if (base1.Min != base2.Min) { distance += AspectWeights.WEIGHT_BASE_MIN; }
            if (base1.Max != base2.Max) { distance += AspectWeights.WEIGHT_BASE_MAX; }
            Program.LogAspectDifference((weight * distance), "Base");
            return weight * distance;
        }
    }
}
