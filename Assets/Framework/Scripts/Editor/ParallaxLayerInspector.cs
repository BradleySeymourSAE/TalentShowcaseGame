using Framework.Scripts.Core;
using UnityEditor;
using UnityEngine;
namespace Framework.Scripts.Editor
{
	[CustomEditor(typeof(ParallaxLayer))]
	[CanEditMultipleObjects]
	public class ParallaxLayerInspector : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			// Parallax mode
			SerializedProperty rSerializedParallaxMode = serializedObject.FindProperty("ParallaxMode");
			EditorGUILayout.PropertyField(rSerializedParallaxMode);
			
			// Selection Mode Parameter
			if(rSerializedParallaxMode.hasMultipleDifferentValues == false)
			{
				ParallaxLayer.EParallaxMode eParallaxMode = (ParallaxLayer.EParallaxMode)rSerializedParallaxMode.intValue;
				
				switch(eParallaxMode)
				{
					case ParallaxLayer.EParallaxMode.Uniform:
					{
						EditorGUILayout.PropertyField(serializedObject.FindProperty("Parallax"), true);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("FreezeAxesSettings"), true);
					}
					break;
					
					case ParallaxLayer.EParallaxMode.ByAxis:
					{
						EditorGUILayout.PropertyField(serializedObject.FindProperty("ParallaxAxisSettings"), true);
					}
					break;
				}
			}
			
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			{			
				// Camera Selection Mode
				SerializedProperty rSerializedMasterCameraSelectionMode = serializedObject.FindProperty("SelectionMode");
				EditorGUILayout.PropertyField(rSerializedMasterCameraSelectionMode, true);
				
				// Selection Mode Parameter
				if(rSerializedMasterCameraSelectionMode.hasMultipleDifferentValues == false)
				{
					ParallaxLayer.EMasterCameraSelectionMode eSelectionMode = (ParallaxLayer.EMasterCameraSelectionMode)rSerializedMasterCameraSelectionMode.intValue;
					
					switch(eSelectionMode)
					{
						case ParallaxLayer.EMasterCameraSelectionMode.ByReference:
						{
							EditorGUILayout.PropertyField(serializedObject.FindProperty("MasterCameraReference"), true);
						}
						break;
						
						case ParallaxLayer.EMasterCameraSelectionMode.ByName:
						{
							EditorGUILayout.PropertyField(serializedObject.FindProperty("MasterCameraName"), true);
						}
						break;
						
						case ParallaxLayer.EMasterCameraSelectionMode.ByTag:
						{
							EditorGUILayout.PropertyField(serializedObject.FindProperty("MasterCameraTag"), true);
						}
						break;
					}
					
					if(eSelectionMode != ParallaxLayer.EMasterCameraSelectionMode.ByReference && Application.isPlaying)
					{
						EditorGUILayout.PropertyField(serializedObject.FindProperty("MasterCameraReference"), true);
					}
				}
			}
			
			serializedObject.ApplyModifiedProperties();
		}
	}
}