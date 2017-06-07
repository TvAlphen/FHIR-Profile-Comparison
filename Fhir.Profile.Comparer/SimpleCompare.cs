using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileComparisonMethod
{
    public static class SimpleCompare
    {
        public static double CompareNameReference(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0; 
            if (one.NameReference != two.NameReference) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "NameReference");
            return factor * weight;
        }

        public static double CompareMustSupport(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            if (one.MustSupport != two.MustSupport) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "MustSupport");
            return factor * weight;
        }

        public static double CompareIsModifier(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            if (one.IsModifier != two.IsModifier) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "IsModifier");
            return factor * weight;
        }

        public static double CompareIsSummary(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            if (one.IsSummary != two.IsSummary) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "IsSummary");
            return factor * weight;
        }

        public static double CompareRequirements(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            if (one.Requirements != two.Requirements) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "Requirements");
            return factor * weight;
        }

        public static double CompareComments(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            if (one.Comments != two.Comments) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "Comments");
            return factor * weight;
        }

        public static double CompareDefinition(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            if (one.Definition != two.Definition) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "Definition");
            return factor * weight;
        }

        public static double CompareShort(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            if (one.Short != two.Short) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "Short");
            return factor * weight;
        }
        public static double CompareLabel(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            if (one.Label != two.Label) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "Label");
            return factor * weight;
        }

        public static double CompareName(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            if (one.Name != two.Name) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "Name");
            return factor * weight;
        }

        public static double CompareMeaningWhenMissing(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            if (one.MeaningWhenMissing != two.MeaningWhenMissing) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "MeaningWhenMissing");
            return factor * weight;
        }

        public static double CompareMaxLength(ElementDefinition one, ElementDefinition two, double weight)
        {
            double factor = 0.0;
            if (one.MaxLength != two.MaxLength) factor = 1.0;
            Program.LogAspectDifference((factor * weight), "MaxLength");
            return factor * weight;
        }
    }
}
