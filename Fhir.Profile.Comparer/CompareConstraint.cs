using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileComparisonMethod
{
    class CompareConstraint
    {
        public static double DistanceConstraint(List<ElementDefinition.ConstraintComponent> list1, List<ElementDefinition.ConstraintComponent> list2, double weight)
        {
            if (!list1.Any() && !list2.Any())
            {
                return 0.0;
            }
            if ((list1.Any() && !list2.Any()) || (!list1.Any() && list2.Any()))
            {
                Program.LogAspectDifference(weight, "Constraint");
                Program.PercentageAspectDifference(100);
                return weight;
            }

            List<ElementDefinition.ConstraintComponent> SortedList1 = list1.OrderBy(c => c.Xpath).ThenBy(c => c.Severity).ThenBy(c => c.Key).ThenBy(c => c.Human).ThenBy(c => c.Requirements).ToList();
            List<ElementDefinition.ConstraintComponent> SortedList2 = list2.OrderBy(c => c.Xpath).ThenBy(c => c.Severity).ThenBy(c => c.Key).ThenBy(c => c.Human).ThenBy(c => c.Requirements).ToList();
            double maxdistance = Convert.ToDouble(list1.Count + list2.Count);
            //delete exact similar constraints in both lists 
            var intersection = SortedList1.Intersect(SortedList2, ConstraintComparer.Default).ToList();
            List<ElementDefinition.ConstraintComponent> L1 = SortedList1.Except(intersection, ConstraintComparer.Default).ToList();
            List<ElementDefinition.ConstraintComponent> L2 = SortedList2.Except(intersection, ConstraintComparer.Default).ToList();
            var L1_ = new List<ElementDefinition.ConstraintComponent>();
            var L2_ = new List<ElementDefinition.ConstraintComponent>();
            var L = new List<ElementDefinition.ConstraintComponent>();
            //key differs
            foreach (ElementDefinition.ConstraintComponent c in L1)
            {
                var constraint = L2.Where(e => e.Xpath == c.Xpath && e.Human == c.Human && e.Severity == c.Severity && e.Requirements == c.Requirements).FirstOrDefault();
                if (constraint != null)
                {
                    L1_.Add(c);
                    L.Add(c);
                    L2_.Add(constraint);
                    L2.Remove(constraint);
                }
            }
            L1 = L1.Except(L, ConstraintComparer.Default).ToList();
            L.Clear();
            //(key &) requirements differs
            foreach (ElementDefinition.ConstraintComponent c in L1)
            {
                var constraint = L2.Where(e => e.Xpath == c.Xpath && e.Human == c.Human && e.Severity == c.Severity).FirstOrDefault();
                if (constraint != null)
                {
                    L1_.Add(c);
                    L.Add(c);
                    L2_.Add(constraint);
                    L2.Remove(constraint);
                }
            }
            L1 = L1.Except(L, ConstraintComparer.Default).ToList();
            L.Clear();
            //(key & requirements &) severity differs
            foreach (ElementDefinition.ConstraintComponent c in L1)
            {
                var constraint = L2.Where(e => e.Xpath == c.Xpath && e.Human == c.Human).FirstOrDefault();
                if (constraint != null)
                {
                    L1_.Add(c);
                    L.Add(c);
                    L2_.Add(constraint);
                    L2.Remove(constraint);
                }
            }
            L1 = L1.Except(L, ConstraintComparer.Default).ToList();
            L.Clear();
            //(key & requirements &) human discription differs
            foreach (ElementDefinition.ConstraintComponent c in L1)
            {
                var constraint = L2.Where(e => e.Xpath == c.Xpath && e.Severity == c.Severity).FirstOrDefault();
                if (constraint != null)
                {
                    L1_.Add(c);
                    L.Add(c);
                    L2_.Add(constraint);
                    L2.Remove(constraint);
                }
            }
            L1 = L1.Except(L, ConstraintComparer.Default).ToList();
            L.Clear();
            // L1 en L2 contain constraints without matching xpath and/or severity/human in counter-part -->  distance (delete, insert)
            //L1_ en L2_ contain codes that differ only on... -> distance L1_ en L2_ is cost for update operations
            double distance = 0.0;
            for (int i = 0; i < L1_.Count; i++)
            {
                distance += CompareConstraints(L1_[i], L2_[i]);
            }

            var m = L1.Count + 1;
            var n = L2.Count + 1;

            // empty lists?
            if (m < 2 && n < 2)
            {
                Program.LogAspectDifference((distance / maxdistance) * weight, "Constraint");
                Program.PercentageAspectDifference((distance / maxdistance) * 100);
                return (distance / maxdistance) * weight;
            }
            if (m < 2 || n < 2)
            {
                Program.LogAspectDifference((distance + Math.Abs(m - n)) / maxdistance * weight, "Constraint");
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

                    cost = CompareConstraints(a, b);
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
            Program.LogAspectDifference(x, "Constraint");
            Program.PercentageAspectDifference(((d[m - 1, n - 1] + distance) / maxdistance) * 100);
            return x;
        }

        private static double CompareConstraints(ElementDefinition.ConstraintComponent constraint1, ElementDefinition.ConstraintComponent constraint2)
        {
            double factor = 0;
            if (constraint1.Xpath != constraint2.Xpath) { return AspectWeights.WEIGHT_CONSTRAINT_XPATH; }
            if (constraint1.Human != constraint2.Human) { factor += AspectWeights.WEIGHT_CONSTRAINT_HUMAN; }
            if (constraint1.Severity != constraint2.Severity) { factor += AspectWeights.WEIGHT_CONSTRAINT_SEVERITY; }
            if (constraint1.Key != constraint2.Key) { factor += AspectWeights.WEIGHT_CONSTRAINT_KEY; }
           
            if (constraint1.Requirements != constraint2.Requirements) { factor += AspectWeights.WEIGHT_CONSTRAINT_REQUIREMENTS; }

            return 1.0 * factor;
        }
    }
    public class ConstraintComparer : IEqualityComparer<ElementDefinition.ConstraintComponent>
    {
        public static ConstraintComparer Default { get; } = new ConstraintComparer();

        public bool Equals(ElementDefinition.ConstraintComponent x, ElementDefinition.ConstraintComponent y) => (x == null || y == null) ? (y == null && x == null) : x.IsExactly(y);

        public int GetHashCode(ElementDefinition.ConstraintComponent obj) => 0;
    }
}
