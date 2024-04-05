#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace Framework.Scripts.Common.Injection.Editor
{
    [CustomEditor(typeof(InjectionSystem))]
    public class InjectorBehaviourInspector : UnityEditor.Editor 
    {
        private InjectionSystem Target => target as InjectionSystem;
        private GUIContent content; 
        
        public override void OnInspectorGUI()
        {
            if (Target == null)
            {
                return;
            }
            DrawDefaultInspector(); 
            
            content = new GUIContent("Validate Dependencies", "Validates all dependencies on this object and all children.");
            if (GUILayout.Button(content, GUILayout.Height(30.0f)))
            {
                Target.ValidateDependencies(); 
            }
            
            content = new GUIContent("Clear Dependencies", "Clears all dependencies on this object and all children."); 
            if (GUILayout.Button(content, GUILayout.Height(30.0f)))
            {
                Target.ClearDependencies();
                EditorUtility.SetDirty(Target); 
            }
        }
    }
}
#endif 