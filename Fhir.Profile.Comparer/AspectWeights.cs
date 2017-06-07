using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileComparisonMethod
{
    public static class AspectWeights
    {
        public const double PATH = 1;
        // CARDINALITY
        public const double BOTH_PROHIBITED = 0;
        public const double MANDATORY_PROHIBITED = 1;
        public const double PROHIBITED_OPTIONAL = 0.8;

        public const double MANDATORY_OPTIONAL = 0.438308131;
        public const double BOTH_MANDATORY_NO_OVERLAP = 0.438308131;
        public const double BOTH_MANDATORY_RANGE_WIHTIN_OTHER = 0.109577033;
        public const double BOTH_MANDATORY_PARTIAL_OVERLAP = 0.109577033;
        public const double MIN_OR_MAX_DIFFERS = 0.109577033;
        
        // TYPE
        public const double WEIGHT_TYPE = 0.273942582;
        public const double WEIGHT_TYPE_CODE = 1.0;
        public const double WEIGHT_TYPE_PROFILE = 0.15;
        public const double WEIGHT_TYPE_AGGREGATION = 0.05;

        // RANGE VALUE
        public const double VALUE_RANGE_WITHIN_OTHER = 0.010272847;
        public const double MINVALUE_OR_MAXVALUE_DIFFERS = 0.00684856;
        public const double VALUE_NO_OVERLAP = 0.17121411;
        public const double VALUE_PARTIAL_OVERLAP = 0.01027285;
        public const double VALUE_RANGE_TYPE_DIFFERS = 0.171214114;

        public const double WEIGHT_NAMEREFERENCE = 0;
        public const double WEIGHT_MUSTSUPPORT = 0.054788516;
        public const double WEIGHT_ISMODIFIER = 0.054788516;
        public const double WEIGHT_ISSUMMARY = 0.000547885;
        public const double WEIGHT_REQUIREMENTS = 0.000547885;
        public const double WEIGHT_COMMENTS = 0.000547885;
        public const double WEIGHT_DEFINITION = 0.000547885;
        public const double WEIGHT_SHORT = 0.000547885;
        public const double WEIGHT_LABEL = 0.000547885;
        public const double WEIGHT_NAME = 0;
        public const double WEIGHT_MEANINGWHENMISSING = 0.00109577;
        public const double WEIGHT_MAXLENGTH = 0.005478852;

        public const double WEIGHT_DEFAULTVALUE = 0.006848565;
        public const double WEIGHT_FIXED = 0.205456936;
        public const double WEIGHT_PATTERN = 0.006848565;
        public const double WEIGHT_EXAMPLE = 0.000547885;

        // BINDING
        public const double WEIGHT_BINDING = 0.164365549;
        public const double WEIGHT_BINDING_STRENGTH = 0.3;
        public const double WEIGHT_BINDING_DESCRIPTION = 0.01;
        public const double WEIGHT_BINDING_VALUESET = 0.699;

        // BASE
        public const double WEIGHT_BASE = 0.0000547885;
        public const double WEIGHT_BASE_PATH = 1.0;
        public const double WEIGHT_BASE_MIN = 0.5;
        public const double WEIGHT_BASE_MAX = 0.5;

        public const double WEIGHT_REPRESENTATION = 0.0000547885;

        // CODE
        public const double WEIGHT_CODE = 0.000547885;
        public const double WEIGHT_CODE_SYSTEM = 1.0;
        public const double WEIGHT_CODE_VERSION = 0.3;
        public const double WEIGHT_CODE_CODE = 0.5;
        public const double WEIGHT_CODE_DISPLAY = 0.1;
        public const double WEIGHT_CODE_USERSELECTED = 0.05;

        public const double WEIGHT_ALIAS = 0.000547885;
        
        // MAPPING
        public const double WEIGHT_MAPPING = 0.000547885;
        public const double WEIGHT_MAPPING_IDENTITY = 1.0;
        public const double WEIGHT_MAPPING_LANGUAGE = 0.2;
        public const double WEIGHT_MAPPING_MAP = 0.7;

        public const double WEIGHT_CONDITION = 0.000547885;

        // CONSTRAINT
        public const double WEIGHT_CONSTRAINT = 0.00109577;
        public const double WEIGHT_CONSTRAINT_XPATH = 1.0;
        public const double WEIGHT_CONSTRAINT_SEVERITY = 0.3;
        public const double WEIGHT_CONSTRAINT_KEY = 0.001;
        public const double WEIGHT_CONSTRAINT_REQUIREMENTS = 0.1;
        public const double WEIGHT_CONSTRAINT_HUMAN = 0.599;

        //ELEMENTS with more weight
        public const double ELEMENT_STATUS = 39.5;
        public const double ELEMENT_VALUE = 39.5;
        public const double ELEMENT_CODE = 39.5;
        public const double ELEMENT_CODE_CODING = 39.5;
        public const double ELEMENT_CODE_CODING_SYSTEM = 39.5;
        public const double ELEMENT_CODE_CODING_CODE = 39.5;
    }
}
