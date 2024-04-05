using UnityEngine;
namespace Framework.Scripts.Core.Damage
{
    public interface IDamageProvider
    {
        public GameObject gameObject { get; }
        public float GetDamage() { return 5.0f; } 
        public float GetForce() { return 5.0f; } 
        void OnDamageDealt(float Damage) {}
        void OnDamageDealt(IDamageReceiver Receiver, float Damage){ }
    }
}