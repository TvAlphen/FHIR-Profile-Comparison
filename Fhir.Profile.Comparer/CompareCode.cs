using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileComparisonMethod
{
    class CompareCode
    {
        public static double DistanceCode(List<Coding> list1, List<Coding> list2, double weight)
        {
            if (!list1.Any() && !list2.Any())
            {
                return 0.0;
            }
            if((list1.Any() && !list2.Any()) || (!list1.Any() && list2.Any()))
            {
                Program.LogAspectDifference(weight, "Code");
                Program.PercentageAspectDifference(100);
                return weight;
            }

            List<Coding> SortedList1 = list1.OrderBy(c => c.System).ThenBy(c => c.Version).ThenBy(c => c.Code).ThenBy(c => c.Display).ThenBy(c => c.UserSelected).ToList();
            List<Coding> SortedList2 = list2.OrderBy(c => c.System).ThenBy(c => c.Version).ThenBy(c => c.Code).ThenBy(c => c.Display).ThenBy(c => c.UserSelected).ToList();
            double maxdistance = Convert.ToDouble(list1.Count + list2.Count);
            //Delete exact similar Codings in both lists 
            var intersection = SortedList1.Intersect(SortedList2, CodeComparer.Default).ToList();
            List<Coding> L1 = SortedList1.Except(intersection, CodeComparer.Default).ToList();
            List<Coding> L2 = SortedList2.Except(intersection, CodeComparer.Default).ToList();
            var L1_ = new List<Coding>();
            var L2_ = new List<Coding>();
            var L = new List<Coding>();
            //userselected differs
            foreach (Coding c in L1)
            {
                var code = L2.Where(e => e.System == c.System && e.Version == c.Version && e.Code == c.Code && e.Display == c.Display).FirstOrDefault();
                if (code != null)
                {
                    L1_.Add(c);
                    L.Add(c);
                    L2_.Add(code);
                    L2.Remove(code);
                }
            }
            L1 = L1.Except(L, CodeComparer.Default).ToList();
            L.Clear();
            //display differs
            foreach (Coding c in L1)
            {
                var code = L2.Where(e => e.System == c.System && e.Version == c.Version && e.Code == c.Code && e.UserSelected == c.UserSelected).FirstOrDefault();
                if (code != null)
                {
                    L1_.Add(c);
                    L.Add(c);
                    L2_.Add(code);
                    L2.Remove(code);
                }
            }
            L1 = L1.Except(L, CodeComparer.Default).ToList();
            L.Clear();
            //userselected & display differs
            foreach (Coding c in L1)
            {
                var code = L2.Where(e => e.System == c.System && e.Version == c.Version && e.Code == c.Code).FirstOrDefault();
                if (code != null)
                {
                    L1_.Add(c);
                    L.Add(c);
                    L2_.Add(code);
                    L2.Remove(code);
                }
            }
            L1 = L1.Except(L, CodeComparer.Default).ToList();
            L.Clear();
            //userselected & display & code differs
            foreach (Coding c in L1)
            {
                var code = L2.Where(e => e.System == c.System && e.Version == c.Version).FirstOrDefault();
                if (code != null)
                {
                    L1_.Add(c);
                    L.Add(c);
                    L2_.Add(code);
                    L2.Remove(code);
                }
            }
            L1 = L1.Except(L, CodeComparer.Default).ToList();
            L.Clear();
            //userselected & display & version differs
            foreach (Coding c in L1)
            {
                var code = L2.Where(e => e.System == c.System && e.Code == c.Code).FirstOrDefault();
                if (code != null)
                {
                    L1_.Add(c);
                    L.Add(c);
                    L2_.Add(code);
                    L2.Remove(code);
                }
            }
            L1 = L1.Except(L, CodeComparer.Default).ToList();
            // L1 en L2 contain codes without matching system and/or version in counter-part --> distance (insert, delete)
            //L1_ en L2_ contain codes that differ only on... -> distance L1_ en L2_ is cost for update operations
            double distance = 0.0;
            for (int i = 0; i < L1_.Count; i++)
            {
                distance += CompareCodes(L1_[i], L2_[i]);
            }

            var m = L1.Count + 1;
            var n = L2.Count + 1;

            // empty lists?
            if (m < 2 && n < 2)
            {
                Program.LogAspectDifference((distance / maxdistance) * weight, "Code");
                Program.PercentageAspectDifference((distance / maxdistance) * 100);
                return (distance / maxdistance) * weight;
            }
            if (m < 2 || n < 2)
            {
                Program.LogAspectDifference((distance + Math.Abs(m - n)) / maxdistance * weight, "Code");
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
                    var a = L1[i - 1];
                    var b = L2[j - 1];
                    // IF not equal, compute cost
                    cost = CompareCodes(a, b);
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
            Program.LogAspectDifference(x, "Code");
            Program.PercentageAspectDifference(((d[m - 1, n - 1] + distance) / maxdistance) * 100);
            return x;
        }

        private static double CompareCodes(Coding code1, Coding code2)
        {
            double factor = 0;
            if (code1.System != code2.System) { return AspectWeights.WEIGHT_CODE_SYSTEM; }
            if (code1.Version != code2.Version) { factor += AspectWeights.WEIGHT_CODE_VERSION; }
            if (code1.Code != code2.Code) { factor += AspectWeights.WEIGHT_CODE_CODE; }
            if (code1.Display != code2.Display) { factor += AspectWeights.WEIGHT_CODE_DISPLAY; }
            if (code1.UserSelected != code2.UserSelected) { factor += AspectWeights.WEIGHT_CODE_USERSELECTED; }

            return 1.0 * factor;
        }
    }
    public class CodeComparer : IEqualityComparer<Coding>
    {
        public static CodeComparer Default { get; } = new CodeComparer();

        public bool Equals(Coding x, Coding y) => (x == null || y == null) ? (y == null && x == null) : x.IsExactly(y);

        public int GetHashCode(Coding obj) => 0;
    }
}
