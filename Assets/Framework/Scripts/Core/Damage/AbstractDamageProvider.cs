using UnityEngine;
namespace Framework.Scripts.Core.Damage
{
    /// <summary>
    ///     Provide's a base implementation properties for a component that is able to provide damage or force to other objects.
    /// </summary>
    public abstract class AbstractDamageProvider : MonoBehaviour, IDamageProvider
    {
        [field: SerializeField] public float Damage { get; set; } = 5.0f; 
        [field: SerializeField] public float Force { get; } = 5.0f; 

        /// <summary>
        ///     This method is called after this object has dealt damage to another object.
        ///     This can be used to apply additional effects to this component or modifying the next damage value.
        /// </summary>
        /// <param name="Receiver">The receiver that took damage from this component</param>
        /// <param name="Damage">The damage that the receiver took</param>
        protected virtual void OnDamageDealt_Internal(IDamageReceiver Receiver, float Damage)
        {

        }
        
        void IDamageProvider.OnDamageDealt(float Damage)
        {
            OnDamageDealt_Internal(null, Damage); 
        }

        void IDamageProvider.OnDamageDealt(IDamageReceiver Receiver, float Damage)
        {
            OnDamageDealt_Internal(Receiver, Damage);
        }

        float IDamageProvider.GetDamage() => Damage; 
        float IDamageProvider.GetForce() => Force; 
    }
}