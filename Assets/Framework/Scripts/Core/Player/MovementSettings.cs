using Framework.Scripts.Common;
using Framework.Scripts.Common.Attributes;
using UnityEditor;
using UnityEngine;
namespace Framework.Scripts.Core.Player
{
    [CreateAssetMenu(fileName = "Movement Settings", menuName = "Player/Movement Settings"), ExecuteInEditMode] 
    public class MovementSettings : ScriptableObject
    {
        [Readonly] public float GravityMagnitude;
        [Readonly] public float GravityScale;
        [Readonly] public float RunningAccelerationAmount;
        [Readonly] public float RunningDecelerationAmount;
        [Readonly] public float InitialJumpForce;
     
        // Movement Settings 
        public float MaximumWalkingSpeed = 10.0f; 
        public float MaximumRunningSpeed = 25.0f;
        public float RunningAcceleration = 2.5f;
        public float RunningDeceleration = 5.0f;
        public float AirborneAccelerationFactor = 0.65f;
        public float AirborneDecelerationFactor = 0.65f;

        // Falling Settings
        public float NormalFallGravityMultiplier = 1.5f;
        public float MaximumNormalFallSpeed = 25.0f;
        public float QuickFallGravityMultiplier = 2.0f;
        public float MaximumQuickFallSpeed = 30.0f;

        // Jump Settings
        public bool UseConserveMomentum = true;
        public float JumpPeakHeight = 3.5f;
        public float JumpRiseDuration = 0.3f;
        public float JumpCancelGravityMultiplier = 2.0f;
        public float MidAirHangGravityMultiplier = 0.5f;
        public float JumpAirHangTimeLimit = 1.0f;
        public float MidAirHangAccelerationMultiplier = 1.1f;
        public float MidAirHangMaxSpeedMultiplier = 1.3f;
        public float CoyoteTime = 0.1f;
        public float JumpInputBufferTime = 0.1f;
        
        // Wall Jump Settings 
        public Vector2 WallJumpForce = new Vector2(15.0f, 25.0f); 
        public float WallJumpMovementSmoothingFactor = 0.5f;
        public float WallJumpTime = 0.15f;
        public bool TurnCharacterOnWallJump;
        
        // Wall Slide Settings 
        public float MaxWallSlideSpeed;
        public float WallSlideAcceleration;
        
        // Dash Settings
        public int MaximumNumberOfDashes = 1;
        public float DashMaxSpeed = 20.0f;
        public float DashSleepTime = 0.05f;
        public float DashEngageDuration = 0.15f;
        public float DashEndTime = 0.15f;
        public Vector2 DashEndSpeed = new Vector2(15.0f, 15.0f); 
        public float DashEndedMovementSmoothingFactor = 0.5f;
        public float DashRechargeDuration = 0.1f;
        public float DashInputBufferDuration = 0.1f;
        
        // Climbing 
        public LayerMask ClimbingLayerMask; 
        public float ClimbingSpeed = 5.0f;
        public float ClimbGrabbingTime = 0.25f;
        
        // Raycast Checks 
        public Vector2 GroundCheckScale = new Vector2(0.49f, 0.03f);
        public Vector2 WallCheckScale = new Vector2(0.5f, 1f);
        public LayerMask GroundLayerMask;
        
        // Tilting
        public float MaxTilt = 10.0f;
        public float TiltSpeed = 0.08f;

        
        #if UNITY_EDITOR 
        private void OnValidate()
        {
            if (JumpRiseDuration != 0 && Physics2D.gravity.y != 0)
            {
                GravityMagnitude = -(2.0f * JumpPeakHeight) / (JumpRiseDuration * JumpRiseDuration);
                GravityScale = GravityMagnitude / Physics2D.gravity.y;
            }
            RunningAccelerationAmount = (50 * RunningAcceleration) / MaximumRunningSpeed;
            RunningDecelerationAmount = (50 * RunningDeceleration) / MaximumRunningSpeed;
            InitialJumpForce = Mathf.Abs(GravityMagnitude) * JumpRiseDuration;
            RunningAcceleration = Mathf.Clamp(RunningAcceleration, 0.01f, MaximumRunningSpeed);
            RunningDeceleration = Mathf.Clamp(RunningDeceleration, 0.01f, MaximumRunningSpeed);
        }
        #endif 
    }
    
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(MovementSettings))]
    public class MovementSettingsInspector : UnityEditor.Editor
    {
        private MovementSettings Target => target as MovementSettings;
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
                // Display the readonly fields 
                content = new GUIContent($"Gravity Magnitude: {Target.GravityMagnitude}");
                EditorGUILayout.LabelField(content, EditorStyles.boldLabel);
                content = new GUIContent($"Gravity Scale: {Target.GravityScale}"); 
                EditorGUILayout.LabelField(content, EditorStyles.boldLabel); 
                content = new GUIContent($"Running Acceleration Amount: {Target.RunningAccelerationAmount}"); 
                EditorGUILayout.LabelField(content, EditorStyles.boldLabel); 
                content = new GUIContent($"Running Deceleration Amount: {Target.RunningDecelerationAmount}"); 
                EditorGUILayout.LabelField(content, EditorStyles.boldLabel); 
                content = new GUIContent($"Initial Jump Force: {Target.InitialJumpForce}");
                EditorGUILayout.LabelField(content, EditorStyles.boldLabel); 
            }
            EditorGUILayout.EndVertical(); 

            EditorGUILayout.BeginVertical(Styles.Skin.box);
            {
                content = new GUIContent("Movement Settings", "Settings for the player's movement");
                EditorGUILayout.LabelField(content, EditorStyles.boldLabel); 
                
                content = new GUIContent("Maximum Walking Speed", "The maximum speed the player can walk at"); 
                Target.MaximumWalkingSpeed = EditorGUILayout.FloatField(content, Target.MaximumWalkingSpeed); 
                
                content = new GUIContent("Maximum Running Speed", "The maximum speed the player can run at"); 
                Target.MaximumRunningSpeed = EditorGUILayout.FloatField(content, Target.MaximumRunningSpeed); 
                
                content = new GUIContent("Running Acceleration", "The rate at which the player accelerates when running"); 
                Target.RunningAcceleration = EditorGUILayout.FloatField(content, Target.RunningAcceleration); 
                
                content = new GUIContent("Running Deceleration", "The rate at which the player decelerates when running"); 
                Target.RunningDeceleration = EditorGUILayout.FloatField(content, Target.RunningDeceleration); 
                
                content = new GUIContent("Airborne Acceleration Factor", "The factor by which the player's acceleration is reduced when airborne"); 
                Target.AirborneAccelerationFactor = EditorGUILayout.Slider(content, Target.AirborneAccelerationFactor, 0.0f, 1.0f); 
                
                content = new GUIContent("Airborne Deceleration Factor", "The factor by which the player's deceleration is reduced when airborne"); 
                Target.AirborneDecelerationFactor = EditorGUILayout.Slider(content, Target.AirborneDecelerationFactor, 0.0f, 1.0f);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(Styles.FoldoutItemSpace);


            EditorGUILayout.BeginVertical(Styles.Skin.box);
            {
                content = new GUIContent("Falling Settings", "Settings for when the player's falling"); 
                EditorGUILayout.LabelField(content, EditorStyles.boldLabel); 
                
                content = new GUIContent("Normal Fall Gravity Multiplier", "The multiplier for the player's gravity when falling normally"); 
                Target.NormalFallGravityMultiplier = EditorGUILayout.FloatField(content, Target.NormalFallGravityMultiplier);
                
                content = new GUIContent("Maximum Normal Fall Speed", "The maximum speed the player can fall at normally");
                Target.MaximumNormalFallSpeed = EditorGUILayout.FloatField(content, Target.MaximumNormalFallSpeed);
                
                content = new GUIContent("Quick Fall Gravity Multiplier", "The multiplier for the player's gravity when falling quickly");
                Target.QuickFallGravityMultiplier = EditorGUILayout.FloatField(content, Target.QuickFallGravityMultiplier);
                
                content = new GUIContent("Maximum Quick Fall Speed", "The maximum speed the player can fall at quickly");
                Target.MaximumQuickFallSpeed = EditorGUILayout.FloatField(content, Target.MaximumQuickFallSpeed);
            }
            EditorGUILayout.EndVertical(); 
            
            EditorGUILayout.Space(Styles.FoldoutItemSpace); 
            
            EditorGUILayout.BeginVertical(Styles.Skin.box);
            {
                content = new GUIContent("Jump Settings", "Settings for the player's jumping"); 
                EditorGUILayout.LabelField(content, EditorStyles.boldLabel); 
                
                content = new GUIContent("Use Conserve Momentum", "Whether the player should conserve momentum when jumping");
                Target.UseConserveMomentum = EditorGUILayout.Toggle(content, Target.UseConserveMomentum);
                
                content = new GUIContent("Jump Peak Height", "The height the player jumps to at the peak of the jump");
                Target.JumpPeakHeight = EditorGUILayout.FloatField(content, Target.JumpPeakHeight);
                
                content = new GUIContent("Jump Rise Duration", "The duration of the jump's rise");
                Target.JumpRiseDuration = EditorGUILayout.FloatField(content, Target.JumpRiseDuration);
                
                content = new GUIContent("Jump Cancel Gravity Multiplier", "The multiplier for the player's gravity when cancelling a jump");
                Target.JumpCancelGravityMultiplier = EditorGUILayout.FloatField(content, Target.JumpCancelGravityMultiplier);
                
                content = new GUIContent("Mid Air Hang Gravity Multiplier", "The multiplier for the player's gravity when hanging in mid-air");
                Target.MidAirHangGravityMultiplier = EditorGUILayout.Slider(content, Target.MidAirHangGravityMultiplier, 0.0f, 1.0f);
                
                content = new GUIContent("Jump Air Hang Time Limit", "The time limit for how long the player can hang in mid-air");
                Target.JumpAirHangTimeLimit = EditorGUILayout.FloatField(content, Target.JumpAirHangTimeLimit);
                
                content = new GUIContent("Mid Air Hang Acceleration Multiplier", "The multiplier for the player's acceleration when hanging in mid-air");
                Target.MidAirHangAccelerationMultiplier = EditorGUILayout.FloatField(content, Target.MidAirHangAccelerationMultiplier);
                
                content = new GUIContent("Mid Air Hang Max Speed Multiplier", "The multiplier for the player's maximum speed when hanging in mid-air");
                Target.MidAirHangMaxSpeedMultiplier = EditorGUILayout.FloatField(content, Target.MidAirHangMaxSpeedMultiplier);
                
                content = new GUIContent("Coyote Time", "The time the player has to jump after leaving a platform"); 
                Target.CoyoteTime = EditorGUILayout.Slider(content, Target.CoyoteTime, 0.01f, 0.5f);
                
                content = new GUIContent("Jump Input Buffer Time", "The time the player has to jump after pressing the jump button");
                Target.JumpInputBufferTime = EditorGUILayout.Slider(content, Target.JumpInputBufferTime, 0.01f, 0.5f);
            } 
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(Styles.FoldoutItemSpace); 
            
            EditorGUILayout.BeginVertical(Styles.Skin.box);
            {
                content = new GUIContent("Wall Jump Settings", "Settings for the player's wall jumping"); 
                EditorGUILayout.LabelField(content, EditorStyles.boldLabel); 
                
                content = new GUIContent("Wall Jump Force", "The force applied to the player when wall jumping");
                Target.WallJumpForce = EditorGUILayout.Vector2Field(content, Target.WallJumpForce);
                
                content = new GUIContent("Wall Jump Movement Smoothing Factor", "The factor by which the player's movement is smoothed when wall jumping");
                Target.WallJumpMovementSmoothingFactor = EditorGUILayout.Slider(content, Target.WallJumpMovementSmoothingFactor, 0.0f, 1.0f);
                
                content = new GUIContent("Wall Jump Time", "The duration of the wall jump");
                Target.WallJumpTime = EditorGUILayout.Slider(content, Target.WallJumpTime, 0.0f, 1.5f);
                
                content = new GUIContent("Turn Character On Wall Jump", "Whether the player should turn when wall jumping");
                Target.TurnCharacterOnWallJump = EditorGUILayout.Toggle(content, Target.TurnCharacterOnWallJump);
                
                content = new GUIContent("Max Wall Slide Speed", "The maximum speed the player can slide down a wall");
                Target.MaxWallSlideSpeed = EditorGUILayout.FloatField(content, Target.MaxWallSlideSpeed);
                
                content = new GUIContent("Wall Slide Acceleration", "The rate at which the player accelerates when sliding down a wall");
                Target.WallSlideAcceleration = EditorGUILayout.FloatField(content, Target.WallSlideAcceleration);
            } 
            EditorGUILayout.EndVertical(); 
            
            EditorGUILayout.Space(Styles.FoldoutItemSpace); 
            
            EditorGUILayout.BeginVertical(Styles.Skin.box);
            {
                content = new GUIContent("Dash Settings", "Settings for the player's dashing"); 
                EditorGUILayout.LabelField(content, EditorStyles.boldLabel); 
                
                content = new GUIContent("Maximum Number Of Dashes", "The maximum number of dashes the player can perform");
                Target.MaximumNumberOfDashes = EditorGUILayout.IntField(content, Target.MaximumNumberOfDashes);
                
                content = new GUIContent("Dash Max Speed", "The maximum speed the player can dash at");
                Target.DashMaxSpeed = EditorGUILayout.FloatField(content, Target.DashMaxSpeed);
                
                content = new GUIContent("Dash Sleep Time", "The time the player has to wait before dashing again");
                Target.DashSleepTime = EditorGUILayout.FloatField(content, Target.DashSleepTime);
                
                content = new GUIContent("Dash Engage Duration", "The duration of the dash's engage");
                Target.DashEngageDuration = EditorGUILayout.FloatField(content, Target.DashEngageDuration);
                
                content = new GUIContent("Dash End Time", "The duration of the dash's end");
                Target.DashEndTime = EditorGUILayout.FloatField(content, Target.DashEndTime);
                
                content = new GUIContent("Dash End Speed", "The speed the player ends the dash at");
                Target.DashEndSpeed = EditorGUILayout.Vector2Field(content, Target.DashEndSpeed);
                
                content = new GUIContent("Dash Ended Movement Smoothing Factor", "The factor by which the player's movement is smoothed when the dash ends");
                Target.DashEndedMovementSmoothingFactor = EditorGUILayout.Slider(content, Target.DashEndedMovementSmoothingFactor, 0.0f, 1.0f);
                
                content = new GUIContent("Dash Recharge Duration", "The duration of the dash's recharge");
                Target.DashRechargeDuration = EditorGUILayout.FloatField(content, Target.DashRechargeDuration);
                
                content = new GUIContent("Dash Input Buffer Duration", "The time the player has to dash after pressing the dash button");
                Target.DashInputBufferDuration = EditorGUILayout.Slider(content, Target.DashInputBufferDuration, 0.01f, 0.5f);
            }
            EditorGUILayout.EndVertical(); 
            EditorGUILayout.Space(Styles.FoldoutItemSpace); 
            
            EditorGUILayout.BeginVertical(Styles.Skin.box);
            {
                content = new GUIContent("Climbing Settings", "Settings for the player's climbing"); 
                EditorGUILayout.LabelField(content, EditorStyles.boldLabel); 
                
                content = new GUIContent("Ladder Climb Speed", "The speed the player climbs ladders at");
                Target.ClimbingSpeed = EditorGUILayout.FloatField(content, Target.ClimbingSpeed);
                
                content = new GUIContent("Grabbing Time", "The time the player has to grab a ladder");
                Target.ClimbGrabbingTime = EditorGUILayout.FloatField(content, Target.ClimbGrabbingTime);
                
                content = new GUIContent("Ladder Layer Mask", "The layer mask for ladders");
                Target.ClimbingLayerMask = EditorExtensions.LayerMaskField(content.text, Target.ClimbingLayerMask);
            }
            EditorGUILayout.EndVertical(); 
            EditorGUILayout.Space(Styles.FoldoutItemSpace); 
            
            EditorGUILayout.BeginVertical(Styles.Skin.box);
            {
                content = new GUIContent("Ground Check Settings", "Settings for the player's ground checks"); 
                EditorGUILayout.LabelField(content, EditorStyles.boldLabel); 
                
                content = new GUIContent("Ground Check Scale", "The scale of the ground check");
                Target.GroundCheckScale = EditorGUILayout.Vector2Field(content, Target.GroundCheckScale);
                
                content = new GUIContent("Wall Check Scale", "The scale of the wall check");
                Target.WallCheckScale = EditorGUILayout.Vector2Field(content, Target.WallCheckScale);
                
                content = new GUIContent("Ground Layer Mask", "The layer mask for the ground");
                Target.GroundLayerMask = EditorExtensions.LayerMaskField(content.text, Target.GroundLayerMask); 
            } 
            EditorGUILayout.EndVertical(); 
            EditorGUILayout.Space(Styles.FoldoutItemSpace);

            EditorGUILayout.BeginVertical(Styles.Skin.box);
            {
                content = new GUIContent("Tilting Settings", "Settings for the player's tilting"); 
                EditorGUILayout.LabelField(content, EditorStyles.boldLabel); 
                
                content = new GUIContent("Max Tilt", "The maximum tilt the player can have"); 
                Target.MaxTilt = EditorGUILayout.FloatField(content, Target.MaxTilt); 
                
                content = new GUIContent("Tilt Speed", "The speed at which the player tilts"); 
                Target.TiltSpeed = EditorGUILayout.Slider(content, Target.TiltSpeed, 0.0f, 1.0f); 
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties(); 
        }
    }
    #endif 
}