using Framework.Scripts.Common.TriggerToolkit;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Framework.CustomEditors.Drawers
{
    [CustomPropertyDrawer(typeof(TriggerCondition))]
    public sealed class TriggerConditionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty typeProperty = property.FindPropertyRelative("TriggerConditionType");
            TriggerCondition.ETriggerEventType conditionType = (TriggerCondition.ETriggerEventType) typeProperty.enumValueIndex;

            property.isExpanded = EditorGUI.Foldout(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded, conditionType.ToString()
            );
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (property.isExpanded)
            {
                // reset the height of the property 
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, typeProperty);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // draw the float property if appropriate 
                if (conditionType == TriggerCondition.ETriggerEventType.STAY)
                {
                    SerializedProperty floatProperty = property.FindPropertyRelative("FloatValue");
                    EditorGUI.PropertyField(position, floatProperty, new GUIContent("Duration (s)"));
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                // Retrieve all tags 
                string[] outputTags = UnityEditorInternal.InternalEditorUtility.tags;
                bool[] selectedTags = new bool[outputTags.Length];

                // Retrieve tags property 
                SerializedProperty l_TagsProperty = property.FindPropertyRelative("Tags");

                // determine which tags are selected 
                for (int selectedIndex = 0; selectedIndex < l_TagsProperty.arraySize; ++selectedIndex)
                {
                    string tag = l_TagsProperty.GetArrayElementAtIndex(selectedIndex).stringValue;
                    for (int tagIndex = 0; tagIndex < outputTags.Length; ++tagIndex)
                    {
                        // check if the tag is selected
                        if (outputTags[tagIndex] == tag)
                        {
                            selectedTags[tagIndex] = true;
                            break;
                        }
                    }
                }

                // draw the list of tags 
                EditorGUI.LabelField(position, "Tags to check against", EditorStyles.boldLabel);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                bool hasModifiedTags = false;
                int numberOfSelectedTags = 0;

                // get all of the tags 
                for (int idx = 0; idx < outputTags.Length; ++idx)
                {
                    bool previouslySelected = selectedTags[idx];
                    selectedTags[idx] = EditorGUI.Toggle(position, new GUIContent(outputTags[idx]), selectedTags[idx]);
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    hasModifiedTags |= previouslySelected != selectedTags[idx];
                    if (selectedTags[idx])
                    {
                        numberOfSelectedTags++;
                    }
                }

                // rebuild the tags array 
                if (hasModifiedTags)
                {
                    l_TagsProperty.arraySize = numberOfSelectedTags;

                    // update the tags list 
                    int writeIndex = 0;
                    for (int index = 0; index < outputTags.Length; ++index)
                    {
                        if (selectedTags[index])
                        {
                            l_TagsProperty.GetArrayElementAtIndex(writeIndex).stringValue = outputTags[index];
                            ++writeIndex;
                        }
                    }
                }

                // on trigger property 
                SerializedProperty onTriggerProperty = property.FindPropertyRelative("OnTrigger");
                EditorGUI.PropertyField(position, onTriggerProperty);
            }
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (property.isExpanded)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                SerializedProperty propertyType = property.FindPropertyRelative("TriggerConditionType");
                if ((TriggerCondition.ETriggerEventType) propertyType.enumValueIndex == TriggerCondition.ETriggerEventType.STAY)
                {
                    height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                height += UnityEditorInternal.InternalEditorUtility.tags.Length * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

                SerializedProperty onTriggerProperty = property.FindPropertyRelative("OnTrigger");
                height += EditorGUI.GetPropertyHeight(onTriggerProperty, true);
            }
            return height;
        }
    }
}
#endif
