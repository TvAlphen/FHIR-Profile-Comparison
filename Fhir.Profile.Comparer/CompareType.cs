using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileComparisonMethod
{
    public static class CompareType
    {
        // return is percentage van max distance (everything differs)
        public static double DistanceType(List<ElementDefinition.TypeRefComponent> list1, List<ElementDefinition.TypeRefComponent> list2, double weight)
        {
            if (list1.Count == 0 && list2.Count == 0)
            {
                return 0.0;
            }
            if (list1.Count == 0 || list2.Count == 0)
            {
                Program.LogAspectDifference(weight, "Type");
                Program.PercentageAspectDifference(100);
                return weight;
            }
            // STU3: Only one profile per typeref component
            // DSTU2: let's ignore multiple profiles per type
            List<ElementDefinition.TypeRefComponent> SortedList1 = list1.OrderBy(t => t.Code).ThenBy(t => t.Profile.FirstOrDefault()).ThenBy(t => AggregationSortKey(t.Aggregation)).ToList();
            List<ElementDefinition.TypeRefComponent> SortedList2 = list2.OrderBy(t => t.Code).ThenBy(t => t.Profile.FirstOrDefault()).ThenBy(t => AggregationSortKey(t.Aggregation)).ToList();
            
            // max distance between lists
            double maxdistance = Convert.ToDouble(list1.Count + list2.Count);
            //delete exact similar types in both lists
            var intersection = SortedList1.Intersect(SortedList2, TypeComparer.Default).ToList();
            List<ElementDefinition.TypeRefComponent> L1 = SortedList1.Except(intersection, TypeComparer.Default).ToList();
            List<ElementDefinition.TypeRefComponent> L2 = SortedList2.Except(intersection, TypeComparer.Default).ToList();
            var L1_ = new List<ElementDefinition.TypeRefComponent>();
            var L2_ = new List<ElementDefinition.TypeRefComponent>();
            foreach (ElementDefinition.TypeRefComponent t in L1)
            {
                var type = L2.Where(e => e.Code == t.Code && e.Profile == t.Profile).FirstOrDefault();
                if(type != null)
                {
                    L1_.Add(t);
                    L2_.Add(type);
                    L2.Remove(type);
                }
            }
            L1 = L1.Except(L1_, TypeComparer.Default).ToList();
            // L1 en L2 contain types without matching code and/or profile in counter-part -->  distance (delete / insert)
            //L1_ en L2_ contain types that differ only on aggregation -> distance L1_ en L2 is cost for update aggregation of each type in one list
            double distance = 0.0;
            for (int i = 0; i < L1_.Count; i++)
            {
                distance += CompareTypes(L1_[i], L2_[i]);
            }

            var m = L1.Count + 1;
            var n = L2.Count + 1;

            // empty list?
            if (m <2 && n < 2)
            {
                Program.LogAspectDifference((distance / maxdistance) * weight, "Type");
                Program.PercentageAspectDifference((distance / maxdistance) * 100);
                return (distance/maxdistance) * weight;
            }
            if (m<2 || n < 2)
            {
                Program.LogAspectDifference((distance + Math.Abs(m - n)) / maxdistance * weight, "Type");
                Program.PercentageAspectDifference((distance + Math.Abs(m - n)) / maxdistance * 100);
                return (distance + Math.Abs(m - n)) / maxdistance * weight;
            }

            var d = new double[m, n];

            for (int i = 0; i < m; i++)
            {
                d[i, 0] = i;
            }
            for (int j = 0; j < n; j++)
            {
                d[0, j] = j;
            }

            for (int j = 1; j < n; j++)
            {
                for (int i = 1; i < m; i++)
                {
                    // IF equal cost = 0
                    double cost = 0.0;
                    // IF not equal, compute cost
                    var a = L1[i - 1];
                    var b = L2[j - 1];

                    cost = CompareTypes(a, b);
                    var MinCost = new List<double>();
                    //delete
                    MinCost.Add(d[i - 1, j] + 1);
                    //insert
                    MinCost.Add(d[i, j - 1] + 1);
                    //change
                    if (cost < 1) MinCost.Add(d[i - 1, j - 1] + cost);
                    d[i, j] = MinCost.Min();
                }
            }
            var x = ((d[m - 1, n - 1] + distance) / maxdistance) * weight;
            Program.LogAspectDifference(x, "Type");
            Program.PercentageAspectDifference(((d[m - 1, n - 1] + distance) / maxdistance) * 100);
            return x;
        }

        private static double CompareTypes(ElementDefinition.TypeRefComponent type1, ElementDefinition.TypeRefComponent type2)
        {
            double factor = 0;
            // Different Code -> max distance
            if (type1.Code != type2.Code) { return AspectWeights.WEIGHT_TYPE_CODE; }
            if (type1.Profile.FirstOrDefault() != type2.Profile.FirstOrDefault()) { factor += AspectWeights.WEIGHT_TYPE_PROFILE; }
            if (AggregationSortKey(type1.Aggregation) != AggregationSortKey(type2.Aggregation)) { factor += AspectWeights.WEIGHT_TYPE_AGGREGATION; }

            return 1.0 * factor;
        }

        private static int AggregationSortKey(IEnumerable<ElementDefinition.AggregationMode?> modes)
        {
            int result = 0;
            if (modes.Contains(ElementDefinition.AggregationMode.Contained)) result += 1;
            if (modes.Contains(ElementDefinition.AggregationMode.Referenced)) result += 2;
            if (modes.Contains(ElementDefinition.AggregationMode.Bundled)) result += 4;
            return result;
        }

    }
    public class TypeComparer : IEqualityComparer<ElementDefinition.TypeRefComponent>
    {
        public static TypeComparer Default { get; } = new TypeComparer();

        public bool Equals(ElementDefinition.TypeRefComponent x, ElementDefinition.TypeRefComponent y) => (x == null || y == null) ? (y == null && x == null) : x.IsExactly(y);

        public int GetHashCode(ElementDefinition.TypeRefComponent obj) => 0;
    }
}
