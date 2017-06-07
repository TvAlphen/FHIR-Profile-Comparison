using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileComparisonMethod
{
    public static class CompareAspects
    {
        public static double Compare(ElementDefinition one, ElementDefinition two)
        {

            double difference = 0;
            // cost depends on difference in cardinality between elements
            if (two == null)
            {
               if(StringToInteger.MaxInt(one) == 0)
                {
                    // both prohibited
                    Program.LogAspectDifference(AspectWeights.BOTH_PROHIBITED, "Cardinality");
                    return AspectWeights.BOTH_PROHIBITED;
                }
               if(StringToInteger.MaxInt(one) != 0 && one.Min == 0)
                {
                    // one optional and two prohibited
                    Program.LogAspectDifference(AspectWeights.PROHIBITED_OPTIONAL, "Cardinality");
                    return AspectWeights.PROHIBITED_OPTIONAL;
                }
                if (StringToInteger.MaxInt(one) != 0 && one.Min != 0)
                {
                    // one mandatory and two prohibited
                    Program.LogAspectDifference(AspectWeights.MANDATORY_PROHIBITED, "Cardinality");
                    return AspectWeights.MANDATORY_PROHIBITED;
                }
            }
            if (!PathComparison.ComparePath(one.Path, two.Path))
            {
                return AspectWeights.PATH;
            }

            //if cardinality is not defined in one element -> max difference (same as no overlap not 1) and compare other aspects!
            if (one.Max == null && one.Min == null && two.Max != null && two.Min != null) difference += AspectWeights.BOTH_MANDATORY_NO_OVERLAP;
            if (two.Max == null && two.Min == null && one.Max != null && one.Min != null) difference += AspectWeights.BOTH_MANDATORY_NO_OVERLAP;
            if ((one.Max == null && one.Min != null && two.Max != null && two.Min != null) || (one.Max != null && one.Min != null && two.Max == null && two.Min != null))
            {
                if (one.Min == two.Min) difference += 0.5 * AspectWeights.BOTH_MANDATORY_NO_OVERLAP;
                else difference += AspectWeights.BOTH_MANDATORY_NO_OVERLAP;
            }
            if ((one.Max != null && one.Min == null && two.Max != null && two.Min != null) || (one.Max != null && one.Min != null && two.Max != null && two.Min == null))
            {
                if (one.Max == two.Max) difference += 0.5 * AspectWeights.BOTH_MANDATORY_NO_OVERLAP;
                else difference += AspectWeights.BOTH_MANDATORY_NO_OVERLAP;
            }
            if ((one.Max == null && one.Min != null && two.Max != null && two.Min == null) || (one.Max != null && one.Min == null && two.Max == null && two.Min != null))
            {
                difference += AspectWeights.BOTH_MANDATORY_NO_OVERLAP;
            }

            if (one.Max != null && one.Min != null && two.Max != null && two.Min != null)
            {
                // both prohibited -> skip other aspects, difference no meaning 
                if (one.Min == 0 && two.Min == 0 && StringToInteger.MaxInt(one) == 0 && StringToInteger.MaxInt(two) == 0)
                {
                    Program.LogAspectDifference(AspectWeights.BOTH_PROHIBITED, "Cardinality");
                    return AspectWeights.BOTH_PROHIBITED;
                }
                if ((StringToInteger.MaxInt(one) == 0 && two == null) || (StringToInteger.MaxInt(two) == 0 && one == null))
                {
                    Program.LogAspectDifference(AspectWeights.BOTH_PROHIBITED, "Cardinality");
                    return AspectWeights.BOTH_PROHIBITED;
                }
                // 1 prohibited and 1 mandatory
                if ((one.Min == 0 && one.Max == "0" && two.Min != 0) || (two.Min == 0 && two.Max == "0" && one.Min != 0))
                {
                    Program.LogAspectDifference(AspectWeights.MANDATORY_PROHIBITED, "Cardinality");
                    return AspectWeights.MANDATORY_PROHIBITED;
                }
                // 1 prohibited en 1 optional
                if ((one.Min == two.Min && one.Max == "0" && two.Max != "0") || (two.Min == one.Min && two.Max == "0" && one.Max != "0"))
                {
                    Program.LogAspectDifference(AspectWeights.PROHIBITED_OPTIONAL, "Cardinality");
                    return AspectWeights.PROHIBITED_OPTIONAL;
                }

                // ranges cardinality, remaining options 
                difference += CompareCardinality.CompareRangesCardinality(one, two);
            }
            // Note: API guarantees that list elements (such as ElementDefinition.Type) are never null !
            var distanceType = CompareType.DistanceType(one.Type, two.Type, AspectWeights.WEIGHT_TYPE);
            difference += distanceType;

            difference += SimpleCompare.CompareNameReference(one, two, AspectWeights.WEIGHT_NAMEREFERENCE);
            difference += SimpleCompare.CompareMustSupport(one, two, AspectWeights.WEIGHT_MUSTSUPPORT);
            difference += SimpleCompare.CompareIsModifier(one, two, AspectWeights.WEIGHT_ISMODIFIER);
            difference += SimpleCompare.CompareIsSummary(one, two, AspectWeights.WEIGHT_ISSUMMARY);
            difference += SimpleCompare.CompareRequirements(one, two, AspectWeights.WEIGHT_REQUIREMENTS);
            difference += SimpleCompare.CompareComments(one, two, AspectWeights.WEIGHT_COMMENTS);
            difference += SimpleCompare.CompareDefinition(one, two, AspectWeights.WEIGHT_DEFINITION);
            difference += SimpleCompare.CompareShort(one, two, AspectWeights.WEIGHT_SHORT);
            difference += SimpleCompare.CompareLabel(one, two, AspectWeights.WEIGHT_LABEL);
            difference += SimpleCompare.CompareName(one, two, AspectWeights.WEIGHT_NAME);
            difference += SimpleCompare.CompareMeaningWhenMissing(one, two, AspectWeights.WEIGHT_MEANINGWHENMISSING);
            difference += SimpleCompare.CompareMaxLength(one, two, AspectWeights.WEIGHT_MAXLENGTH);
            difference += CompareValueProperty.CompareExample(one, two, AspectWeights.WEIGHT_EXAMPLE);

            // only compare ValueRanges when type is the same & valueRange can only be set if type is a single type (no list)
            if (distanceType <= ((AspectWeights.WEIGHT_TYPE_AGGREGATION + AspectWeights.WEIGHT_TYPE_PROFILE)* AspectWeights.WEIGHT_TYPE) && one.Type.Count() == two.Type.Count() && one.Type.Count() == 1)
            {
                difference += CompareRangeValues.CompareRange(one, two);
                difference += CompareValueProperty.CompareDefaultValue(one, two, AspectWeights.WEIGHT_DEFAULTVALUE);
                difference += CompareValueProperty.CompareFixed(one, two, AspectWeights.WEIGHT_FIXED);
                difference += CompareValueProperty.ComparePattern(one, two, AspectWeights.WEIGHT_PATTERN);
            }
            
            difference += CompareBinding.DistanceBinding(one.Binding, two.Binding, AspectWeights.WEIGHT_BINDING);
            difference += CompareBase.DistanceBase(one.Base, two.Base, AspectWeights.WEIGHT_BASE);

            difference += CompareRepresentation.DistanceRepresentation(one.Representation, two.Representation, AspectWeights.WEIGHT_REPRESENTATION);
            difference += CompareCode.DistanceCode(one.Code, two.Code, AspectWeights.WEIGHT_CODE);
            difference += CompareAlias.DistanceAlias(one.Alias, two.Alias, AspectWeights.WEIGHT_ALIAS);
            difference += CompareMapping.DistanceMapping(one.Mapping, two.Mapping, AspectWeights.WEIGHT_MAPPING);
            difference += CompareCondition.DistanceCondition(one.Condition, two.Condition, AspectWeights.WEIGHT_CONDITION);
            difference += CompareConstraint.DistanceConstraint(one.Constraint, two.Constraint, AspectWeights.WEIGHT_CONSTRAINT);
            return difference;
        }
    }
}
