using Framework.Common;
using Framework.Scripts.Common;
using Framework.Scripts.Common.TriggerToolkit;
using UnityEngine;
using UnityEngine.Events;
namespace Framework.Scripts.Core.Damage
{
    public class DamageOnCollision : AbstractDamageProvider
    {
        [SerializeField] protected UnityEvent OnEnabled = new(); 
        [SerializeField] protected UnityEvent OnDisabled = new(); 
        [SerializeField] protected bool DestroyOnCollision = true;
        [SerializeField] protected TriggerToolkit Detection;

        protected virtual void OnEnable()
        {
            gameObject.AssignIfNull(ref Detection);
            Detection.OnTriggerEvent += OnCollisionDetected;
            OnEnabled?.Invoke(); 
        }
        
        protected virtual void OnDisable()
        {
            Detection.OnTriggerEvent -= OnCollisionDetected;
            OnDisabled?.Invoke(); 
        } 

        private void OnCollisionDetected(Collider2D Collider, TriggerCondition.ETriggerEventType EventType)
        {
            if (EventType is TriggerCondition.ETriggerEventType.ENTER)
            {
                if (Collider.TryGetComponent(out IDamageReceiver Receiver))
                {
                    DamageProcessor.ProcessDamage(this, Receiver);
                }
                if (DestroyOnCollision)
                {
                    Destroy(gameObject);
                }
            } 
        }
    }
}