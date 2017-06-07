using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PogingOmIetsTeVergelijken4
{
    class CompareValueProperty
    {
        public class ComplexElementComparer : IEqualityComparer<Element>
        {
            // Singleton
            public static ComplexElementComparer Default { get; } = new ComplexElementComparer();

            public bool Equals(Element x, Element y) => (x == null || y == null) ? (y == null && x == null) : x.IsExactly(y);

            public int GetHashCode(Element obj) => 0;
        }

        public static double CompareDefaultValue (ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            if (!ComplexElementComparer.Default.Equals(one.DefaultValue, two.DefaultValue)) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "DefaultValue");
            return factor * weight;
        }

        public static double CompareFixed(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            //if (one.Fixed == null && two.Fixed == null) { return 0; }
            //if ((one.Fixed == null && two.Fixed != null) || (two.Fixed == null && one.Fixed != null))
            //{
            //    Program.LogAspectDifference(weight, "Fixed");
            //    return weight;
            //}
            if (!ComplexElementComparer.Default.Equals(one.Fixed, two.Fixed)) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "FixedValue");
            return factor * weight;
        }

        public static double ComparePattern(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            //if (one.Pattern == null && two.Pattern == null) { return 0; }
            //if ((one.Pattern == null && two.Pattern != null) || (two.Pattern == null && one.Pattern != null))
            //{
            //    Program.LogAspectDifference(weight, "Pattern");
            //    return weight;
            //}
            if (!ComplexElementComparer.Default.Equals(one.Pattern, two.Pattern)) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "Pattern");
            return factor * weight;
        }

        public static double CompareExample(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            //if (one.Example == null && two.Example == null) { return 0; }
            //if ((one.Example == null && two.Example != null) || (two.Example == null && one.Example != null))
            //{
            //    Program.LogAspectDifference(weight, "Example");
            //    return weight;
            //}
            if (!ComplexElementComparer.Default.Equals(one.Example, two.Example)) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "Example");
            return factor * weight;
        }
    }
}
