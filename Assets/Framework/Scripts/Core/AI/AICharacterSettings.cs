using System;
using Animancer;
using Framework.Scripts.Core.AI.Strategies;
using UnityEngine;
namespace Framework.Scripts.Core.AI
{
    [CreateAssetMenu(fileName = "AICharacterSettings", menuName = "AI/Character Settings")]
    public class AICharacterSettings : CharacterSettings
    {
        // Movement 
        public bool AllowMovement = true; 
        public float RunningSpeed = 10.0f;
        public float RunningAccelerationAmount = 0.5f; 
        public float RunningDecelerationAmount = 0.5f; 
        // Turn Settings 
        public float LookAtTargetThreshold = 0.5f;
        public float TurnCooldownTime = 1.0f; 
        public float DamageBufferTime = 0.35f; 

        // Attack 
        public WeaponStrategy[] AttackStrategies = Array.Empty<WeaponStrategy>();
        
        // Vision Settings 
        public float VisionAngle = 45.0f;
        public float VisionRange = 10.0f;
        public LayerMask VisionLayerMask; 
        public Color VisionColor = new Color(1f, 0f, 0f, 0.25f);

        public int ScoreValue = 100;
    }

}