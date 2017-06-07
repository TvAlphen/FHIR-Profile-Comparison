using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PogingOmIetsTeVergelijken4
{
    class PathComparison
    {
        public static Boolean ComparePath(string one, string two)
        {
            
            if (one == two)
            {
                return true;
            }
            else if (one.Split('.').Last().StartsWith("value") && two.Split('.').Last().StartsWith("value"))
            {
                if(one.Split('.').Count() == two.Split('.').Count())
                {
                    var path1 = one.Substring(0, one.IndexOf("value") + "value".Length);
                    var path2 = two.Substring(0, two.IndexOf("value") + "value".Length);
                    if (path1 == path2)
                    {
                        return true;
                    }
                }
            }
            else if (one.Split('.').Last().StartsWith("effective") && two.Split('.').Last().StartsWith("effective"))
            {
                if (one.Split('.').Count() == two.Split('.').Count())
                {
                    var path1 = one.Substring(0, one.IndexOf("effective") + "effective".Length);
                    var path2 = two.Substring(0, two.IndexOf("effective") + "effective".Length);
                    if (path1 == path2)
                    {
                        return true;
                    }
                }
            }

            //value[x] en effective[x] in observation.. voor andere resources andere toevoegen?
            //else if(one.Split('.').Last().StartsWith("value") && two.Split('.').Last().StartsWith("value") && one.Split('.').Count() == two.Split('.').Count())
            //{
            //    return true;
            //}
            //else if (one.Split('.').Last().StartsWith("effective") && two.Split('.').Last().StartsWith("effective") && one.Split('.').Count() == two.Split('.').Count())
            //{
            //    return true;
            //}
            return false;
        }
    }
}
