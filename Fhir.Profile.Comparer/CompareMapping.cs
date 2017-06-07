using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileComparisonMethod
{
    class CompareMapping
    {
        public static double DistanceMapping(List<ElementDefinition.MappingComponent> list1, List<ElementDefinition.MappingComponent> list2, double weight)
        {
            if (!list1.Any() && !list2.Any())
            {
                return 0.0;
            }
            if ((list1.Any() && !list2.Any()) || (!list1.Any() && list2.Any()))
            {
                Program.LogAspectDifference(weight, "Mapping");
                Program.PercentageAspectDifference(100);
                return weight;
            }

            List<ElementDefinition.MappingComponent> SortedList1 = list1.OrderBy(map => map.Identity).ThenBy(map => map.Map).ThenBy(map => map.Language).ToList();
            List<ElementDefinition.MappingComponent> SortedList2 = list2.OrderBy(map => map.Identity).ThenBy(map => map.Map).ThenBy(map => map.Language).ToList();
            double maxdistance = Convert.ToDouble(list1.Count + list2.Count);
            //delete exact similar mappings in both lists
            var intersection = SortedList1.Intersect(SortedList2, MappingComparer.Default).ToList();
            List<ElementDefinition.MappingComponent> L1 = SortedList1.Except(intersection, MappingComparer.Default).ToList();
            List<ElementDefinition.MappingComponent> L2 = SortedList2.Except(intersection, MappingComparer.Default).ToList();
            var L1_ = new List<ElementDefinition.MappingComponent>();
            var L2_ = new List<ElementDefinition.MappingComponent>();
            foreach (ElementDefinition.MappingComponent ma in L1)
            {
                var mapping = L2.Where(e => e.Identity == ma.Identity && e.Map == ma.Map).FirstOrDefault();
                if (mapping != null)
                {
                    L1_.Add(ma);
                    L2_.Add(mapping);
                    L2.Remove(mapping);
                }
            }
            L1 = L1.Except(L1_, MappingComparer.Default).ToList();
            // L1 en L2 contain mappings without matching identity and/or map in counter-part -->  distance (insert, delete)
            //L1_ en L2_ contain mappings that differ only on language -> distance L1_ en L2 is cost for updating language of each mapping in one list
            double distance = 0.0;
            for (int i = 0; i < L1_.Count; i++)
            {
                distance += CompareMappings(L1_[i], L2_[i]);
            }

            var m = L1.Count + 1;
            var n = L2.Count + 1;

            // empty list?
            if (m < 2 && n < 2)
            {
                Program.LogAspectDifference((distance / maxdistance) * weight, "Mapping");
                Program.PercentageAspectDifference((distance / maxdistance) * 100);
                return (distance / maxdistance) * weight;
            }
            if (m < 2 || n < 2)
            {
                Program.LogAspectDifference((distance + Math.Abs(m - n)) / maxdistance * weight, "Mapping");
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

                    cost = CompareMappings(a, b);
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
            Program.LogAspectDifference(x, "Mapping");
            Program.PercentageAspectDifference(((d[m - 1, n - 1] + distance) / maxdistance) * 100);
            return x;
        }

        private static double CompareMappings(ElementDefinition.MappingComponent map1, ElementDefinition.MappingComponent map2)
        {
            double factor = 0;
            if (map1.Identity != map2.Identity) { return AspectWeights.WEIGHT_MAPPING_IDENTITY; }
            if (map1.Language != map2.Language) { factor += AspectWeights.WEIGHT_MAPPING_LANGUAGE; }
            if (map1.Map != map2.Map) { factor += AspectWeights.WEIGHT_MAPPING_MAP; }

            return 1.0 * factor;
        }
    }
    public class MappingComparer : IEqualityComparer<ElementDefinition.MappingComponent>
    {
        public static MappingComparer Default { get; } = new MappingComparer();

        public bool Equals(ElementDefinition.MappingComponent x, ElementDefinition.MappingComponent y) => (x == null || y == null) ? (y == null && x == null) : x.IsExactly(y);

        public int GetHashCode(ElementDefinition.MappingComponent obj) => 0;
    }
}
