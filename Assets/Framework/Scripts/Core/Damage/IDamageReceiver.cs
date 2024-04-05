using UnityEngine;
namespace Framework.Scripts.Core.Damage
{
    public interface IDamageReceiver
    {
        public GameObject gameObject { get; } 
        public void TakeDamage(IDamageProvider Provider, float DamageValue); 
    }
}