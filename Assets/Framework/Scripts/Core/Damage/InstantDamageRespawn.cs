using Framework.Scripts.Common;
using Framework.Scripts.Common.Injection;
using Framework.Scripts.Common.TriggerToolkit;
using Framework.Scripts.Core.Player;
using Framework.Scripts.Core.Systems;
using UnityEngine;
namespace Framework.Scripts.Core.Damage
{
    public class InstantDamageRespawn : MonoBehaviour, IDamageProvider
    {
        [SerializeField] protected TriggerToolkit m_DetectionComponent;
        [SerializeField] private float DamageValue = 10.0f; 
        [Injection] private HeroController m_HeroController; 
        
        
        protected void OnEnable()
        {
            gameObject.Assign(ref m_DetectionComponent);
            m_DetectionComponent.OnTriggerEvent += HandleDetection; 
        }

        protected void OnDisable()
        {
            m_DetectionComponent.OnTriggerEvent -= HandleDetection; 
        }

        private void HandleDetection(Collider2D Collider, TriggerCondition.ETriggerEventType EventType)
        {
            if (EventType is TriggerCondition.ETriggerEventType.ENTER)
            {
                console.log(this, "Player entered instant respawn trigger. Respawning player...");
                DamageProcessor.ProcessDamage(this, m_HeroController.State.HealthComponent); 
                GameStateManager.Respawn(); 
            }
        }

        float IDamageProvider.GetDamage() => DamageValue; 
    }
}