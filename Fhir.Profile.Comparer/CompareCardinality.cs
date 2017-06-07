using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PogingOmIetsTeVergelijken4
{
    public static class CompareCardinality
    {
        public static double CompareRangesCardinality(ElementDefinition one, ElementDefinition two)
        {
            var MaxOne = StringToInteger.MaxInt(one);
            var MaxTwo = StringToInteger.MaxInt(two);
            double difference = 0;
            // max van 1 is kleiner dan min van 2..geen overlap
            if ((MaxOne != 0 && MaxOne < two.Min) || (MaxTwo != 0 && MaxTwo < one.Min)) difference += AspectWeights.BOTH_MANDATORY_NO_OVERLAP;
            
            // 1 mandatory en 1 optional
            if ((one.Min != 0 && one.Max != "0" && two.Min == 0 && two.Max != "0") || (two.Min != 0 && two.Max != "0" && one.Min == 0 && one.Max != "0")) difference += AspectWeights.MANDATORY_OPTIONAL;
            // both optional 
            if (one.Min == two.Min && one.Min == 0 && MaxOne != MaxTwo && MaxOne != 0 && MaxTwo != 0) difference += AspectWeights.MIN_OR_MAX_DIFFERS;
            // range van 2 binnen range van 1 beide verplicht
            if ((one.Min != 0 && one.Min < two.Min && MaxOne == MaxTwo) || (one.Min != 0 && one.Min == two.Min && MaxOne > MaxTwo)) difference += AspectWeights.MIN_OR_MAX_DIFFERS;
            if (one.Min != 0 && one.Min < two.Min && MaxOne > MaxTwo) difference += AspectWeights.BOTH_MANDATORY_RANGE_WIHTIN_OTHER;
            // range van 1 binnen range van 2 beide verplicht
            if ((two.Min != 0 && two.Min < one.Min && MaxTwo == MaxOne) || (two.Min != 0 && two.Min == one.Min && MaxTwo > MaxOne)) difference += AspectWeights.MIN_OR_MAX_DIFFERS;
            if (two.Min != 0 && two.Min < one.Min && MaxTwo > MaxOne) difference += AspectWeights.BOTH_MANDATORY_RANGE_WIHTIN_OTHER;
            // half overlap beide verplicht
            if ((one.Min != 0 && one.Min < two.Min && MaxOne < MaxTwo) || (two.Min != 0 && one.Min > two.Min && MaxOne > MaxTwo)) difference += AspectWeights.BOTH_MANDATORY_PARTIAL_OVERLAP;
            // beide precies gelijk difference blijft 0
            Program.LogAspectDifference(difference, "Cardinality");
            return difference;
        }
    }
}
