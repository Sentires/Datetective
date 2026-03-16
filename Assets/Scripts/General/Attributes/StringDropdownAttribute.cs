using UnityEngine;

namespace TheDates.General
{
    public class StringDropdownAttribute : PropertyAttribute
    {
        public string[] Options { get; private set; }
        public string SourceName { get; private set; }
        public bool IsDynamic { get; private set; }

        // Constructor for static lists
        public StringDropdownAttribute(params string[] options)
        {
            Options = options;
            SourceName = "";
            IsDynamic = false;
        }
    
        // Constructor for dynamic lists via reflection
        public StringDropdownAttribute(string sourceName)
        {
            Options = null;
            this.SourceName = sourceName;
            IsDynamic = true;
        }
    }
}
