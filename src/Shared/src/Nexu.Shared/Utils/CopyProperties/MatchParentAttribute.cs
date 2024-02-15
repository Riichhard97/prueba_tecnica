using System;

namespace Nexu.Shared.Utils.CopyProperties
{
    /// <summary>
    /// Create to references to parent class to use the utility copy
    /// Eample to use
    /// [MatchParent("Name")]
    /// public string NameMatch { get; set; }
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MatchParentAttribute : Attribute
    {
        public readonly string ParentPropertyName;
        public MatchParentAttribute(string parentPropertyName)
        {
            ParentPropertyName = parentPropertyName;
        }
    }
}
