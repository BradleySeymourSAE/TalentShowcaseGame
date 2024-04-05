#if UNITY_EDITOR 
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Scripts.Editor
{

    public class SortingOrderChanger : EditorWindow
    {
        [SerializeField] private int OffsetSortingOrder = 50; 
       
        private List<GameObject> m_SelectedObjects = new();
        private GUIContent content;
        private Vector2 m_HeirarchySelectionScrollPosition; 

        [MenuItem("Framework/Sorting Order Changer")]
        private static void ShowWindow()
        {
            EditorWindow.GetWindow<SortingOrderChanger>();
        }

        private void OnGUI()
        {
            OffsetSortingOrder = EditorGUILayout.IntField("Offset Sorting Order", OffsetSortingOrder); 
            if (GUILayout.Button("Change Sorting Order"))
            {
                GameObject[] selection = Selection.gameObjects;
                for (int i = selection.Length - 1; i >= 0; --i)
                {
                    Renderer[] renderers = selection[i].GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        renderer.sortingOrder += OffsetSortingOrder;
                    }
                }
            } 
            
            EditorGUILayout.Separator(); 
            EditorGUILayout.LabelField("Selected Objects: " + Selection.objects.Length);
            DrawHeirachySelections(); 
        }
        
        private void DrawHeirachySelections()
        {
            content = new GUIContent("Heirarchy Selections:", "Selected assets in the HIERARCHY view.");
            EditorGUILayout.LabelField(content, EditorStyles.boldLabel);
            EditorGUILayout.Separator(); 
            m_HeirarchySelectionScrollPosition = EditorGUILayout.BeginScrollView(m_HeirarchySelectionScrollPosition);
            {
                if (m_SelectedObjects.Count == 0)
                {
                    EditorGUILayout.LabelField("Please select some asset(s) in the hierarchy view.", EditorStyles.miniBoldLabel);
                }
                else
                {
                    for (int i = 0; i < m_SelectedObjects.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(">   " + m_SelectedObjects[i].name, EditorStyles.miniLabel);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndScrollView(); 
        }
    }
}
#endif