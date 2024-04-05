using UnityEngine;
namespace Framework.Scripts.Common.Attributes
{
    public class TagSelectorAttribute : PropertyAttribute
    {
        public readonly bool UseDefaultTagFieldDrawer;

        public TagSelectorAttribute(bool UseDefaultFieldDrawer = false)
        {
            this.UseDefaultTagFieldDrawer = UseDefaultFieldDrawer; 
        }
    }
}