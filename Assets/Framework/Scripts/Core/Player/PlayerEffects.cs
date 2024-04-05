using System;
using System.Collections.Generic;
using Framework.Common;
using Framework.Common.Tools.AwaitExtensions.Plugins;
using Framework.Scripts.Common;
using Framework.Scripts.Common.Injection;
using Framework.Scripts.Core.ObjectPool;
using UnityEngine;
namespace Framework.Scripts.Core.Player
{
    public class PlayerEffects : MonoBehaviour
    {
        [Injection] private HeroController m_HeroController;
        private MovementController m_MovementController; 
        private ParticleSystem m_JumpParticleSystem;
        private ParticleSystem m_LandParticleSystem;
        [SerializeField] private Renderer[] m_Renderers = Array.Empty<Renderer>();
        private List<(Renderer, Color)> m_CachedRenderers = new(); 
        
        private void Awake()
        {
            m_MovementController = GetComponent<MovementController>(); 
            foreach (Renderer renderer in m_Renderers)
            {
                m_CachedRenderers.Add((renderer, renderer.material.color)); 
            }
        }

        private void OnEnable()
        {
            m_MovementController.Jumped += OnJumped; 
            m_MovementController.Landed += OnLanded; 
            m_HeroController.State.HealthComponent.OnDeath += HandleDeathEffects;
            m_HeroController.State.HealthComponent.OnDamageTaken += HandleDamageTaken; 
        }
        private void OnDisable()
        {
            m_MovementController.Jumped -= OnJumped; 
            m_MovementController.Landed -= OnLanded; 
            m_HeroController.State.HealthComponent.OnDeath -= HandleDeathEffects; 
            m_HeroController.State.HealthComponent.OnDamageTaken -= HandleDamageTaken; 
        }

        private void LateUpdate()
        {
            UpdateParticleSystemModules();
        }

        private void UpdateParticleSystemModules()
        {
            if (m_JumpParticleSystem != null && m_LandParticleSystem != null)
            {
                ParticleSystem.MainModule jumpParticleSystemModule = m_JumpParticleSystem.main;
                jumpParticleSystemModule.startColor = new ParticleSystem.MinMaxGradient(m_HeroController.Settings.ParticleColor); 
                ParticleSystem.MainModule landParticleSystemModule = m_LandParticleSystem.main;
                landParticleSystemModule.startColor = new ParticleSystem.MinMaxGradient(m_HeroController.Settings.ParticleColor); 
            }
        }
        
        protected void HandleDamageTaken()
        {
            OnDamageTakenFlash(10, 0.05f, 0.05f); 
        }

        protected void HandleDeathEffects(IEntity Entity)
        {
            console.log(this, "Player Death Effects");
        }
        
        protected async void OnDamageTakenFlash(int Flashes = 10, float FlashDuration = 0.05f, float Interval = 0.1f)
        {
            // perform a flash effect on the renderer 
            int flashIndex = 0;
            while(flashIndex < Flashes)
            {
                foreach ((Renderer renderer, Color _) in m_CachedRenderers)
                {
                    renderer.material.color = Color.red; 
                }
                await new WaitForSeconds(FlashDuration); 
                foreach ((Renderer renderer, Color originalColor) in m_CachedRenderers)
                {
                    renderer.material.color = originalColor; 
                } 
                await new WaitForSeconds(Interval); 
                flashIndex++; 
            }
        }

        private async void OnLanded(Vector3 Position, float LandingVelocity)
        {
            FlyweightBehaviour result = ObjectPoolFactory.Spawn(m_HeroController.Settings.LandEffect); 
            result.gameObject.Assign(ref m_LandParticleSystem); 
            result.transform.position = transform.position - (Vector3.up * transform.localScale.y / 1.5f); 
            result.transform.rotation = Quaternion.Euler(-90, 0, 0);
            await new WaitForSeconds(1.0f);
            ObjectPoolFactory.Despawn(result); 
            
           // GameObject effect = Instantiate(m_HeroController.Settings.LandEffectPrefab, transform.position - (Vector3.up * transform.localScale.y / 1.5f), Quaternion.Euler(-90, 0, 0));
            // Destroy(effect, 1.0f);
        }

        private async void OnJumped()
        {
            FlyweightBehaviour result = ObjectPoolFactory.Spawn(m_HeroController.Settings.JumpEffect); 
            result.gameObject.Assign(ref m_JumpParticleSystem); 
            result.transform.position = transform.position - (Vector3.up * transform.localScale.y / 2); 
            result.transform.rotation = Quaternion.Euler(-90, 0, 0); 
            await new WaitForSeconds(1.0f); 
            ObjectPoolFactory.Despawn(result); 
            //
            // GameObject effect = Instantiate(m_HeroController.Settings.JumpEffectPrefab, transform.position - (Vector3.up * transform.localScale.y / 2), Quaternion.Euler(-90, 0, 0));
            // Destroy(effect, 1.0f);
        }
    }
}