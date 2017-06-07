using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Navigation;
using PogingOmIetsTeVergelijken4.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PogingOmIetsTeVergelijken4
{
    class LogElementsWithoutCounterPart
    {
        public static void Log(List<ElementDefinition> profile1, List<ElementDefinition> profile2)
        {
            //////////checken welke elementen (path) in profiel 1 zitten en niet in 2 -> voor log bestand////////////////////
            foreach (ElementDefinition element in profile1)
            {
                // als slices verwijderd zijn is er van elk pad maar 1 element, behalve bij meer extensies..
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
                // als slices verwijderd zijn is er van elk pad maar 1 element, behalve bij meer extensies..
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
