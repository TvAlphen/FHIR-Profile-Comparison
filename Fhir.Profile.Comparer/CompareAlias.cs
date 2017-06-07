using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PogingOmIetsTeVergelijken4
{
    static class CompareAlias
    {
        public static double DistanceAlias(IEnumerable<string> list1, IEnumerable<string> list2, double weight)
        {
            // lege lijst vergelijken met levenstein...geeft uitkomst NAN, 
            if (!list1.Any() && !list2.Any())
            {
                return 0.0;
            }
            if ((list1.Any() && !list2.Any()) || (!list1.Any() && list2.Any()))
            {
                Program.LogAspectDifference(weight, "Alias");
                Program.PercentageAspectDifference(100);
                return weight;
            }

            List<string> SortedList1 = list1.OrderBy(a => a).ToList();
            List<string> SortedList2 = list2.OrderBy(a => a).ToList();

            var m = SortedList1.Count + 1;
            var n = SortedList2.Count + 1;
            double maxdistance = Convert.ToDouble(SortedList1.Count + SortedList2.Count);
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
                    var a = SortedList1[i - 1];
                    var b = SortedList2[j - 1];
                    // IF not equal, compute cost
                    if (a != b)
                    {
                        cost = 1.0;
                    }
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
            var x = (d[m - 1, n - 1] / maxdistance) * weight;
            Program.LogAspectDifference(x, "Alias");
            Program.PercentageAspectDifference((d[m - 1, n - 1] / maxdistance) * 100);
            return (d[m - 1, n - 1] / maxdistance) * weight;
        }
    }
}


