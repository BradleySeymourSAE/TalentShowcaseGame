using UnityEngine;


namespace Framework.Scripts.Common.Attributes
{
    public class CreateScriptableObjectAttribute : UnityEngine.PropertyAttribute 
    {
        public bool isAsset = true;

        public CreateScriptableObjectAttribute(bool isAsset) => this.isAsset = isAsset;
        public CreateScriptableObjectAttribute() => this.isAsset = true;
        
    }
   

    #if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(CreateScriptableObjectAttribute), true)]
    public class CreateAssetDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            Object element = property.objectReferenceValue;
            CreateScriptableObjectAttribute attr = attribute as CreateScriptableObjectAttribute;

            if (element == null)
            {
                position.width -= 22;
                UnityEditor.EditorGUI.PropertyField(position, property);
                Rect AddButtonRect = new Rect(position) { x = position.width + position.x + 2, width = 20 };

                if (GUI.Button(AddButtonRect, "+"))
                {
                    if (attr.isAsset)
                    {
                        EditorExtensions.CreateScriptableAsset(property, EditorExtensions.GetType(property), EditorExtensions.GetSelectedPathOrFallback());
                    }
                    else
                    {
                        EditorExtensions.CreateScriptableAssetInternal(property, EditorExtensions.GetType(property));
                    }
                }
            }
            else
            {
                UnityEditor.EditorGUI.PropertyField(position, property);
            }
        }
    }
    #endif
}