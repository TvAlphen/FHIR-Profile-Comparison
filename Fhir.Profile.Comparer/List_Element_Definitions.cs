using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileComparisonMethod
{
    class List_Element_Definitions
    {
        public static List<ElementDefinition> ExistingElements(StructureDefinition profileA)
        {
            var profile1 = profileA.Snapshot.Element;
            var NotExisting1 = profile1.Where(p => p.Max == "0");
            var paths1 = new List<string>();
            foreach (ElementDefinition element in NotExisting1)
            {
                paths1.Add(element.Path);
            }
            var list1 = new List<ElementDefinition>();
            foreach (string pad in paths1)
            {
                var lijst = profile1.Where(p => p.Path.StartsWith(pad)).ToList();
                list1.AddRange(lijst);
            }
            profile1 = profile1.Except(list1).ToList();

            return profile1;

        }

        public static List<ElementDefinition> RemoveSlices(List<ElementDefinition> profileA)
        {
            var list1 = new List<ElementDefinition>();
            var slicepaths = new List<string>();

            slicepaths = profileA.Where(e => e.Slicing != null).Select( e=> e.Path).ToList();
            var sliceInfoList = new List<SliceInfo>();

            foreach( var path in slicepaths )
            {
                var sliceInfo = new SliceInfo();
                sliceInfo.Path = path;
                sliceInfo.Slice = false;
                sliceInfoList.Add(sliceInfo);
            }

            foreach(ElementDefinition element in profileA)
            {
                if (sliceInfoList.Where(i => i.Slice == false).Where(e => PathComparison.ComparePath(e.Path, element.Path)).Any())
                {
                    sliceInfoList.Where(i => PathComparison.ComparePath(i.Path, element.Path)).FirstOrDefault().Slice = true;
                }
                else if (sliceInfoList.Where(i => i.Slice == true).Where(e => PathComparison.ComparePath(e.Path, element.Path)).Any())
                {
                    var Index1Slice = sliceInfoList.Where(i => PathComparison.ComparePath(i.Path, element.Path)).FirstOrDefault().IndexFirstSlice = profileA.IndexOf(element);

                    var lijst = profileA.Where(p => p.Path.StartsWith(element.Path)).ToList();
                    foreach (ElementDefinition e in lijst)
                    {
                        var indexElement = profileA.IndexOf(e);
                        if (indexElement >= Index1Slice)
                        {
                            list1.Add(e);
                        }
                    }
                }
            }
            profileA = profileA.Except(list1).ToList();
            return profileA;
        }

    }

        public class SliceInfo
        {
            public string Path;
            public bool Slice;
            public int IndexFirstSlice;
        } 
}


    

