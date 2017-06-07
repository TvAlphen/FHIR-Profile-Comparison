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
    class Program
    {
        private static double ED1;
        private static List<string> lines2 = new List<string>();
        private static List<string> lines3 = new List<string>();
        private static List<string> diffs = new List<string>();
        private static List<string> lines4 = new List<string>();

        static void Main(string[] args)
        {
            var parser = new FhirXmlParser();
            var ProfileList = new List<ProfileInfo>();
            
            for(int i =1; i <= 24; i++)
            {
                var name = "Profile_";
                var profile = name+ i.ToString();
                var p = Resources.ResourceManager.GetObject(profile).ToString();
                var profile1 = parser.Parse<StructureDefinition>(Resources.ComparisonProfile);
                var profile2 = parser.Parse<StructureDefinition>(p);
                // compute distance between profiles
                var EditDist = DistanceTrees(profile1, profile2);
                var profileInfo = new ProfileInfo();
                
                profileInfo.Profiel = profile;
                profileInfo.ED = EditDist;
                ProfileList.Add(profileInfo);
            }
            List<ProfileInfo> SortedProfileList = ProfileList.OrderBy(t => t.ED).ThenBy(t => t.Profiel).ToList();

            foreach(ProfileInfo p in SortedProfileList)
            {
                var profiel = p.Profiel + ";" + p.ED.ToString();
                lines4.Add(profiel);
            }
            string path4 = "C:/Users/TessaA/Documents/profilecomparison/ranking.csv";
            // file with a ranking of the profile set
            File.WriteAllLines(path4, lines4);
            string path2 = "C:/Users/TessaA/Documents/profilecomparison/test2.txt";
            string path3 = "C:/Users/TessaA/Documents/profilecomparison/test3.csv";
            // file with log of all differences between profiles
            File.WriteAllLines(path2, lines2);
            // file with edit distance matrix between two profiles
            File.WriteAllLines(path3, lines3);
        }

        private static double DistanceTrees(StructureDefinition profileA, StructureDefinition profileB)
        {
            ED1 = 0;

            //  delete all prohibited elements and children-elements
            var profile1 = List_Element_Definitions.ExistingElements(profileA);
            var profile2 = List_Element_Definitions.ExistingElements(profileB);

            //  delete slices and extensions
            profile1 = List_Element_Definitions.RemoveSlices(profile1);
            profile2 = List_Element_Definitions.RemoveSlices(profile2);

            if (profile1.Count == 0 && profile2.Count == 0)
            {
                return 0.0;
            }

            //different concept
            if (profile1[0].Path != profile2[0].Path)
            {
                return int.MaxValue;
            }

            //log elements that only exists in one profile
            LogElementsWithoutCounterPart.Log(profile1, profile2);

            // Wagner-Fischer algorithm applied to profiles
            var m = profile1.Count + 1;
            var n = profile2.Count + 1;
            var d = new double[m, n];

            for (int i = 0; i < m; i++)
            {
                d[i, 0] = i;
            }
            for (int j = 0; j < n; j++)
            {
                d[0, j] = j;
            }

            // first row of results matrix, which indicates the elements of profile 1
            var elementen = " " + ";" ;
            foreach (ElementDefinition element in profile1)
            {
                elementen += element.Path + ";";
            }
            lines3.Add(elementen);

            for (int j = 1; j < n; j++)
            {
                string content = profile2[j-1].Path + ";";
                for (int i = 1; i < m; i++)
                {
                    double cost = 0.0;
                    // IF not equal, compute cost
                    var a = profile1[i - 1];
                    var b = profile2[j - 1];
                    cost = CompareAspects.Compare(a, b);
                    var MinCost = new List<double>();
                    // cost depends on cardinality of deleted element 
                    var costdelete = 1.0;
                    if (a.Max != "0" && a.Min == 0)
                    {
                        costdelete = AspectWeights.PROHIBITED_OPTIONAL;
                    }
                    if (a.Max != "0" && a.Min != 0)
                    {
                        costdelete = AspectWeights.MANDATORY_PROHIBITED;
                    }
                    MinCost.Add(d[i - 1, j] + costdelete);
                    //cost depends on cardinality of inserted element
                    var costinsert = 1.0;
                    if (b.Max != "0" && b.Min == 0)
                    {
                        costinsert = AspectWeights.PROHIBITED_OPTIONAL;
                    }
                    if (b.Max != "0" && b.Min != 0)
                    {
                        costinsert = AspectWeights.MANDATORY_PROHIBITED;
                    }
                    MinCost.Add(d[i, j - 1] + costinsert);
                    //change or update
                    if (cost <= 1 && PathComparison.ComparePath(a.Path, b.Path))
                    {
                        MinCost.Add(d[i - 1, j - 1] + cost);
                    }
                    d[i, j] = MinCost.Min();
                    
                    // log differences between elements
                    if (cost > 0 && PathComparison.ComparePath(a.Path, b.Path))
                    {
                        LogDistance2(a.Path, b.Path, cost, "", "");
                    }
                    content += d[i, j].ToString() + ";";
                }
                // save matrix
                lines3.Add(content);
            }
            ED1 = d[m - 1, n - 1];

            //additional semantic weight for the elements: value[x], code, status
            var c1 = CompareAspects.Compare(profile1.Where(p => p.Path == "Observation.status").FirstOrDefault(), profile2.Where(p => p.Path == "Observation.status").FirstOrDefault());
            if (c1 > 0)
            {
                ED1 += c1 * AspectWeights.ELEMENT_STATUS;
            }
            var c2 = CompareAspects.Compare(profile1.Where(p => p.Path == "Observation.code").FirstOrDefault(), profile2.Where(p => p.Path == "Observation.code").FirstOrDefault());
            if (c2 > 0)
            {
                ED1 += c2 * AspectWeights.ELEMENT_CODE;
            }
            var c3 = CompareAspects.Compare(profile1.Where(p => p.Path == "Observation.code.coding").FirstOrDefault(), profile2.Where(p => p.Path == "Observation.code.coding").FirstOrDefault());
            if (c3 > 0)
            {
                ED1 += c3 * AspectWeights.ELEMENT_CODE_CODING;
            }
            var c4 = CompareAspects.Compare(profile1.Where(p => p.Path == "Observation.code.coding.system").FirstOrDefault(), profile2.Where(p => p.Path == "Observation.code.coding.system").FirstOrDefault());
            if (c4 > 0)
            {
                ED1 += c4 * AspectWeights.ELEMENT_CODE_CODING_SYSTEM;
            }
            var c5 = CompareAspects.Compare(profile1.Where(p => p.Path == "Observation.code.coding.code").FirstOrDefault(), profile2.Where(p => p.Path == "Observation.code.coding.code").FirstOrDefault());
            if (c5 > 0)
            {
                ED1 += c5 * AspectWeights.ELEMENT_CODE_CODING_CODE;
            }
            var c6 = CompareAspects.Compare(profile1.Where(p => p.Path.StartsWith("Observation.value")).FirstOrDefault(), profile2.Where(p => p.Path.StartsWith("Observation.value")).FirstOrDefault());
            if (c6 > 0)
            {
                ED1 += c6 * AspectWeights.ELEMENT_CODE_CODING_SYSTEM;
            }
            return ED1;
        }


        public static void LogDistance2(string from, string to, double dist, string s, string card)
        {
            if (dist > 0.0)
            {
                lines2.Add($" {s} '{from}' => '{to}' = {dist}  {card}");
                lines2.AddRange(diffs);
                diffs.Clear();
            }
        }

        public static void LogAspectDifference(double dist, string aspect)
        {
            if (dist > 0.0)
            {
                diffs.Add($"      {aspect} =>  distance = {dist}");
            }
        }

        public static void PercentageAspectDifference(double dist)
        {
            if (dist > 0.0)
            {
                diffs.Add($"         difference percentage = {dist} %");
            }
        }
        
        public class ProfileInfo
        {
            public string Profiel;
            public double ED;
        }
    }
}
