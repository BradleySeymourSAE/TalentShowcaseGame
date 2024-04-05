#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;


namespace Framework.Scripts.Common
{
    public static class EditorExtensions
    {
        /// <summary>
        /// Creates a LayerMask field in an editor(EditorWindow, Editor).
        /// Unity is missing it, so there is the need to implement this handmade.
        /// Use example:
        /// private LayerMask layerMask = 0; // this has global scope
        /// 
        /// layerMask = CustomEditorExtension.LayerMaskField("Layer Mask: ", layerMask);
        /// </summary>
        /// <param name="label"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static LayerMask LayerMaskField(string label, LayerMask layerMask)
        {
            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "")
                {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }
            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                {
                    maskWithoutEmpty |= (1 << i);
                }
            }
            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());
            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                {
                    mask |= (1 << layerNumbers[i]);
                }
            }
            layerMask.value = mask;
            return layerMask;
        }

        public static System.Type GetType(SerializedProperty property)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            FieldInfo fi = parentType.GetFieldViaPath(property.propertyPath);
            return fi.FieldType;
        }

        public static FieldInfo GetFieldViaPath(this Type type, string path)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            Type parent = type;
            FieldInfo fi = parent.GetField(path, flags);
            string[] paths = path.Split('.');
            for (int i = 0; i < paths.Length; i++)
            {
                fi = parent.GetField(paths[i], flags);

                // there are only two container field type that can be serialized:
                // Array and List<T>
                if (fi.FieldType.IsArray)
                {
                    parent = fi.FieldType.GetElementType();
                    i += 2;
                    continue;
                }
                if (fi.FieldType.IsGenericType)
                {
                    parent = fi.FieldType.GetGenericArguments()[0];
                    i += 2;
                    continue;
                }
                if (fi != null)
                {
                    parent = fi.FieldType;
                }
                else
                {
                    return null;
                }
            }
            return fi;
        }

        public static string GetSelectedPathOrFallback()
        {
            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }

        public static void CreateAssetWithPath(SerializedProperty property, string selectedAssetPath)
        {
            Type type = GetType(property); //Get all the Types from an Abstract class
            if (type.IsAbstract)
            {
                List<Type> allTypes = EditorExtensions.GetAllTypes(type);
                GenericMenu addMenu = new GenericMenu();
                for (int i = 0; i < allTypes.Count; i++)
                {
                    Type st = allTypes[i];
                    string Rname = st.Name;
                    addMenu.AddItem(new GUIContent(Rname), false, () => CreateScriptableAsset(property, st, selectedAssetPath));
                }
                addMenu.ShowAsContext();
                EditorGUILayout.EndHorizontal();
                property.serializedObject.ApplyModifiedProperties();
                return;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                type = type.GetGenericArguments()[0];
            }
            property.objectReferenceValue = CreateAssetWithSavePrompt(type, selectedAssetPath);
        }

        public static void DrawObjectReferenceInspector(SerializedProperty property)
        {
            if (property != null && property.objectReferenceValue != null)
            {
                UnityEditor.Editor.CreateEditor(property.objectReferenceValue).OnInspectorGUI();
            }
        }

        public static List<Type> GetAllTypes(Type type)
        {
            // Get all the types that are in the same Assembly (all the runtime scripts) as the Reaction type.
            Type[] allTypes = type.Assembly.GetTypes();

            // Create an empty list to store all the types that are subtypes of Reaction.
            List<Type> SubTypeList = new List<Type>();

            // Go through all the types in the Assembly...
            for (int i = 0; i < allTypes.Length; i++)
            {
                // ... and if they are a non-abstract subclass of Reaction then add them to the list.
                if (allTypes[i].IsSubclassOf(type) && !allTypes[i].IsAbstract)
                {
                    SubTypeList.Add(allTypes[i]);
                }
            }

            // Convert the list to an array and store it.
            return SubTypeList;
        }


        public static void CreateScriptableAsset(SerializedProperty property, Type type, string selectedAssetPath)
        {
            property.objectReferenceValue = CreateAssetWithSavePrompt(type, selectedAssetPath);
            property.serializedObject.ApplyModifiedProperties();
        }

        public static void CreateScriptableAssetInternal(SerializedProperty property, Type type)
        {
            property.objectReferenceValue = ScriptableObject.CreateInstance(type);
            property.serializedObject.ApplyModifiedProperties();
        }

        public static void CreateScriptableAssetInternal(SerializedProperty property, Type type, string path)
        {
            ScriptableObject newInstance = ScriptableObject.CreateInstance(type);
            newInstance.hideFlags = HideFlags.None;
            newInstance.name = type.Name;
            property.objectReferenceValue = newInstance;
            property.serializedObject.ApplyModifiedProperties();
            AssetDatabase.AddObjectToAsset(newInstance, path);
            AssetDatabase.SaveAssets();
        }

        private static ScriptableObject CreateAssetWithSavePrompt(Type type, string path)
        {
            if (type == null)
            {
                return null; //HACK
            }
            string defaultName = string.Format("New {0}.asset", type.Name);
            string message = string.Format("Enter a file name for the {0} ScriptableObject.", type.Name);
            path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", defaultName, "asset", message, path);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            ScriptableObject asset = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            EditorGUIUtility.PingObject(asset);
            return asset;
        }

        public static List<T> GetAllInstances<T>() where T : ScriptableObject
        {
            #if UNITY_EDITOR
            if (Application.isEditor)
            {
                string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name); //FindAssets uses tags check documentation for more info
                T[] a = new T[guids.Length];
                for (int i = 0; i < guids.Length; i++) //probably could get optimized 
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                    a[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                }
                List<T> aA = Enumerable.ToList(a);
                return aA;
            }
            #endif
            return null;
        }
    }


    public static class Styles
    {
        [SerializeField] private static GUISkin s_GUISkin;
        public static GUISkin Skin
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    s_GUISkin = Resources.Load("UI Skin Dark") as GUISkin;
                }
                return s_GUISkin;
            }
        }
        public static GUIStyle FoldoutStyle = Skin.GetStyle(k_FoldoutStyleName);
        public static GUIStyle HeaderStyle = Skin.GetStyle(k_HeaderStyleName);
        public static GUIStyle DefaultHeaderStyle = Skin.GetStyle(k_DefaultHeaderStyleName);
        public static GUIStyle ToggleHelperStyle = Skin.GetStyle(k_ToggleHelperStyleName);
        public static float FoldoutItemSpace = 2.0f;
        public static float FoldoutTopSpace = 5.0f;
        public static float FoldoutBottomSpace = 2.0f;

        private static Color s_DefaultFontColor = Color.white;
        private static Color s_DefaultBackgroundColor = Color.black;

        public static GUILayoutOption ExpandedWidth => GUILayout.ExpandWidth(true);
        public static GUILayoutOption DontExpandWidth => GUILayout.ExpandWidth(false);
        public static GUILayoutOption ExpandedHeight => GUILayout.ExpandHeight(true);
        public static GUILayoutOption DontExpandHeight => GUILayout.ExpandHeight(false);


        private const string k_DefaultHeaderStyleName = "Default Header";
        private const string k_FoldoutStyleName = "UIM Foldout";
        private const string k_HeaderStyleName = "UIM Header";
        private const string k_ToggleHelperStyleName = "Toggle Helper";
        private const string k_ButtonStyleName = "Default Button";

        public static Color LightBlueColor = new Color(0.6627f, 0.7176f, 0.9373f);
        public static Color InputVariableColor = new Color(0.6902f, 0.5412f, 0.4549f);
        public static Color FunctionColor = new Color(0.2078f, 0.6118f, 0.6353f);
        public static Color KeywordColor = new Color(0.2588f, 0.3137f, 0.7333f);
        public static Color DarkBlueColor = new Color(0.1059f, 0.1333f, 0.451f);

        public static void SetFontColor(Color FontColor) => GUI.color = FontColor;
        public static void SetColor(Color FontColor) => GUI.color = FontColor;
        public static void SetBackgroundColor(Color BackgroundColor) => GUI.backgroundColor = BackgroundColor;

        public static void SetFontAndBackgroundColor(Color FontAndBackgroundColor)
        {
            SetFontAndBackgroundColor(FontAndBackgroundColor, FontAndBackgroundColor);
        }

        public static void SetFontAndBackgroundColor(Color FontColor, Color BackgroundColor)
        {
            SetFontColor(FontColor);
            SetBackgroundColor(BackgroundColor);
        }
    }

}
#endif 