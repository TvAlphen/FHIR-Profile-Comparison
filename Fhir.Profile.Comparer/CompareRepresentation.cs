using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileComparisonMethod
{
    class CompareRepresentation
    {
        public static double DistanceRepresentation(IEnumerable<ElementDefinition.PropertyRepresentation?> list1, IEnumerable<ElementDefinition.PropertyRepresentation?> list2, double weight)
        {
            if (!list1.Any() && !list2.Any())
            { 
                return 0.0;
            }
            if ((list1.Any() && !list2.Any()) || (!list1.Any() && list2.Any()))
            {
                Program.LogAspectDifference(weight, "Representation");
                Program.PercentageAspectDifference(100);
                return weight;
            }

            //Removed all "nulls" and casted to non-nullable enum.
            var Enum1 = list1.Where(p => p.HasValue).Select(p => p.Value);
            var Enum2 = list2.Where(p => p.HasValue).Select(p => p.Value);

            //check for empty list
            if (!Enum1.Any() && !Enum2.Any())
            {
                return 0.0;
            }
            if ((Enum1.Any() && !Enum2.Any()) || (!Enum1.Any() && Enum2.Any()))
            {
                Program.LogAspectDifference(weight, "Representation");
                Program.PercentageAspectDifference(100);
                return weight;
            }

            var SortedList1 = Enum1.OrderBy(r => r).ToList();
            var SortedList2 = Enum2.OrderBy(r => r).ToList();

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
            Program.LogAspectDifference(x, "Representation");
            Program.PercentageAspectDifference((d[m - 1, n - 1] / maxdistance)*100);
            return (d[m - 1, n - 1] / maxdistance) * weight;
            
           
            
        }
    }
}
