using Framework.Common;
using Framework.Scripts.Common;
using Framework.Scripts.Core.AI;
using Framework.Scripts.Core.Events;
using Framework.Scripts.Core.Player;
using UnityEngine;
using UnityEngine.Events;
namespace Framework.Scripts.Core.Damage
{
    public class HealthComponent : MonoBehaviour, IDamageReceiver
    {
        public float CurrentHealth { get; protected set; }
        
        protected float PreviousHealth;
        protected float HealthRecoveryDelayRemaining;
        protected IEntity Entity; 
        protected CharacterSettings Settings;
        
        [SerializeField] protected UnityEvent<float, float> OnHealthChanged = new(); 
        [SerializeField] protected UnityEvent OnDeathEvent = new(); 
        [SerializeField] protected UnityEvent OnDamageTakenEvent = new(); 
        public event System.Action OnDamageTaken = delegate { }; 
        public event System.Action<IEntity> OnDeath = delegate { };

        protected virtual void Start()
        {
            PreviousHealth = CurrentHealth = Settings.MaxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, Settings.MaxHealth); 
        }

        
        public void Initialize(IEntity Entity, CharacterSettings Settings)
        {
            this.Entity = Entity;
            this.Settings = Settings; 
        }
        
        
        public void OnPerformHeal(GameObject Source, float Value)
        {
            CurrentHealth = Mathf.Min(CurrentHealth + Value, Settings.MaxHealth);
        }

        protected void Update()
        {
            UpdateHealthStatus();
            UpdateHealthRecovery(); 
        }

        protected void TakeDamage(IDamageProvider Source, float Value)
        {
            console.log(this, Source.gameObject.name, "dealt", Value, "damage to", gameObject.name);
            OnDamageTaken();
            OnDamageTakenEvent?.Invoke(); 

            CurrentHealth = Mathf.Max(CurrentHealth - Value, 0f);
            HealthRecoveryDelayRemaining = Settings.HealthRecoveryDelay;
            
            if (CurrentHealth <= 0f && PreviousHealth >= 0f)
            {
                OnDeath(Entity);
                OnDeathEvent?.Invoke();
            }
        }
        
        protected void UpdateHealthStatus()
        {
            if (PreviousHealth != CurrentHealth)
            {
                PreviousHealth = CurrentHealth; 
                OnHealthChanged?.Invoke(CurrentHealth, Settings.MaxHealth); 
            }
        }

        protected virtual void UpdateHealthRecovery()
        {
            if (Settings != null && CurrentHealth < Settings.MaxHealth)
            {
                if (HealthRecoveryDelayRemaining > 0f)
                {
                    HealthRecoveryDelayRemaining -= Time.deltaTime;
                }

                if (HealthRecoveryDelayRemaining <= 0f)
                {
                    CurrentHealth = Mathf.Min(CurrentHealth + Settings.HealthRecoveryRate * Time.deltaTime, Settings.MaxHealth);
                }
            }
        }
        
        
        void IDamageReceiver.TakeDamage(IDamageProvider Provider, float DamageValue)
        {
            this.TakeDamage(Provider, DamageValue);
        }
    }
}