using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltraMan.Inspector
{
    /************************************************************************
     * Adds ConditinalHideAttribute to hide a property in the inspector
     * using another boolean.
     * 
     ************************************************************************/

    //[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        /* Attribute Parameters */
        public string sourceField = "";         // Field for Controlling-bool
        public bool hideInInspector = false;    // Determines if contents are hidden
        public float min, max;                  // Range for sliders
        public List<int> values;                // Values to show on.
        public Attribute attribute;

        /// <summary> Hide contents depending on boolean </summary>
        /// <param name="_sourceField"> Name of controlling boolean </param>
        public ConditionalHideAttribute(string _sourceField)
        {
            this.sourceField = _sourceField;
            this.hideInInspector = true;
            min = 0;
            max = 0;
            values = null;
        }

        /// <summary> Hide contents depending on boolean </summary>
        /// <param name="_sourceField"> Name of controlling boolean </param>
        /// <param name="_hideInInspector"> Determines whether contents will be shown by default </param>
        public ConditionalHideAttribute(string _sourceField, bool _hideInInspector)
        {
            this.sourceField = _sourceField;
            this.hideInInspector = _hideInInspector;
            min = 0;
            max = 0;
            values = null;
        }

        /// <summary> Hide contents depending on boolean with Range </summary>
        /// <param name="_sourceField"> Name of controlling boolean </param>
        /// <param name="_min"> Minimum value </param>
        /// <param name="_max"> Maximum value </param>
        public ConditionalHideAttribute(string _sourceField, float _min, float _max)
        {
            this.sourceField = _sourceField;
            this.hideInInspector = true;
            min = _min;
            max = _max;
            values = null;
        }

        /// <summary> Hide contents depending on boolean with Range </summary>
        /// <param name="_sourceField"> Name of controlling boolean </param>
        /// <param name="_hideInInspector"> Determines whether contents will be shown by default </param>
        /// <param name="_min"> Minimum value </param>
        /// <param name="_max"> Maximum value </param>
        public ConditionalHideAttribute(string _sourceField, bool _hideInInspector, float _min, float _max)
        {
            this.sourceField = _sourceField;
            this.hideInInspector = _hideInInspector;
            min = _min;
            max = _max;
            values = null;
        }

        /// <summary> Shows content depeding on enum integer value. </summary>
        /// <param name="_sourceField"> Name of controlling enum. </param>
        /// <param name="_value"> Enum index that shows this content. </param>
        public ConditionalHideAttribute(string _sourceField, int[] _values)
        {
            this.sourceField = _sourceField;
            this.hideInInspector = false;
            min = 0;
            max = 0;
            values = new List<int>(_values);
        }

        /// <summary> Shows content depeding on enum integer value. </summary>
        /// <param name="_sourceField"> Name of controlling enum. </param>
        /// <param name="_value"> Enum index that shows this content. </param>
        public ConditionalHideAttribute(string _sourceField, int _value)
        {
            this.sourceField = _sourceField;
            this.hideInInspector = false;
            min = 0;
            max = 0;
            values = new List<int>() { _value };
        }

        /// <summary> Shows content depeding on enum integer value with range. </summary>
        /// <param name="_sourceField"> Name of controlling enum. </param>
        /// <param name="_value"> Enum index that shows this content. </param>
        /// <param name="_min"> Minimum value </param>
        /// <param name="_max"> Maximum value </param>
        public ConditionalHideAttribute(string _sourceField, int _value, float _min, float _max)
        {
            this.sourceField = _sourceField;
            this.hideInInspector = false;
            min = _min;
            max = _max;
            values = new List<int>() { _value };
        }

        /// <summary> Shows content depeding on enum integer value with range. </summary>
        /// <param name="_sourceField"> Name of controlling enum. </param>
        /// <param name="_value"> Enum index that shows this content. </param>
        /// <param name="_min"> Minimum value </param>
        /// <param name="_max"> Maximum value </param>
        public ConditionalHideAttribute(string _sourceField, int[] _values, float _min, float _max)
        {
            this.sourceField = _sourceField;
            this.hideInInspector = false;
            min = _min;
            max = _max;
            values = new List<int>(_values);
        }

    }

}
