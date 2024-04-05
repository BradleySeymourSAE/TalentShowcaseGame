using UnityEngine;
namespace Framework.Scripts.Core.AI
{
    public abstract class CharacterSettings : ScriptableObject
    {
        public float MaxHealth = 100.0f;
        public float HealthRecoveryRate = 1.0f; 
        public float HealthRecoveryDelay = 5.0f; 
    }
}