using Framework.Scripts.Common;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif 
namespace Framework.Scripts.Core.Platforms
{
    [System.Serializable]
    public enum ERotationAxis { XAxis, YAxis, ZAxis }
    [System.Serializable]
    public enum EPlatformMovementMode { Linear, Circular }

    [CreateAssetMenu(fileName = "PlatformSettings", menuName = "Framework/Platform Settings", order = 0)]
    public class PlatformSettings : ScriptableObject
    {
        // Movement Settings 
        public bool UseMovement;
        public EPlatformMovementMode MovementMode = EPlatformMovementMode.Linear; 
        public float MovementTime = 3.0f;
        public Vector3 MovementOffset = new Vector3(0.0f, 5.0f, 0.0f);
        public float MovementRadius = 5.0f; 

        // Rotation Settings 
        public bool UseRotation; 
        public float RotationVelocity = 30.0f;
        public ERotationAxis RotationAxis = ERotationAxis.YAxis;
        public Vector3 RotationOffset;
        public bool UseLocalRotation;
        public bool UsePendulumRotation;
        public float PendulumRotationRange = 45.0f; 
        
        // Vanish settings 
        public bool UseVanish;
        
        public float VanishDuration = 3.0f; 
        public float VanishStartDelay = 1.0f;
        public float ReappearDelay = 1.0f; 
        public float ReappearDuration = 1.0f; 
        public bool ShouldReappear = true;
        public bool ShouldLoop = true; 
        public bool ShouldVanishOnStart = false;

    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(PlatformSettings))]
    public class PlatformSettingsInspector : UnityEditor.Editor
    {
        private PlatformSettings Target => target as PlatformSettings;
        private GUIContent content;
        

        public override void OnInspectorGUI()
        {
            if (Target == null)
            {
                return;
            }
            serializedObject.Update();

            EditorGUILayout.BeginVertical(Styles.Skin.box);
            {
                EditorGUILayout.Space(Styles.FoldoutTopSpace); 
                
                content = new GUIContent("Platform Settings", "Settings for the platform object");
                EditorGUILayout.LabelField(content.text, EditorStyles.boldLabel); 
                
                content = new GUIContent("Use Movement", "Should the platform move?"); 
                Target.UseMovement = EditorGUILayout.Toggle(content, Target.UseMovement);
                
                content = new GUIContent("Use Rotation", "Should the platform rotate?"); 
                Target.UseRotation = EditorGUILayout.Toggle(content, Target.UseRotation); 
                
                content = new GUIContent("Use Vanish", "Should the platform vanish?"); 
                Target.UseVanish = EditorGUILayout.Toggle(content, Target.UseVanish); 
                
                EditorGUILayout.Space(Styles.FoldoutBottomSpace); 
            } 
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(Styles.FoldoutItemSpace);

            if (Target.UseMovement)
            {
                EditorGUILayout.BeginVertical(Styles.Skin.box);
                {
                    EditorGUILayout.Space(Styles.FoldoutTopSpace); 
                    
                   content = new GUIContent("Movement Settings", "Settings for the movement of the platform object");
                   EditorGUILayout.LabelField(content.text, EditorStyles.boldLabel); 
                   
                   content = new GUIContent("Movement Mode", "The mode of movement for the platform object"); 
                   Target.MovementMode = (EPlatformMovementMode) EditorGUILayout.EnumPopup(content, Target.MovementMode); 
                   
                   content = new GUIContent("Movement Time", "The time it takes for the platform to move"); 
                   Target.MovementTime = EditorGUILayout.FloatField(content, Target.MovementTime);

                   if (Target.MovementMode is EPlatformMovementMode.Linear)
                   {
                       content = new GUIContent("Movement Offset", "The offset of the movement");
                       Target.MovementOffset = EditorGUILayout.Vector3Field(content, Target.MovementOffset);
                   } 
                   if (Target.MovementMode is EPlatformMovementMode.Circular)
                   {
                       content = new GUIContent("Movement Radius", "The radius of the movement"); 
                       Target.MovementRadius = EditorGUILayout.FloatField(content, Target.MovementRadius); 
                   }
                   
                   EditorGUILayout.Space(Styles.FoldoutBottomSpace);
                }
                EditorGUILayout.EndVertical();   
                EditorGUILayout.Space(Styles.FoldoutItemSpace);
            }

            if (Target.UseRotation)
            {
                EditorGUILayout.BeginVertical(Styles.Skin.box);
                {
                    EditorGUILayout.Space(Styles.FoldoutTopSpace); 
                
                    content = new GUIContent("Rotation Settings", "Settings for the rotation of the platform object"); 
                    EditorGUILayout.LabelField(content.text, EditorStyles.boldLabel); 
                    
                    content = new GUIContent("Rotation Velocity", "The velocity of the rotation"); 
                    Target.RotationVelocity = EditorGUILayout.FloatField(content, Target.RotationVelocity); 
                    content = new GUIContent("Rotation Axis", "The axis of the rotation"); 
                    Target.RotationAxis = (ERotationAxis) EditorGUILayout.EnumPopup(content, Target.RotationAxis); 
                    content = new GUIContent("Rotation Offset", "The offset of the rotation"); 
                    Target.RotationOffset = EditorGUILayout.Vector3Field(content, Target.RotationOffset); 
                    content = new GUIContent("Use Local Rotation", "Should the rotation be local?"); 
                    Target.UseLocalRotation = EditorGUILayout.Toggle(content, Target.UseLocalRotation); 
                    content = new GUIContent("Use Pendulum Rotation", "Should the rotation be a pendulum?"); 
                    Target.UsePendulumRotation = EditorGUILayout.Toggle(content, Target.UsePendulumRotation); 
                    
                    if (Target.UsePendulumRotation)
                    {
                        content = new GUIContent("Pendulum Rotation Range", "The range of the swinging pendulum rotation"); 
                        Target.PendulumRotationRange = EditorGUILayout.FloatField(content, Target.PendulumRotationRange); 
                    } 
                }
                EditorGUILayout.EndVertical(); 
                EditorGUILayout.Space(Styles.FoldoutItemSpace);
            }
            
            if (Target.UseVanish)
            {
                EditorGUILayout.BeginVertical(Styles.Skin.box);
                {
                    EditorGUILayout.Space(Styles.FoldoutTopSpace); 
                    
                    content = new GUIContent("Vanish Settings", "Settings for the vanish of the platform object"); 
                    EditorGUILayout.LabelField(content.text, EditorStyles.boldLabel); 
                    
                    content = new GUIContent("Should Vanish On Start", "Should the platform vanish on start?");
                    Target.ShouldVanishOnStart = EditorGUILayout.Toggle(content, Target.ShouldVanishOnStart); 
                    content = new GUIContent("Vanish Duration", "The duration of the vanish"); 
                    Target.VanishDuration = EditorGUILayout.FloatField(content, Target.VanishDuration); 
                    content = new GUIContent("Vanish Start Delay", "The time it takes before the vanish starts"); 
                    Target.VanishStartDelay = EditorGUILayout.FloatField(content, Target.VanishStartDelay); 
                    
                    
                    content = new GUIContent("Should Reappear", "Should the platform reappear?"); 
                    Target.ShouldReappear = EditorGUILayout.Toggle(content, Target.ShouldReappear); 
                    if (Target.ShouldReappear)
                    {
                        content = new GUIContent("Reappear Delay", "The delay before the platform reappears"); 
                        Target.ReappearDelay = EditorGUILayout.FloatField(content, Target.ReappearDelay); 
                        content = new GUIContent("Reappear Duration", "The duration of the reappear"); 
                        Target.ReappearDuration = EditorGUILayout.FloatField(content, Target.ReappearDuration); 
                        content = new GUIContent("Should Loop", "Should the platform loop the vanish and reappear?"); 
                        Target.ShouldLoop = EditorGUILayout.Toggle(content, Target.ShouldLoop); 
                    }
                }
                EditorGUILayout.EndVertical(); 
                EditorGUILayout.Space(Styles.FoldoutItemSpace);
            }
            serializedObject.ApplyModifiedProperties(); 
        }
    }
    #endif
}