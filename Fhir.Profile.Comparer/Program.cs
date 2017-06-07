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
    class Program
    {
        //private static double ED;
        private static double ED1;
        //private static List<string> lines = new List<string>();
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

                //var EditDistance = ComparisonTrees(profile1, profile2);
                var EditDist = DistanceTrees(profile1, profile2);
                var profileInfo = new ProfileInfo();
                
                profileInfo.Profiel = profile;
                profileInfo.EDLevenshtein = EditDist;
                //profileInfo.EDdepthfirst = EditDistance;
                ProfileList.Add(profileInfo);
            }
            List<ProfileInfo> SortedProfileList = ProfileList.OrderBy(t => t.EDLevenshtein).ThenBy(t => t.Profiel).ToList();

            foreach(ProfileInfo p in SortedProfileList)
            {
                // + ";" + p.EDdepthfirst.ToString()
                var profiel = p.Profiel + ";" + p.EDLevenshtein.ToString();
                lines4.Add(profiel);
            }
            string path4 = "C:/Users/TessaA/Documents/profilecomparison/ranking.csv";
            File.WriteAllLines(path4, lines4);
//            string path = "C:/Users/TessaA/Documents/profilecomparison/test.txt";
            string path2 = "C:/Users/TessaA/Documents/profilecomparison/test2.txt";
            string path3 = "C:/Users/TessaA/Documents/profilecomparison/test3.csv";
//            File.WriteAllLines(path, lines);
            File.WriteAllLines(path2, lines2);
            File.WriteAllLines(path3, lines3);
        }

        private static double DistanceTrees(StructureDefinition profileA, StructureDefinition profileB)
        {
            ED1 = 0;

            //  elementen met 0..0 verwijderd + kinderen -> hoeven niet vergeleken te worden
            var profile1 = List_Element_Definitions.ExistingElements(profileA);
            var profile2 = List_Element_Definitions.ExistingElements(profileB);

            //  slices verwijderen (en extensies)
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
            
            //log differences: elementen in 1 profiel, niet beide
            LogElementsWithoutCounterPart.Log(profile1, profile2);

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
                    //if (cost > 1 && PathComparison.ComparePath(a.Path,b.Path)) { cost = 1.0; }
                    var MinCost = new List<double>();
                    //delete van profile 1-> cost afhankelijjk van cardinaliteit 
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
                    //insert (delete van profile 2) -> cost afhankelijjk van cardinaliteit counter-part element
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
                    //change
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
                // save d
                lines3.Add(content);
            }
            ED1 = d[m - 1, n - 1];

            //semantic weight: value[x], code, status
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


        //private static double ComparisonTrees(StructureDefinition profileA, StructureDefinition profileB)
        //{
        //    var profile1 = profileA;
        //    var profile2 = profileB;
        //    profile1.Snapshot.Element = List_Element_Definitions.RemoveSlices(profile1.Snapshot.Element);
        //    profile2.Snapshot.Element = List_Element_Definitions.RemoveSlices(profile2.Snapshot.Element);

        //    var navA = ElementDefinitionNavigator.ForSnapshot(profile1);
        //    var navB = ElementDefinitionNavigator.ForSnapshot(profile2);

        //    ED = 0;
        //    // Initally not positioned, nav -> root
        //    if (navA.MoveToFirstChild() && navB.MoveToFirstChild())
        //    {
        //        if (PathComparison.ComparePath(navA.Current.Path, navB.Current.Path))
        //        {
        //            CompareSubtree(navA, navB);
        //        }
        //        else
        //        {
        //            Console.WriteLine("different root/base");
        //            ED = int.MaxValue;
        //        }
        //    }
        //    return ED;
        //}

        //// klopt niet als elementen missen.. worden bomen niet meer gelijk doorlopen!!!!!!!!!!
        //private static void CompareSubtree(ElementDefinitionNavigator nav, ElementDefinitionNavigator nav1)
        //{
        //    bool a = true;
        //    bool b = true;
        //    string path1 = " ";
        //    string path2 = " ";
        //    do
        //    {
        //        if (a && b)
        //        {
        //            // extensies overslaan (+ nog tellen hoeveel, zodat je daar iig nog iets over kan zeggen?) 
        //            //while (nav.Current.Path.EndsWith(".extension") == true | nav.Current.Path.EndsWith(".modifierExtension") == true)
        //            //{
        //            //    nav.MoveToNext();
        //            //}
        //            //while (nav1.Current.Path.EndsWith(".extension") | nav1.Current.Path.EndsWith(".modifierExtension"))
        //            //{
        //            //    nav1.MoveToNext();
        //            //}

        //            // slices overslaan -> bij zelfde path skippen
        //            //while (nav.Current.Path == path1)
        //            //{
        //            //    nav.MoveToNext();
        //            //}
        //            //while(nav1.Current.Path == path2)
        //            //{
        //            //    nav1.MoveToNext();
        //            //}

        //            var distance = CompareAspects.Compare(nav.Current, nav1.Current);
        //            LogDistance(nav.Path, nav1.Path, distance, " ");
        //            ED += distance;
        //            ////////////////////////////////
        //            path1 = nav.Current.Path;
        //            path2 = nav1.Current.Path;

        //            var bm = nav.Bookmark();
        //            var bm1 = nav1.Bookmark();

        //            // Assumption: both structures have the same set of elements
        //            // TODO:
        //            // - Handle extensions
        //            // - Handle slices

        //            // Compare children when cardinality is not 0..0 
        //            if (nav.Current.Max != "0" && nav1.Current.Max != "0")
        //            {
        //                if (nav.MoveToFirstChild() && nav1.MoveToFirstChild())
        //                {

        //                    //if (nav.Current.Path == nav1.Current.Path)
        //                    //{
        //                    CompareSubtree(nav, nav1);
        //                    //}
        //                    // meer elementen zelfde path???? slicing of meer extensies (unordered) -> hoe krijg ik bomen weer gelijk?
        //                    //else
        //                    //{
        //                    //    if (nav.Current.Path.EndsWith(".extension"))
        //                    //    {
        //                    //        ProcessSubtree(nav, nav1, "1");
        //                    //    }
        //                    //    if (nav1.Current.Path.EndsWith(".extension"))
        //                    //    {
        //                    //        ProcessSubtree(nav1, nav, "2");
        //                    //    }
        //                    //    // niet terug naar bookmark nav.current is nu nav1.current
        //                    //    CompareSubtree(nav, nav1);
        //                    //}
        //                }
        //                else
        //                {
        //                    //als ene wel kinderen heeft en ander niet, verspringt ene wel -> allebei terug 
        //                    nav.ReturnToBookmark(bm);
        //                    nav1.ReturnToBookmark(bm1);
        //                }
        //                if (nav.MoveToFirstChild() && !nav1.MoveToFirstChild())
        //                {
        //                    ProcessSubtree1(nav, "1");
        //                }
        //                else
        //                {
        //                    nav.ReturnToBookmark(bm);
        //                    nav1.ReturnToBookmark(bm1);
        //                }
        //                if (nav1.MoveToFirstChild() && !nav.MoveToFirstChild())
        //                {
        //                    ProcessSubtree1(nav1, "2");
        //                }
        //                else
        //                {
        //                    nav.ReturnToBookmark(bm);
        //                    nav1.ReturnToBookmark(bm1);
        //                }

        //                //nodig om terug naar begin van subtree te gaan
        //                nav.ReturnToBookmark(bm);
        //                nav1.ReturnToBookmark(bm1);
        //                a = nav.MoveToNext();
        //                b = nav1.MoveToNext();
        //            }
        //            //  If Max == 0 for 1 profile, then skip child elements - compare children other profile with null
        //            else
        //            {
        //                if (nav.Current.Max == "0" && nav1.Current.Max != "0")
        //                {
        //                    if (nav1.MoveToFirstChild())
        //                    {
        //                        ProcessSubtree1(nav1, "2");
        //                    }
        //                    nav1.ReturnToBookmark(bm1);
        //                    a = nav.MoveToNext();
        //                    b = nav1.MoveToNext();
        //                }
        //                else
        //                {
        //                    if (nav.Current.Max != "0" && nav1.Current.Max == "0")
        //                    {
        //                        if (nav.MoveToFirstChild())
        //                        {
        //                            ProcessSubtree1(nav, "1");
        //                        }
        //                        nav.ReturnToBookmark(bm);
        //                        a = nav.MoveToNext();
        //                        b = nav1.MoveToNext();
        //                    }
        //                    else
        //                    {
        //                        //  If Max == 0 for both profiles, then skip child elements - no meaning, don't compare
        //                        if (nav.Current.Max == "0" && nav1.Current.Max == "0")
        //                        {
        //                            a = nav.MoveToNext();
        //                            b = nav1.MoveToNext();
        //                        }
        //                    }
                            
        //                }
                       
        //            }
        //        }
        //        else
        //        {
        //            if (a)
        //            {
        //                ProcessSubtree1(nav, "1");
        //                a = false;
        //            }
        //            if (b)
        //            {
        //                ProcessSubtree1(nav1, "2");
        //                b = false;
        //            }
        //        }
        //    } while (a | b);
        //}
        //// niet evenveel kinderen, movetonext kan bij 1 niet..vergelijk je 1 element (subtree?) nog een keer met ander child-element, a en b gebruiken?

        ////private static void ProcessSubtree(ElementDefinitionNavigator nav, ElementDefinitionNavigator nav1, string s)
        ////{
        ////    // while (nav.Current.Path != nav1.Current.Path)
        ////    do
        ////    {
        ////        if (nav.Current.Path == nav1.Current.Path) { break; }
                
        ////        var distance = CompareAspects.Compare(nav.Current, null);
        ////        LogDistance(nav.Path, null, distance, s);
        ////        ED += distance;

        ////        var BM = nav.Bookmark();
        ////        if(nav.Current.Max != "0" && nav.MoveToFirstChild())
        ////        {
        ////            ProcessSubtree(nav, nav1, s);
        ////        }
        ////        nav.ReturnToBookmark(BM);
        ////    } while (nav.MoveToNext());
        ////}

        //private static void ProcessSubtree1(ElementDefinitionNavigator nav, string s)
        //{
        //    //var path1 = " ";
        //    do
        //    {
        //        // extensies overslaan (+ tellen hoeveel?) 
        //        //while (nav.Current.Path.EndsWith(".extension") | nav.Current.Path.EndsWith(".modifierExtension") )
        //        //{
        //        //    nav.MoveToNext();
        //        //}
        //        // slices overslaan
        //        //while (nav.Current.Path == path1)
        //        //{
        //        //    var a = nav.MoveToNext();
        //        //    if (!a) { return; }
        //        //}
      
        //        if (nav.Current.Max != "0")
        //        {
        //            var distance = CompareAspects.Compare(nav.Current, null);
        //            LogDistance(nav.Path, null, distance, s);
        //            ED += distance;
        //            //path1 = nav.Current.Path;
        //            var bm = nav.Bookmark();
        //            if (nav.MoveToFirstChild())
        //            {
        //                ProcessSubtree1(nav, s);
        //            }
        //            nav.ReturnToBookmark(bm);
        //        }
                
        //    } while (nav.MoveToNext());
        //}

        //static void LogDistance(string from, string to, double dist, string s)
        //{
        //    if (dist > 0.0)
        //    {
        //        lines.Add($"{s} '{from}' => '{to}' = {dist}");
        //        lines.AddRange(diffs);
        //        diffs.Clear();
        //    }
        //}

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
            public double EDLevenshtein;
            public double EDdepthfirst;
        }
    }
}
