using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Navigation;
using ProfileComparisonMethod.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProfileComparisonMethod
{
    class LogElementsWithoutCounterPart
    {
        public static void Log(List<ElementDefinition> profile1, List<ElementDefinition> profile2)
        {
            // elements (path) in profile 1 but not in profile 2 -> log
            foreach (ElementDefinition element in profile1)
            {
                if (!profile2.Where(p => PathComparison.ComparePath(p.Path, element.Path)).Any())
                {
                    var cost = 0.0;
                    if (element.Max != "0" && element.Min == 0)
                    {
                        // one optional and two prohibited
                        cost = AspectWeights.PROHIBITED_OPTIONAL;
                        Program.LogDistance2(element.Path, "cost", cost, "1", "optional element");
                    }
                    if (element.Max != "0" && element.Min != 0)
                    {
                        // one mandatory and two prohibited
                        cost = AspectWeights.MANDATORY_PROHIBITED;
                        Program.LogDistance2(element.Path, "cost", cost, "1","mandatory element");
                    }
                }
            }
            foreach (ElementDefinition element in profile2)
            {
                if (!profile1.Where(p => PathComparison.ComparePath(p.Path, element.Path)).Any())
                {
                    var cost = 0.0;
                    if (element.Max != "0" && element.Min == 0)
                    {
                        // one optional and two prohibited
                        cost = AspectWeights.PROHIBITED_OPTIONAL;
                        Program.LogDistance2(element.Path, "cost", cost, "2", "optional element");
                    }
                    if (element.Max != "0" && element.Min != 0)
                    {
                        // one mandatory and two prohibited
                        cost = AspectWeights.MANDATORY_PROHIBITED;
                        Program.LogDistance2(element.Path, "cost", cost, "2", "mandatory element");

                    }
                }
            }
        }
     

    }
}
