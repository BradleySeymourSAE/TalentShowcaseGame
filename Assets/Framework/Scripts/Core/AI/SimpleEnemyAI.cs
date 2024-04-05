using System;
using System.Collections.Generic;
using Framework.Common;
using Framework.Common.Tools.AwaitExtensions.Plugins;
using Framework.Scripts.Common;
using Framework.Scripts.Core.AI.Strategies;
using Framework.Scripts.Core.Player;
using UnityEngine;
using UnityEngine.Events;
namespace Framework.Scripts.Core.AI
{
    public class SimpleEnemyAI : AbstractAIEntity
    {
        [SerializeField] protected Renderer[] m_Renderers = Array.Empty<Renderer>();
        protected List<(Renderer, Color)> m_CachedRenderers = new(); 
        
        private int m_CurrentStrategyIndex;

        protected override void Awake()
        {
            base.Awake(); 
            foreach (Renderer renderer in m_Renderers)
            {
                m_CachedRenderers.Add((renderer, renderer.material.color)); 
            } 
        }

        protected override void Update()
        {
            base.Update();
            
            if (IsTargetVisible())
            {
                ExecuteAttack(); 
            }
            
        }

        protected override void HandleOnDamageTaken()
        {    
            base.HandleOnDamageTaken();
            OnDamageTakenFlash(10, 0.05f, 0.05f); 
            // Set the Hurt animation state 
            StateContext.LastAttackTime = 0.0f; 
            StateContext.LinkedAnimator.SetTrigger(Animations.Hurt);
        }

        protected override void HandleOnDeath(IEntity Entity)
        {
            base.HandleOnDeath(Entity);
            console.log(this, "Enemy Entity", this.gameObject.name, "has died");
            StateContext.LinkedAnimator.SetInteger(Animations.State, 9);
            m_CanMove = false; 
            StateContext.LinkedRigidbody.velocity = Vector2.zero; 
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
        
        public WeaponStrategy GetCurrentAttackStrategy()
        {
            if (m_CurrentStrategyIndex < 0 || m_CurrentStrategyIndex >= Settings.AttackStrategies.Length)
            {
                console.warn(this, "Invalid strategy index: " + m_CurrentStrategyIndex); 
                return null;
            } 
            return Settings.AttackStrategies[m_CurrentStrategyIndex]; 
        } 
        
        
        public void SetAttackStrategy(int index)
        {
            if (index < 0 || index >= Settings.AttackStrategies.Length)
            {
                console.error(this, "Invalid strategy index: " + index); 
                return;
            }
            m_CurrentStrategyIndex = index; 
        } 
        
        private bool CanAttack()
        {
            WeaponStrategy strategy = GetCurrentAttackStrategy(); 
            
            return ActiveTarget != null && 
                   AttackOrigin != null && 
                   strategy != null && 
                   Time.time - StateContext.LastAttackTime >= strategy.AttackRate && 
                   DamageBuffer <= 0.0f; 
        } 
        
        

        private void ExecuteAttack()
        {
            if (!CanAttack()) 
            {
                return;
            }
            StateContext.LastAttackTime = Time.time; 
            console.log(this, "Executing attack with strategy: " + Settings.AttackStrategies[m_CurrentStrategyIndex].name);
            StateContext.LinkedAnimator.SetTrigger(Animations.Attack); 
        }
        
        public void OnAnimationAttack()
        {
            console.log(this, "OnAnimationAttack");
            StateContext.LinkedAnimator.ResetTrigger(Animations.Attack);
            Weapon.WeaponStrategy.Attack(StateContext, AttackOrigin, TargetPosition); 
        } 
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected(); 
            if (Settings != null)
            {
                foreach (WeaponStrategy strategy in Settings.AttackStrategies)
                {
                    if (strategy == null)
                    {
                        continue;
                    }
                    strategy.DrawGizmos(this);
                }
            } 
            if (EyesTransform != null)
            {
                Gizmos.color = Color.green;
                // check if the player is within the arc of vision 
                Vector3 direction = TargetPosition - EyesTransform.position; 
                UnityEditor.Handles.Label(EyesTransform.position + Vector3.up * 0.5f, $"Detection Range: {direction.magnitude}"); 
                float angle = Vector3.Angle(EyesTransform.right, direction);

                bool isVisible = (angle < Settings.VisionAngle && direction.sqrMagnitude <= (Settings.VisionRange * Settings.VisionRange)); 
                Color redColor = new Color(1, 0, 0, 0.05f); 
                Color greenColor = new Color(0, 1, 0, 0.05f);
                Gizmos.color = isVisible ? greenColor : redColor; 
                UnityEditor.Handles.color = isVisible ? greenColor : redColor; 
                
                // For 2D, use up instead of right
                Gizmos.DrawLine(EyesTransform.position, EyesTransform.position + EyesTransform.right * Settings.VisionRange);
                Vector3 leftDirection = Quaternion.Euler(0, 0, -Settings.VisionAngle) * EyesTransform.right;
                UnityEditor.Handles.DrawSolidArc(
                    EyesTransform.position, 
                    Vector3.forward, 
                    leftDirection, 2 * Settings.VisionAngle, 
                    Settings.VisionRange);
            }
        }
    }

}