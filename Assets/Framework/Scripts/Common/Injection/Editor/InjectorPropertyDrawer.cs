#if UNITY_EDITOR
using Framework.Common;
using UnityEditor;
using UnityEngine;
namespace Framework.Scripts.Common.Injection.Editor
{
    [CustomPropertyDrawer(typeof(InjectionAttribute))]
    public class InjectorPropertyDrawer : UnityEditor.PropertyDrawer
    {
        private Texture2D Icon;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
           Icon = LoadIcon();
           var rect = new Rect(position.x, position.y, 20, 20);
           position.xMin += 24;
           if (Icon)
           {
               Color currentColor = GUI.color; 
               GUI.color = property.objectReferenceValue == null 
                   ? currentColor
                   : Color.green; 
                GUI.DrawTexture(rect, Icon);
                GUI.color = currentColor; 
           }
           EditorGUI.PropertyField(position, property, label); 
        }
        
        private Texture2D LoadIcon()
        {
            if (Icon == null)
            {
                string path = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("InjectionIcon")[0]);
                console.log(this, path);
                Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
            return Icon; 
        }
    }
}
#endif 