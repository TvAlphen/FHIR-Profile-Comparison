using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PogingOmIetsTeVergelijken4
{
    class CompareBinding
    {
        public static double DistanceBinding(ElementDefinition.BindingComponent binding1, ElementDefinition.BindingComponent binding2, double weight)
        {
            double distance = 0;
            if (binding1 == null && binding2 == null) { return 0; }
            if ((binding1 == null && binding2 != null) || (binding2 == null && binding1 != null))
            {
                Program.LogAspectDifference(weight, "Binding");
                return weight;
            }
            if (binding1.Strength != binding2.Strength) { distance += AspectWeights.WEIGHT_BINDING_STRENGTH; }
            if (!CompareValueProperty.ComplexElementComparer.Default.Equals(binding1.ValueSet, binding2.ValueSet)) { distance += AspectWeights.WEIGHT_BINDING_VALUESET; }
            if (binding1.Description != binding2.Description) { distance += AspectWeights.WEIGHT_BINDING_DESCRIPTION; }
            Program.LogAspectDifference((weight * distance), "Binding");
            return weight * distance;
        }
    }
}
