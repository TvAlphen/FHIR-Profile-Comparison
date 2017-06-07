using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PogingOmIetsTeVergelijken4
{
    class CompareRangeValues
    {
        public static double CompareRange(ElementDefinition one, ElementDefinition two)
        {
            
            double difference = 0;
            if (one.MaxValue == null && two.MaxValue == null && one.MinValue == null && two.MinValue == null)
            {
                Program.LogAspectDifference(difference, "ValueRange");
                return difference;
            }

            if (one.MinValue == null && two.MinValue == null && one.MaxValue != null && two.MaxValue != null)
            {
                if(one.MaxValue.GetType() == two.MaxValue.GetType())
                {
                    // ene range binnen andere, min is gelijk
                    if(one.MaxValue != two.MaxValue)
                    {
                        difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    
                    Program.LogAspectDifference(difference, "ValueRange");
                    return difference;
                }
                else { return AspectWeights.VALUE_RANGE_TYPE_DIFFERS; }
               
            }
            if (one.MinValue == null && two.MinValue == null && one.MaxValue == null && two.MaxValue != null)
            {
                // ene range binnen andere, min is gelijk
                difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                Program.LogAspectDifference(difference, "ValueRange");
                return difference;
            }
            if (one.MinValue == null && two.MinValue == null && one.MaxValue != null && two.MaxValue == null)
            {
                // ene range binnen andere, min is gelijk
                difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                Program.LogAspectDifference(difference, "ValueRange");
                return difference;
            }

            if (one.MaxValue == null && two.MaxValue == null && one.MinValue != null && two.MinValue != null )
            {
                if (one.MinValue.GetType() == two.MinValue.GetType())
                {
                    // ene range binnen andere, max is gelijk
                    if(one.MinValue != two.MinValue)
                    {
                        difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    Program.LogAspectDifference(difference, "ValueRange");
                    return difference;
                }
                else { return AspectWeights.VALUE_RANGE_TYPE_DIFFERS; }
            }
            if (one.MaxValue == null && two.MaxValue == null && one.MinValue == null && two.MinValue != null)
            {
                    // ene range binnen andere, max is gelijk
                    difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    Program.LogAspectDifference(difference, "ValueRange");
                    return difference;
            }
            if (one.MaxValue == null && two.MaxValue == null && one.MinValue != null && two.MinValue == null)
            {
                // ene range binnen andere, max is gelijk
                difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                Program.LogAspectDifference(difference, "ValueRange");
                return difference;
            }

            if (one.MinValue == null && two.MinValue != null && one.MaxValue == null && two.MaxValue != null)
            {
                // ene range binnen andere
                difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                Program.LogAspectDifference(difference, "ValueRange");
                return difference;
            }
            if (two.MinValue == null && one.MinValue != null && two.MaxValue == null && one.MaxValue != null)
            {
                // ene range binnen andere
                difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                Program.LogAspectDifference(difference, "ValueRange");
                return difference;
            }

            if (one.MinValue == null && two.MinValue != null && one.MaxValue != null && two.MaxValue != null)
            {
                if (one.MaxValue.GetType() == two.MaxValue.GetType())
                {
                    if (two.MaxValue is FhirDateTime)
                    {
                        //half overlap
                        if ((FhirDateTime)one.MaxValue < (FhirDateTime)two.MaxValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((FhirDateTime)one.MaxValue > (FhirDateTime)two.MaxValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((FhirDateTime)one.MaxValue == (FhirDateTime)two.MaxValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MaxValue is Date)
                    {
                        //half overlap
                        if ((Date)one.MaxValue < (Date)two.MaxValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((Date)one.MaxValue > (Date)two.MaxValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Date)one.MaxValue == (Date)two.MaxValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MaxValue is Instant)
                    {
                        //half overlap
                        if ((Instant)one.MaxValue < (Instant)two.MaxValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((Instant)one.MaxValue > (Instant)two.MaxValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Instant)one.MaxValue == (Instant)two.MaxValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MaxValue is Time)
                    {
                        //half overlap
                        if ((Time)one.MaxValue < (Time)two.MaxValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((Time)one.MaxValue > (Time)two.MaxValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Time)one.MaxValue == (Time)two.MaxValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    //ignore unit
                    if (two.MaxValue is Quantity)
                    {
                        //half overlap
                        if (((Quantity)one.MaxValue).Value < ((Quantity)two.MaxValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if (((Quantity)one.MaxValue).Value > ((Quantity)two.MaxValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((Quantity)one.MaxValue).Value == ((Quantity)two.MaxValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MaxValue is FhirDecimal)
                    {
                        //half overlap
                        if (((FhirDecimal)one.MaxValue).Value < ((FhirDecimal)two.MaxValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if (((FhirDecimal)one.MaxValue).Value > ((FhirDecimal)two.MaxValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((FhirDecimal)one.MaxValue).Value == ((FhirDecimal)two.MaxValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MaxValue is Integer)
                    {
                        //half overlap
                        if (((Integer)one.MaxValue).Value < ((Integer)two.MaxValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if (((Integer)one.MaxValue).Value > ((Integer)two.MaxValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((Integer)one.MaxValue).Value == ((Integer)two.MaxValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    Program.LogAspectDifference(difference, "ValueRange");
                    return difference;
                }
                else { return AspectWeights.VALUE_RANGE_TYPE_DIFFERS; }
            }

            if (one.MinValue != null && two.MinValue == null && one.MaxValue != null && two.MaxValue != null)
            {
                if (one.MaxValue.GetType() == two.MaxValue.GetType())
                {
                    if (one.MaxValue is FhirDateTime)
                    {
                        //half overlap
                        if ((FhirDateTime)one.MaxValue > (FhirDateTime)two.MaxValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((FhirDateTime)one.MaxValue < (FhirDateTime)two.MaxValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((FhirDateTime)one.MaxValue == (FhirDateTime)two.MaxValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (one.MaxValue is Date)
                    {
                        //half overlap
                        if ((Date)one.MaxValue > (Date)two.MaxValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((Date)one.MaxValue < (Date)two.MaxValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Date)one.MaxValue == (Date)two.MaxValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (one.MaxValue is Instant)
                    {
                        //half overlap
                        if ((Instant)one.MaxValue > (Instant)two.MaxValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((Instant)one.MaxValue < (Instant)two.MaxValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Instant)one.MaxValue == (Instant)two.MaxValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (one.MaxValue is Time)
                    {
                        //half overlap
                        if ((Time)one.MaxValue > (Time)two.MaxValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((Time)one.MaxValue < (Time)two.MaxValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Time)one.MaxValue == (Time)two.MaxValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (one.MaxValue is Quantity)
                    {
                        //half overlap
                        if (((Quantity)one.MaxValue).Value > ((Quantity)two.MaxValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if (((Quantity)one.MaxValue).Value < ((Quantity)two.MaxValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((Quantity)one.MaxValue).Value == ((Quantity)two.MaxValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (one.MaxValue is FhirDecimal)
                    {
                        //half overlap
                        if (((FhirDecimal)one.MaxValue).Value > ((FhirDecimal)two.MaxValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if (((FhirDecimal)one.MaxValue).Value < ((FhirDecimal)two.MaxValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((FhirDecimal)one.MaxValue).Value == ((FhirDecimal)two.MaxValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (one.MaxValue is Integer)
                    {
                        //half overlap
                        if (((Integer)one.MaxValue).Value > ((Integer)two.MaxValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if (((Integer)one.MaxValue).Value < ((Integer)two.MaxValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((Integer)one.MaxValue).Value == ((Integer)two.MaxValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    Program.LogAspectDifference(difference, "ValueRange");
                    return difference;
                }
                else { return AspectWeights.VALUE_RANGE_TYPE_DIFFERS; }
            }

            if (one.MinValue != null && two.MinValue != null && one.MaxValue == null && two.MaxValue != null)
            {
                if (one.MinValue.GetType() == two.MinValue.GetType())
                {
                    if (two.MinValue is FhirDateTime)
                    {
                        //half overlap
                        if ((FhirDateTime)one.MinValue > (FhirDateTime)two.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((FhirDateTime)one.MinValue < (FhirDateTime)two.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((FhirDateTime)one.MinValue == (FhirDateTime)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MinValue is Date)
                    {
                        //half overlap
                        if ((Date)one.MinValue > (Date)two.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((Date)one.MinValue < (Date)two.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Date)one.MinValue == (Date)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MinValue is Instant)
                    {
                        //half overlap
                        if ((Instant)one.MinValue > (Instant)two.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((Instant)one.MinValue < (Instant)two.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Instant)one.MinValue == (Instant)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MinValue is Time)
                    {
                        //half overlap
                        if ((Time)one.MinValue > (Time)two.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((Time)one.MinValue < (Time)two.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Time)one.MinValue == (Time)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MinValue is Quantity)
                    {
                        //half overlap
                        if (((Quantity)one.MinValue).Value > ((Quantity)two.MinValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if (((Quantity)one.MinValue).Value < ((Quantity)two.MinValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((Quantity)one.MinValue).Value == ((Quantity)two.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MinValue is FhirDecimal)
                    {
                        //half overlap
                        if (((FhirDecimal)one.MinValue).Value > ((FhirDecimal)two.MinValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if (((FhirDecimal)one.MinValue).Value < ((FhirDecimal)two.MinValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((FhirDecimal)one.MinValue).Value == ((FhirDecimal)two.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MinValue is Integer)
                    {
                        //half overlap
                        if (((Integer)one.MinValue).Value > ((Integer)two.MinValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if (((Integer)one.MinValue).Value < ((Integer)two.MinValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((Integer)one.MinValue).Value == ((Integer)two.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    Program.LogAspectDifference(difference, "ValueRange");
                    return difference;
                }
                else { return AspectWeights.VALUE_RANGE_TYPE_DIFFERS; }
            }

            if (one.MinValue != null && two.MinValue != null && one.MaxValue != null && two.MaxValue == null)
            {
                if (one.MinValue.GetType() == two.MinValue.GetType())
                {
                    if (two.MinValue is FhirDateTime)
                    {
                        //half overlap
                        if ((FhirDateTime)one.MinValue < (FhirDateTime)two.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((FhirDateTime)one.MinValue > (FhirDateTime)two.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((FhirDateTime)one.MinValue == (FhirDateTime)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MinValue is Date)
                    {
                        //half overlap
                        if ((Date)one.MinValue < (Date)two.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((Date)one.MinValue > (Date)two.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Date)one.MinValue == (Date)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MinValue is Instant)
                    {
                        //half overlap
                        if ((Instant)one.MinValue < (Instant)two.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((Instant)one.MinValue > (Instant)two.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Instant)one.MinValue == (Instant)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MinValue is Time)
                    {
                        //half overlap
                        if ((Time)one.MinValue < (Time)two.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if ((Time)one.MinValue > (Time)two.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Time)one.MinValue == (Time)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MinValue is Quantity)
                    {
                        //half overlap
                        if (((Quantity)one.MinValue).Value < ((Quantity)two.MinValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if (((Quantity)one.MinValue).Value > ((Quantity)two.MinValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((Quantity)one.MinValue).Value == ((Quantity)two.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MinValue is FhirDecimal)
                    {
                        //half overlap
                        if (((FhirDecimal)one.MinValue).Value < ((FhirDecimal)two.MinValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if (((FhirDecimal)one.MinValue).Value > ((FhirDecimal)two.MinValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((FhirDecimal)one.MinValue).Value == ((FhirDecimal)two.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    if (two.MinValue is Integer)
                    {
                        //half overlap
                        if (((Integer)one.MinValue).Value < ((Integer)two.MinValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        // ene range binnen  andere
                        if (((Integer)one.MinValue).Value > ((Integer)two.MinValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((Integer)one.MinValue).Value == ((Integer)two.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                    }
                    Program.LogAspectDifference(difference, "ValueRange");
                    return difference;
                }
                else { return AspectWeights.VALUE_RANGE_TYPE_DIFFERS; }
            }

            if (one.MinValue != null && two.MinValue != null && one.MaxValue != null && two.MaxValue != null)
            {
                if (one.MinValue.GetType() == two.MinValue.GetType() && one.MaxValue.GetType() == two.MaxValue.GetType())
                {
                    if (one.MinValue is FhirDateTime)
                    {
                        // max van 1 is kleiner dan min van 2..geen overlap ranges
                        if ((FhirDateTime)one.MaxValue < (FhirDateTime)two.MinValue || (FhirDateTime)two.MaxValue < (FhirDateTime)one.MinValue) difference += AspectWeights.VALUE_NO_OVERLAP;
                        // range van 2 binnen range van 1
                        if ((FhirDateTime)one.MaxValue > (FhirDateTime)two.MaxValue && (FhirDateTime)one.MinValue < (FhirDateTime)two.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((FhirDateTime)one.MaxValue == (FhirDateTime)two.MaxValue && (FhirDateTime)one.MinValue < (FhirDateTime)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        if ((FhirDateTime)one.MaxValue > (FhirDateTime)two.MaxValue && (FhirDateTime)one.MinValue == (FhirDateTime)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        // range van 1 binnen range van 2
                        if ((FhirDateTime)two.MaxValue > (FhirDateTime)one.MaxValue && (FhirDateTime)two.MinValue < (FhirDateTime)one.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((FhirDateTime)two.MaxValue == (FhirDateTime)one.MaxValue && (FhirDateTime)two.MinValue < (FhirDateTime)one.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        if ((FhirDateTime)two.MaxValue > (FhirDateTime)one.MaxValue && (FhirDateTime)two.MinValue == (FhirDateTime)one.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        // half overlap
                        if ((FhirDateTime)one.MaxValue > (FhirDateTime)two.MaxValue && (FhirDateTime)one.MinValue > (FhirDateTime)two.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        if ((FhirDateTime)two.MaxValue > (FhirDateTime)one.MaxValue && (FhirDateTime)two.MinValue > (FhirDateTime)one.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                    }
                    if (one.MinValue is Date)
                    {
                        // max van 1 is kleiner dan min van 2..geen overlap ranges
                        if ((Date)one.MaxValue < (Date)two.MinValue || (Date)two.MaxValue < (Date)one.MinValue) difference += AspectWeights.VALUE_NO_OVERLAP;
                        // range van 2 binnen range van 1
                        if ((Date)one.MaxValue > (Date)two.MaxValue && (Date)one.MinValue < (Date)two.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Date)one.MaxValue == (Date)two.MaxValue && (Date)one.MinValue < (Date)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        if ((Date)one.MaxValue > (Date)two.MaxValue && (Date)one.MinValue == (Date)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        // range van 1 binnen range van 2
                        if ((Date)two.MaxValue > (Date)one.MaxValue && (Date)two.MinValue < (Date)one.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Date)two.MaxValue == (Date)one.MaxValue && (Date)two.MinValue < (Date)one.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        if ((Date)two.MaxValue > (Date)one.MaxValue && (Date)two.MinValue == (Date)one.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        // half overlap
                        if ((Date)one.MaxValue > (Date)two.MaxValue && (Date)one.MinValue > (Date)two.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        if ((Date)two.MaxValue > (Date)one.MaxValue && (Date)two.MinValue > (Date)one.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                    }
                    if (one.MinValue is Instant)
                    {
                        // max van 1 is kleiner dan min van 2..geen overlap ranges
                        if ((Instant)one.MaxValue < (Instant)two.MinValue || (Instant)two.MaxValue < (Instant)one.MinValue) difference += AspectWeights.VALUE_NO_OVERLAP;
                        // range van 2 binnen range van 1
                        if ((Instant)one.MaxValue > (Instant)two.MaxValue && (Instant)one.MinValue < (Instant)two.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Instant)one.MaxValue == (Instant)two.MaxValue && (Instant)one.MinValue < (Instant)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        if ((Instant)one.MaxValue > (Instant)two.MaxValue && (Instant)one.MinValue == (Instant)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        // range van 1 binnen range van 2
                        if ((Instant)two.MaxValue > (Instant)one.MaxValue && (Instant)two.MinValue < (Instant)one.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Instant)two.MaxValue == (Instant)one.MaxValue && (Instant)two.MinValue < (Instant)one.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        if ((Instant)two.MaxValue > (Instant)one.MaxValue && (Instant)two.MinValue == (Instant)one.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        // half overlap
                        if ((Instant)one.MaxValue > (Instant)two.MaxValue && (Instant)one.MinValue > (Instant)two.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        if ((Instant)two.MaxValue > (Instant)one.MaxValue && (Instant)two.MinValue > (Instant)one.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                    }
                    if (one.MinValue is Time)
                    {
                        // max van 1 is kleiner dan min van 2..geen overlap ranges
                        if ((Time)one.MaxValue < (Time)two.MinValue || (Time)two.MaxValue < (Time)one.MinValue) difference += AspectWeights.VALUE_NO_OVERLAP;
                        // range van 2 binnen range van 1
                        if ((Time)one.MaxValue > (Time)two.MaxValue && (Time)one.MinValue < (Time)two.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Time)one.MaxValue == (Time)two.MaxValue && (Time)one.MinValue < (Time)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        if ((Time)one.MaxValue > (Time)two.MaxValue && (Time)one.MinValue == (Time)two.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        // range van 1 binnen range van 2
                        if ((Time)two.MaxValue > (Time)one.MaxValue && (Time)two.MinValue < (Time)one.MinValue) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if ((Time)two.MaxValue == (Time)one.MaxValue && (Time)two.MinValue < (Time)one.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        if ((Time)two.MaxValue > (Time)one.MaxValue && (Time)two.MinValue == (Time)one.MinValue) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        // half overlap
                        if ((Time)one.MaxValue > (Time)two.MaxValue && (Time)one.MinValue > (Time)two.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        if ((Time)two.MaxValue > (Time)one.MaxValue && (Time)two.MinValue > (Time)one.MinValue) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                    }
                    // Quantity, UNIT er nog bij betrekken, verschil in value kan nog steeds zelfde zijn als unit ook anders is, of zelfde value different unit
                    if (one.MinValue is Quantity)
                    {
                        if(((Quantity)one.MaxValue).Unit != ((Quantity)two.MaxValue).Unit) return AspectWeights.VALUE_RANGE_TYPE_DIFFERS;
                        // max van 1 is kleiner dan min van 2..geen overlap ranges
                        if (((Quantity)one.MaxValue).Value < ((Quantity)two.MinValue).Value || ((Quantity)two.MaxValue).Value < ((Quantity)one.MinValue).Value) difference += AspectWeights.VALUE_NO_OVERLAP;
                        // range van 2 binnen range van 1
                        if (((Quantity)one.MaxValue).Value > ((Quantity)two.MaxValue).Value && ((Quantity)one.MinValue).Value < ((Quantity)two.MinValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((Quantity)one.MaxValue).Value == ((Quantity)two.MaxValue).Value && ((Quantity)one.MinValue).Value < ((Quantity)two.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        if (((Quantity)one.MaxValue).Value > ((Quantity)two.MaxValue).Value && ((Quantity)one.MinValue).Value == ((Quantity)two.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        // range van 1 binnen range van 2
                        if (((Quantity)two.MaxValue).Value > ((Quantity)one.MaxValue).Value && ((Quantity)two.MinValue).Value < ((Quantity)one.MinValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((Quantity)two.MaxValue).Value == ((Quantity)one.MaxValue).Value && ((Quantity)two.MinValue).Value < ((Quantity)one.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        if (((Quantity)two.MaxValue).Value > ((Quantity)one.MaxValue).Value && ((Quantity)two.MinValue).Value == ((Quantity)one.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        // half overlap
                        if (((Quantity)one.MaxValue).Value > ((Quantity)two.MaxValue).Value && ((Quantity)one.MinValue).Value > ((Quantity)two.MinValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        if (((Quantity)two.MaxValue).Value > ((Quantity)one.MaxValue).Value && ((Quantity)two.MinValue).Value > ((Quantity)one.MinValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                    }
                    if (one.MinValue is FhirDecimal)
                    {
                        // max van 1 is kleiner dan min van 2..geen overlap ranges
                        if (((FhirDecimal)one.MaxValue).Value < ((FhirDecimal)two.MinValue).Value || ((FhirDecimal)two.MaxValue).Value < ((FhirDecimal)one.MinValue).Value) difference += AspectWeights.VALUE_NO_OVERLAP;
                        // range van 2 binnen range van 1
                        if (((FhirDecimal)one.MaxValue).Value > ((FhirDecimal)two.MaxValue).Value && ((FhirDecimal)one.MinValue).Value < ((FhirDecimal)two.MinValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((FhirDecimal)one.MaxValue).Value == ((FhirDecimal)two.MaxValue).Value && ((FhirDecimal)one.MinValue).Value < ((FhirDecimal)two.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        if (((FhirDecimal)one.MaxValue).Value > ((FhirDecimal)two.MaxValue).Value && ((FhirDecimal)one.MinValue).Value == ((FhirDecimal)two.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        // range van 1 binnen range van 2
                        if (((FhirDecimal)two.MaxValue).Value > ((FhirDecimal)one.MaxValue).Value && ((FhirDecimal)two.MinValue).Value < ((FhirDecimal)one.MinValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((FhirDecimal)two.MaxValue).Value == ((FhirDecimal)one.MaxValue).Value && ((FhirDecimal)two.MinValue).Value < ((FhirDecimal)one.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        if (((FhirDecimal)two.MaxValue).Value > ((FhirDecimal)one.MaxValue).Value && ((FhirDecimal)two.MinValue).Value == ((FhirDecimal)one.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        // half overlap
                        if (((FhirDecimal)one.MaxValue).Value > ((FhirDecimal)two.MaxValue).Value && ((FhirDecimal)one.MinValue).Value > ((FhirDecimal)two.MinValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        if (((FhirDecimal)two.MaxValue).Value > ((FhirDecimal)one.MaxValue).Value && ((FhirDecimal)two.MinValue).Value > ((FhirDecimal)one.MinValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                    }
                    if (one.MinValue is Integer)
                    {
                        // max van 1 is kleiner dan min van 2..geen overlap ranges
                        if (((Integer)one.MaxValue).Value < ((Integer)two.MinValue).Value || ((Integer)two.MaxValue).Value < ((Integer)one.MinValue).Value) difference += AspectWeights.VALUE_NO_OVERLAP;
                        // range van 2 binnen range van 1
                        if (((Integer)one.MaxValue).Value > ((Integer)two.MaxValue).Value && ((Integer)one.MinValue).Value < ((Integer)two.MinValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((Integer)one.MaxValue).Value == ((Integer)two.MaxValue).Value && ((Integer)one.MinValue).Value < ((Integer)two.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        if (((Integer)one.MaxValue).Value > ((Integer)two.MaxValue).Value && ((Integer)one.MinValue).Value == ((Integer)two.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        // range van 1 binnen range van 2
                        if (((Integer)two.MaxValue).Value > ((Integer)one.MaxValue).Value && ((Integer)two.MinValue).Value < ((Integer)one.MinValue).Value) difference += AspectWeights.VALUE_RANGE_WITHIN_OTHER;
                        if (((Integer)two.MaxValue).Value == ((Integer)one.MaxValue).Value && ((Integer)two.MinValue).Value < ((Integer)one.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        if (((Integer)two.MaxValue).Value > ((Integer)one.MaxValue).Value && ((Integer)two.MinValue).Value == ((Integer)one.MinValue).Value) difference += AspectWeights.MINVALUE_OR_MAXVALUE_DIFFERS;
                        // half overlap
                        if (((Integer)one.MaxValue).Value > ((Integer)two.MaxValue).Value && ((Integer)one.MinValue).Value > ((Integer)two.MinValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                        if (((Integer)two.MaxValue).Value > ((Integer)one.MaxValue).Value && ((Integer)two.MinValue).Value > ((Integer)one.MinValue).Value) difference += AspectWeights.VALUE_PARTIAL_OVERLAP;
                    }
                }
                Program.LogAspectDifference(difference, "ValueRange");
                return difference;
            }
            else { return AspectWeights.VALUE_RANGE_TYPE_DIFFERS; }
        }
    }
}
