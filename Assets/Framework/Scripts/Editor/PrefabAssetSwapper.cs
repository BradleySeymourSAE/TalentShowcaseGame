#if UNITY_EDITOR
using System.Collections.Generic;
using Framework.Common;
using Framework.Scripts.Common;
using UnityEditor;
using UnityEngine;

namespace Framework.Scripts.Editor
{

    public class PrefabAssetReplacer : EditorWindow
    {
        [SerializeField] private GameObject Prefab;
       
        private List<GameObject> m_SelectedObjects = new();
        private GUIContent content;
        private Vector2 m_HeirarchySelectionScrollPosition; 

        [MenuItem("Framework/Prefab Asset Replacer")]
        private static void CreateReplaceWithPrefab()
        {
            EditorWindow.GetWindow<PrefabAssetReplacer>();
        }

        private void OnGUI()
        {
            Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", Prefab, typeof(GameObject), false);

            if (GUILayout.Button("Replace"))
            {
                GameObject[] selection = Selection.gameObjects;
                for (int i = selection.Length - 1; i >= 0; --i)
                {
                    PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(Prefab);
                    GameObject newObject = null; 

                    if (prefabType == PrefabAssetType.NotAPrefab)
                    {
                        continue;
                    }
                    if (prefabType is PrefabAssetType.Regular or PrefabAssetType.Variant)
                    {
                        newObject = (GameObject)PrefabUtility.InstantiatePrefab(Prefab);
                    }
                    else
                    {
                        newObject = Instantiate(Prefab);
                        newObject.name = Prefab.name;
                    }
                    if (newObject == null)
                    {
                       console.error(this, "Error instantiating prefab");
                        break;
                    }
                    Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
                    newObject.transform.parent = selection[i].transform.parent;
                    newObject.transform.localPosition = selection[i].transform.localPosition;
                    newObject.transform.localRotation = selection[i].transform.localRotation;
                    newObject.transform.localScale = selection[i].transform.localScale;
                    newObject.transform.SetSiblingIndex(selection[i].transform.GetSiblingIndex());
                    Undo.DestroyObjectImmediate(selection[i]);
                }
            }
            GUI.enabled = false;
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