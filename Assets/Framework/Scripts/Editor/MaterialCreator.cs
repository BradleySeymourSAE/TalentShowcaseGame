using System.Collections.Generic;
using Framework.Scripts.Common;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
namespace Framework.Common.Editors
{
    
    public class MaterialCreator : EditorWindow
    {
        private static MaterialCreator window;
        private List<string> m_SelectedAssetPaths = new();
        private Vector2 m_ProjectWindowSelectionPosition;
        private Vector2 m_HeirarchySelectionScrollPosition;
        private GUIContent content; 
        
        
        [MenuItem("Framework/Material Creator")] 
        private static void ShowEditor()
        {
            window = EditorWindow.GetWindow<MaterialCreator>(false, "Material Creator");
            window.Show(true);
            

        }

        private void OnGUI()
        {
            DrawCreateMaterialAction(); 
            DrawCreateQuadsFromSelectedMaterialsAction(); 
            ExportDDSTextureAsPNG(); 
            DrawSelectedAssetsList();
        }

        private void DrawCreateMaterialAction()
        {
            if (GUILayout.Button("Create Material"))
            {
                for (int i = 0; i < m_SelectedAssetPaths.Count; i++)
                {
                    string assetPath = m_SelectedAssetPaths[i];
                    // get the asset at the path and ensure it is a texture
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath); 
                    if (texture == null)
                    {
                        console.warn(this, "Asset at path is not a texture: ", assetPath); 
                        continue; 
                    } 
                    // create unlit transparent material 
                    Material material = new Material(Shader.Find("Unlit/Transparent")); 
                    material.name = "M_" + GetAssetNameFromPath(assetPath); 
                    // assign the texture to the material 
                    material.SetTexture("_MainTex", texture); 
                    string materialAssetPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(assetPath) + "/Materials/", material.name + ".mat");
                    AssetDatabase.CreateAsset(material, materialAssetPath);
                } 
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                UpdateSelectedAssetPaths(); 
            } 
        }

        private void DrawCreateQuadsFromSelectedMaterialsAction()
        {
            if (GUILayout.Button("Create Quads From Materials"))
            {
                for (int i = 0; i < m_SelectedAssetPaths.Count; i++)
                {
                   // check to ensure that the selected asset type is a material
                    string assetPath = m_SelectedAssetPaths[i];
                    Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath); 
                    if (material == null)
                    {
                        console.warn(this, "Asset at path is not a material: ", assetPath); 
                        continue; 
                    }
                    // create a quad with the material 
                    GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad); 
                    if (quad == null)
                    {
                        console.warn(this, "Failed to create quad for material: ", assetPath); 
                        continue; 
                    } 
                    quad.GetComponent<MeshRenderer>().material = material; 
                    quad.name = GetAssetNameFromPath(assetPath); 
                    if (quad.TryGetComponent(out MeshCollider collider))
                    {
                        DestroyImmediate(collider); 
                    }
                }
            } 
        }

        private void ExportDDSTextureAsPNG()
        {
            if (GUILayout.Button("Export DDS Texture As PNG"))
            {
                for (int i = 0; i < m_SelectedAssetPaths.Count; i++)
                {
                    string assetPath = m_SelectedAssetPaths[i];
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath); 
                    if (texture == null)
                    {
                        console.warn(this, "Asset at path is not a texture: ", assetPath); 
                        continue; 
                    }
                    byte[] bytes = texture.EncodeToPNG(); 
                    string pngAssetPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(assetPath) + "/Exported/", texture.name + ".png"); 
                    System.IO.File.WriteAllBytes(pngAssetPath, bytes); 
                    
                }
            }
        }

        private void OnSelectionChange()
        {
            UpdateSelectedAssetPaths();
        }
        
        /// <summary>
        ///     Updates the list of selected asset paths. 
        /// </summary>
        private void UpdateSelectedAssetPaths()
        {
            m_SelectedAssetPaths.Clear();
            for (int i = 0; i < Selection.assetGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]);
                m_SelectedAssetPaths.Add(path);
            }
            if (Selection.objects != null && Selection.objects.Length > 0)
            {
                for (int i = 0; i < Selection.objects.Length; ++i)
                {
                    if (Selection.objects[i].GetType() != typeof(GameObject))
                    {
                        continue; 
                    }
                    GameObject target = Selection.objects[i] as GameObject;
                    string name = target.name;
                    if (m_SelectedAssetPaths.Contains(name))
                    {
                        continue;
                    }
                }
            }
            Repaint();
        }
        
        private void DrawSelectedAssetsList()
        {
            EditorGUILayout.Separator();
            content = new GUIContent("Selected:", "Selected assets in the PROJECT view.");
            EditorGUILayout.LabelField(content, EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            m_ProjectWindowSelectionPosition = EditorGUILayout.BeginScrollView(m_ProjectWindowSelectionPosition);
            {
                if (m_SelectedAssetPaths == null || m_SelectedAssetPaths.Count == 0) 
                {
                    EditorGUILayout.LabelField("Please select some asset(s) in the project view.", EditorStyles.miniBoldLabel);
                }
                else
                {
                    for (int i = 0; i < m_SelectedAssetPaths.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(">   " + GetAssetNameFromPath(m_SelectedAssetPaths[i]), EditorStyles.miniLabel);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }
                
        private string GetAssetNameFromPath(string Path)
        {
            int lastIndexOfFolderSlash = Path.LastIndexOf('/') + 1;
            string assetName = Path.Substring(Path.LastIndexOf('/') + 1, Path.Length - lastIndexOfFolderSlash);
            if (assetName.Contains('.'))
            {
                assetName = assetName[..assetName.LastIndexOf('.')];
            }
            return assetName;
        }
    }
}
#endif 