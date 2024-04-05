using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Framework.Scripts.Common;
using GUIContent = UnityEngine.GUIContent;
#if UNITY_EDITOR
using UnityEditor;

namespace Framework.Common.Editors
{
    public class RecursiveAssetRenaming : EditorWindow
    {
        private static RecursiveAssetRenaming window;
        private List<string> m_SelectedAssetPaths = new();
        private List<GameObject> m_SelectedObjects = new(); 
        private Vector2 m_ProjectWindowSelectionPosition;
        private Vector2 m_HeirarchySelectionScrollPosition; 
       
        private string m_FindString;
        private string m_ReplaceString;
        private string m_AddToStartString;
        private string m_AddToEndString;
        private string m_WhitespaceCharacter;
        private int m_IndexedDigits;

        private int m_RemoveFromStartCount;
        private int m_RemoveFromEndCount;
       
        private GUIContent content;

        [MenuItem("Framework/Recursive Asset Renamer")]
        private static void ShowEditor()
        {
            window = EditorWindow.GetWindow<RecursiveAssetRenaming>(false, "Recursive Asset Renamer");
            window.Show(true);
            window.m_WhitespaceCharacter = "_";
            window.m_IndexedDigits = 2;
            window.m_FindString = Selection.activeObject != null ? Selection.activeObject.name : string.Empty;

        }

        private static void FormatAssetName_Internal(string assetName, ref string newAssetName)
        {
            if (assetName.Contains('_'))
            {
                int lastIndexOfUnderscore = assetName.LastIndexOf('_');
                newAssetName = newAssetName.Insert(lastIndexOfUnderscore, "_");
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
            m_SelectedObjects.Clear(); 
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
                    m_SelectedObjects.Add(target); 
                }
            }
            Repaint();
        }
        
        /// <summary>
        ///     Gets the asset name from the given path 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
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

        // private void DrawToPascalCaseAction()
        // {
        //     EditorGUILayout.Separator();
        //     EditorGUILayout.LabelField("Pascal Case", EditorStyles.boldLabel);
        //     EditorGUILayout.Separator();
        //     content = new GUIContent("To Pascal Case", "Converts the asset to PascalCase (TitleCase)");
        //     if (GUILayout.Button(content))
        //     {
        //         for (int i = 0; i < m_SelectedAssetPaths.Count; i++)
        //         {
        //             string assetName = GetAssetNameFromPath(m_SelectedAssetPaths[i]);
        //             string newAssetName = assetName.ToPascalCaseString();
        //             FormatAssetName_Internal(assetName, ref newAssetName);
        //             if (newAssetName != assetName)
        //             {
        //                 AssetDatabase.RenameAsset(m_SelectedAssetPaths[i], newAssetName);
        //             }
        //         }
        //         for (int i = 0; i < m_SelectedObjects.Count; ++i)
        //         {
        //             string assetName = m_SelectedObjects[i].name;
        //             string newAssetName = assetName.ToPascalCaseString();
        //             FormatAssetName_Internal(assetName, ref newAssetName);
        //             if (newAssetName != assetName)
        //             {
        //                 m_SelectedObjects[i].name = newAssetName;
        //             }
        //         }
        //         UpdateSelectedAssetPaths();
        //     }
        //     EditorGUILayout.Separator();
        // }
        //
        // private void DrawToCamelCaseAction()
        // {
        //     EditorGUILayout.Separator();
        //     EditorGUILayout.LabelField("Camel Case", EditorStyles.boldLabel);
        //     EditorGUILayout.Separator();
        //     content = new GUIContent("To Camel Case", "Converts the asset to camelCase");
        //    if (GUILayout.Button(content))
        //     {
        //         for (int i = 0; i < m_SelectedAssetPaths.Count; i++)
        //         {
        //             string assetName = GetAssetNameFromPath(m_SelectedAssetPaths[i]);
        //             string newAssetName = assetName.ToCamelCaseString();
        //             FormatAssetName_Internal(assetName, ref newAssetName);
        //             if (newAssetName != assetName)
        //             {
        //                 AssetDatabase.RenameAsset(m_SelectedAssetPaths[i], newAssetName);
        //             }
        //         }
        //         for (int i = 0; i < m_SelectedObjects.Count; ++i)
        //         {
        //             string assetName = m_SelectedObjects[i].name;
        //             string newAssetName = assetName.ToCamelCaseString();
        //             FormatAssetName_Internal(assetName, ref newAssetName);
        //             if (newAssetName != assetName)
        //             {
        //                 m_SelectedObjects[i].name = newAssetName;
        //             }
        //         }
        //         UpdateSelectedAssetPaths();
        //     }
        //     EditorGUILayout.Separator();
        // }

        private void DrawRemoveParenthesisAction()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Remove Parenthesis", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            content = new GUIContent("Remove Parenthesis", "Removes paranthesis from the asset name");
           if (GUILayout.Button(content))
            {
                Regex patternExpression = new Regex(@"\((.*?)\)");

                for (int i = 0; i < m_SelectedAssetPaths.Count; ++i)
                {
                    string asset = GetAssetNameFromPath(m_SelectedAssetPaths[i]);
                    if (string.IsNullOrEmpty(asset))
                    {
                        continue;
                    }
                    if (patternExpression.IsMatch(asset))
                    {
                        AssetDatabase.RenameAsset(m_SelectedAssetPaths[i], patternExpression.Replace(asset, string.Empty));
                    }
                }
                for (int i = 0; i < m_SelectedObjects.Count; ++i)
                {
                    if (m_SelectedObjects[i] == null)
                    {
                        continue;
                    }
                    string asset = m_SelectedObjects[i].name;
                    if (string.IsNullOrEmpty(asset))
                    {
                        continue;
                    }
                    if (patternExpression.IsMatch(asset))
                    {
                        m_SelectedObjects[i].name = patternExpression.Replace(asset, string.Empty);
                    }
                }
                UpdateSelectedAssetPaths();
            }
            EditorGUILayout.Separator();
        }

        private void DrawReplaceWhitespaceWithCharacterAction()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Trim Whitespace", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            content = new GUIContent("Replace Whitespace with Character", "Replaces all whitespace with the given character");
            m_WhitespaceCharacter = EditorGUILayout.TextField(content, m_WhitespaceCharacter);
            EditorGUILayout.Separator();
           if (GUILayout.Button(content))
            {
                for (int i = 0; i < m_SelectedAssetPaths.Count; ++i)
                {
                    string assetName = GetAssetNameFromPath(m_SelectedAssetPaths[i]);
                    if (string.IsNullOrWhiteSpace(m_WhitespaceCharacter))
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(assetName))
                    {
                        console.warn(this, "Asset name is null or empty");
                        continue;
                    }
                    string newAssetName = assetName.Replace(" ", m_WhitespaceCharacter);
                    if (newAssetName == assetName)
                    {
                        continue;
                    }
                    AssetDatabase.RenameAsset(m_SelectedAssetPaths[i], newAssetName);
                }
                // Now check the selected objects, if they aren't null or empty
                if (m_SelectedObjects != null && m_SelectedObjects.Count > 0)
                {
                    for (int x = 0; x < m_SelectedObjects.Count; ++x)
                    {
                        string candidateName = m_SelectedObjects[x].name;
                        if (string.IsNullOrEmpty(candidateName))
                        {
                            continue;
                        }
                        m_SelectedObjects[x].name = m_SelectedObjects[x].name.Replace(" ", m_WhitespaceCharacter);
                    }
                }
                UpdateSelectedAssetPaths();
            }
            EditorGUILayout.Separator();
        }

        private void DrawFindReplaceAction()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Find & Replace:", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            content = new GUIContent("Find String", "Find string in selected assets.");
            m_FindString = EditorGUILayout.TextField(content, m_FindString);
            EditorGUILayout.Separator(); 
            content = new GUIContent("Replace String", "Replace string in selected assets.");
            m_ReplaceString = EditorGUILayout.TextField(content, m_ReplaceString);
            EditorGUILayout.Separator();
            content = new GUIContent("Find and Replace", "Find and replace string in selected assets.");
            if (GUILayout.Button(content))
            {
                for (int i = 0; i < m_SelectedAssetPaths.Count; i++)
                {
                    string assetName = GetAssetNameFromPath(m_SelectedAssetPaths[i]);
                    if (assetName.Contains(m_FindString))
                    {
                        AssetDatabase.RenameAsset(m_SelectedAssetPaths[i], assetName.Replace(m_FindString, m_ReplaceString));
                    }
                }
                if (m_SelectedObjects != null && m_SelectedObjects.Count > 0)
                {
                    for (int i = 0; i < m_SelectedObjects.Count; i++)
                    {
                        if (m_SelectedObjects[i].name.Contains(m_FindString))
                        {
                            m_SelectedObjects[i].name = m_SelectedObjects[i].name.Replace(m_FindString, m_ReplaceString);
                        }
                    }
                } 
                UpdateSelectedAssetPaths();
            }
            EditorGUILayout.Separator();
        }

        private void DrawAddToStartSection()
        {
            EditorGUILayout.Separator(); 
            EditorGUILayout.LabelField("Add to Start:", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            content = new GUIContent("Add to Start String", "Add to start.");
            m_AddToStartString = EditorGUILayout.TextField(content, m_AddToStartString);
            EditorGUILayout.Separator();
            content = new GUIContent("Add to Start", "Add string to start for selected project window assets");
            if (GUILayout.Button(content))
            {
                for (int i = 0; i < m_SelectedAssetPaths.Count; i++)
                {
                    string assetName = GetAssetNameFromPath(m_SelectedAssetPaths[i]);
                    AssetDatabase.RenameAsset(m_SelectedAssetPaths[i], assetName.Insert(0, m_AddToStartString));
                }
                if (m_SelectedObjects != null && m_SelectedObjects.Count > 0)
                {
                    for (int i = 0; i < m_SelectedObjects.Count; i++)
                    {
                        m_SelectedObjects[i].name = m_SelectedObjects[i].name.Insert(0, m_AddToStartString);
                    }
                } 
                UpdateSelectedAssetPaths();
            }
            EditorGUILayout.Separator();
        }

        private void DrawAddToEnd()
        {
            EditorGUILayout.Separator(); 
            EditorGUILayout.LabelField("Add to End:", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            content = new GUIContent("Add to End String", "Add to end.");
            m_AddToEndString = EditorGUILayout.TextField(content, m_AddToEndString);
            EditorGUILayout.Separator();
            content = new GUIContent("Add to End", "Add string to end in selected assets.");
            if (GUILayout.Button(content))
            {
                for (int i = 0; i < m_SelectedAssetPaths.Count; i++)
                {
                    string assetName = GetAssetNameFromPath(m_SelectedAssetPaths[i]);
                    AssetDatabase.RenameAsset(m_SelectedAssetPaths[i], assetName + m_AddToEndString);
                }
                if (m_SelectedObjects != null && m_SelectedObjects.Count > 0)
                {
                    for (int i = 0; i < m_SelectedObjects.Count; ++i)
                    {
                        GameObject t = m_SelectedObjects[i]; 
                        t.name += m_AddToEndString; 
                    }
                }
                UpdateSelectedAssetPaths();
            }
            EditorGUILayout.Separator();
        }

        private void DrawRemoveFromStart()
        {
            EditorGUILayout.LabelField("Remove from Start:", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            content = new GUIContent("Remove from Start", "Remove x amount from the start of selected assets."); 
            m_RemoveFromStartCount = EditorGUILayout.IntField(content, m_RemoveFromStartCount);
            EditorGUILayout.Separator();
            content = new GUIContent("Remove From Start", "Remove these many characters from the start.");
            if (GUILayout.Button(content))
            {
                for (int i = 0; i < m_SelectedAssetPaths.Count; i++)
                {
                    string assetName = GetAssetNameFromPath(m_SelectedAssetPaths[i]);
                    AssetDatabase.RenameAsset(m_SelectedAssetPaths[i], assetName.Substring(m_RemoveFromStartCount, assetName.Length - m_RemoveFromStartCount));
                }
                UpdateSelectedAssetPaths();

                if (m_SelectedObjects != null && m_SelectedObjects.Count > 0)
                {
                    for (int i = 0; i < m_SelectedObjects.Count; ++i)
                    {
                        GameObject target = m_SelectedObjects[i]; 
                        target.name = target.name.Substring(m_RemoveFromStartCount, target.name.Length - m_RemoveFromStartCount); 
                    }
                }
            }
            EditorGUILayout.Separator();
        }

        private void DrawRemoveFromEnd()
        {
            EditorGUILayout.LabelField("Remove from End:", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            content = new GUIContent("Remove From End Count", "Remove from end.");
            m_RemoveFromEndCount = EditorGUILayout.IntField(content, m_RemoveFromEndCount);
            EditorGUILayout.Separator();
            content = new GUIContent("Remove From End", "Remove these many characters from the end.");
            if (GUILayout.Button(content))
            {
                for (int i = 0; i < m_SelectedAssetPaths.Count; i++)
                {
                    string assetName = GetAssetNameFromPath(m_SelectedAssetPaths[i]);
                    AssetDatabase.RenameAsset(m_SelectedAssetPaths[i], assetName[..^m_RemoveFromEndCount]);
                }
                if (m_SelectedObjects != null && m_SelectedObjects.Count > 0)
                {
                    for (int i = 0; i < m_SelectedObjects.Count; ++i)
                    {
                        GameObject t = m_SelectedObjects[i]; 
                        t.name = t.name[..^m_RemoveFromEndCount]; 
                    } 
                }
                UpdateSelectedAssetPaths();
            }
            EditorGUILayout.Separator();
        }

        private void DrawIndexedRenameAction()
        {
            EditorGUILayout.LabelField("Indexed Rename:", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            content = new GUIContent("Indexed Rename", "Rename selected assets with an index, to the specified number of digits");
            m_IndexedDigits = EditorGUILayout.IntField(content, m_IndexedDigits);
            EditorGUILayout.Separator();
            content = new GUIContent("Indexed Rename", "Rename selected assets with an index.");
            if (GUILayout.Button(content))
            {
                for (int i = 0; i < m_SelectedAssetPaths.Count; i++)
                {
                    string assetName = GetAssetNameFromPath(m_SelectedAssetPaths[i]);
                    if (m_IndexedDigits <= 0)
                    {
                        m_IndexedDigits = 1;
                    }
                    else if (m_IndexedDigits > 10)
                    {
                        content = new GUIContent($"Maximum number of digits has been reached! {m_IndexedDigits}/10");
                        EditorGUILayout.HelpBox(content.text, MessageType.Error);
                        continue;
                    }
                   // for each element in selected, rename it to the index in this format: someAsset_01, someAsset_011
                    bool hasIndexOfUnderScore = assetName.IndexOf("_", StringComparison.Ordinal) != -1;
                    if (hasIndexOfUnderScore)
                    {
                        string[] split = assetName.Split('_');
                        string newName = split[0] + "_" + i.ToString().PadLeft(m_IndexedDigits, '0');
                        AssetDatabase.RenameAsset(m_SelectedAssetPaths[i], newName);
                    }
                    else
                    {
                        string newName = assetName + "_" + i.ToString().PadLeft(m_IndexedDigits, '0');
                        AssetDatabase.RenameAsset(m_SelectedAssetPaths[i], newName);
                    }
                }
                if (m_SelectedObjects != null && m_SelectedObjects.Count > 0)
                {
                    for (int i = 0; i < m_SelectedObjects.Count; ++i)
                    {
                        GameObject t = m_SelectedObjects[i];
                        bool hasIndexOfUnderScore = t.name.IndexOf("_", StringComparison.Ordinal) != -1;
                        if (hasIndexOfUnderScore)
                        {
                            string[] split = t.name.Split('_');
                            t.name = split[0] + "_" + i.ToString().PadLeft(m_IndexedDigits, '0');
                        }
                        else
                        {
                            t.name = t.name + "_" + i.ToString().PadLeft(m_IndexedDigits, '0');
                        }
                    }
                }
                UpdateSelectedAssetPaths();
            }
        }
        
        /// <summary>
        ///     Draws the GUI for the editor window 
        /// </summary>
        private void OnGUI()
        {
            // Draw the controls
            DrawReplaceWhitespaceWithCharacterAction();
            DrawRemoveParenthesisAction();
            DrawIndexedRenameAction();
            DrawFindReplaceAction();
            DrawAddToStartSection();
            DrawAddToEnd();
            DrawRemoveFromStart();
            DrawRemoveFromEnd();
            DrawCapitalizeAndUnderscoreNumberedAction(); 

            // Draw the selected assets
            DrawSelectedAssetsList();
            DrawHeirachySelections(); 
        }

        private void DrawCapitalizeAndUnderscoreNumberedAction()
        {
            EditorGUILayout.Separator(); 
            content = new GUIContent("Capitalize and Underscore Numbered", "Capitalize and underscore numbered assets."); 
            EditorGUILayout.LabelField(content, EditorStyles.boldLabel); 
            EditorGUILayout.Separator();
            if (GUILayout.Button(content))
            {
                // Capitlize the first letter of the asset name 
                for (int i = 0; i < m_SelectedAssetPaths.Count; i++)
                {
                    string assetName = GetAssetNameFromPath(m_SelectedAssetPaths[i]);
                    // capitlize the first letter of the asset name 
                    string newAssetName = assetName;
                    // Check if the asset name contains a number, if it does then add an underscore before the number and ensure the format is _00 
                    // So plants5 becomes Plants_05 
                    const string pattern = @"\d+";
                    if (Regex.IsMatch(assetName, pattern))
                    {
                        int firstMatchIndex = Regex.Match(assetName, pattern).Index;
                        string beforeNumber = char.ToUpper(assetName[0]) + assetName[1..firstMatchIndex]; 
                        string afterNumber = assetName[firstMatchIndex..];
                        newAssetName = beforeNumber + "_" + afterNumber.PadLeft(2, '0');
                    }
                    AssetDatabase.RenameAsset(m_SelectedAssetPaths[i], newAssetName); 
                }
                UpdateSelectedAssetPaths();
            }
            EditorGUILayout.Separator(); 
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

        private void DrawHeirachySelections()
        {
            EditorGUILayout.Separator();
            content = new GUIContent("Heirarchy Selections:", "Selected assets in the HIERARCHY view.");
            EditorGUILayout.LabelField(content, EditorStyles.boldLabel);
            EditorGUILayout.Separator(); 
            m_HeirarchySelectionScrollPosition = EditorGUILayout.BeginScrollView(m_HeirarchySelectionScrollPosition);
            {
                if (m_SelectedObjects == null || m_SelectedObjects.Count == 0)
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
        
        private void OnFocus() => UpdateSelectedAssetPaths();
    }
}
#endif